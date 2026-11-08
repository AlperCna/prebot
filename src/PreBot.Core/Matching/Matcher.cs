using PreBot.Core.Domain;

namespace PreBot.Core.Matching;

public sealed record MatchCriteria(Role Role, Rank MinRank, Rank MaxRank, ulong HostUserId);

/// <summary>
/// Pure matching rules. A listing notifies the pool in waves: small and strict first,
/// then wider (progressive widening), so the best fits hear first and nobody gets spammed.
/// </summary>
public static class Matcher
{
    public const int MaxWaves = 3;

    /// <summary>How many people to notify in a wave (0-based).</summary>
    public static int WaveSize(int wave) => wave switch
    {
        0 => 5,
        1 => 10,
        _ => 15,
    };

    /// <summary>How many tiers the rank range is widened by in a wave (0-based).</summary>
    public static int WidenBy(int wave) => wave switch
    {
        0 => 0,
        1 => 1,
        _ => 2,
    };

    /// <summary>Wait time after a wave before the next one, if the listing is still open.</summary>
    public static TimeSpan DelayAfterWave(int wave) => wave switch
    {
        0 => TimeSpan.FromMinutes(2),
        _ => TimeSpan.FromMinutes(3),
    };

    /// <summary>
    /// Rank range used in a wave. Widening never reaches Master+ unless the host asked for it,
    /// because a Master+ player forces everyone in a Flex party to be at least Emerald.
    /// </summary>
    public static (Rank Min, Rank Max) WidenedRange(MatchCriteria criteria, int wave)
    {
        var widen = WidenBy(wave);
        var min = (Rank)Math.Max((int)Rank.Iron, (int)criteria.MinRank - widen);
        var cap = criteria.MaxRank == Rank.MasterPlus ? Rank.MasterPlus : Rank.Diamond;
        var max = (Rank)Math.Min((int)cap, (int)criteria.MaxRank + widen);
        return (min, max);
    }

    public static bool RoleFits(Role wanted, Role offered) =>
        wanted == Role.Fill || offered == Role.Fill || wanted == offered;

    public static int RankDistance(Rank rank, Rank min, Rank max)
    {
        if (rank < min)
        {
            return (int)min - (int)rank;
        }

        return rank > max ? (int)rank - (int)max : 0;
    }

    /// <summary>
    /// Picks who to notify in this wave. Order: closest rank, exact role before "Fill",
    /// then whoever has been waiting longest (fairness).
    /// </summary>
    public static IReadOnlyList<Availability> FindCandidates(
        IEnumerable<Availability> pool,
        MatchCriteria criteria,
        int wave,
        DateTime nowUtc,
        TimeSpan pingCooldown,
        IReadOnlySet<ulong> excludedUserIds)
    {
        var (min, max) = WidenedRange(criteria, wave);

        return pool
            .Where(a => a.ExpiresAtUtc > nowUtc)
            .Where(a => a.UserId != criteria.HostUserId && !excludedUserIds.Contains(a.UserId))
            .Where(a => RoleFits(criteria.Role, a.Role))
            .Where(a => a.Rank >= min && a.Rank <= max)
            .Where(a => a.LastPingedAtUtc is null || nowUtc - a.LastPingedAtUtc.Value >= pingCooldown)
            .OrderBy(a => RankDistance(a.Rank, criteria.MinRank, criteria.MaxRank))
            .ThenBy(a => a.Role == criteria.Role ? 0 : 1)
            .ThenBy(a => a.CreatedAtUtc)
            .ThenBy(a => a.UserId)
            .Take(WaveSize(wave))
            .ToList();
    }

    /// <summary>How many available players could fit this listing at all (widest wave, ignoring cooldown).</summary>
    public static int CountPotentialMatches(IEnumerable<Availability> pool, MatchCriteria criteria, DateTime nowUtc)
    {
        var (min, max) = WidenedRange(criteria, MaxWaves - 1);
        return pool.Count(a =>
            a.ExpiresAtUtc > nowUtc
            && a.UserId != criteria.HostUserId
            && RoleFits(criteria.Role, a.Role)
            && a.Rank >= min && a.Rank <= max);
    }
}
