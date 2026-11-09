using PreBot.Core.Domain;

namespace PreBot.Core.Matching;

public sealed record MatchCriteria(Role Role, Rank MinRank, Rank MaxRank, ulong HostUserId);

/// <summary>
/// Pure matching rules. A listing notifies the pool in waves: small and strict first,
/// then wider (progressive widening), so the best fits hear first and nobody gets spammed.
/// </summary>
public static class Matcher
{
    /// <summary>
    /// Waves 0..2 widen step by step; after that the listing keeps "listening" with the widest settings
    /// (every 3 minutes, only people who were not asked yet) until it fills or expires.
    /// </summary>
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
    /// Rank range used in a wave. Widening never reaches Master+ unless the host asked for it, and when
    /// the host did ask for Master+ it never widens below Emerald: a Master+ player in a Flex party
    /// requires everyone to be at least Emerald.
    /// </summary>
    public static (Rank Min, Rank Max) WidenedRange(MatchCriteria criteria, int wave)
    {
        var widen = WidenBy(wave);
        var floor = criteria.MaxRank == Rank.MasterPlus
            ? (int)(criteria.MinRank < Rank.Emerald ? criteria.MinRank : Rank.Emerald)
            : (int)Rank.Iron;
        var min = (Rank)Math.Max(floor, (int)criteria.MinRank - widen);
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
    /// Picks who to notify in this wave. Order: players we can actually reach first (when known),
    /// then closest rank, exact role before "Fill", then whoever has been waiting longest (fairness).
    /// </summary>
    /// <param name="reachable">Players with a push device or a Discord DM route; null = everyone is reachable.</param>
    public static IReadOnlyList<Availability> FindCandidates(
        IEnumerable<Availability> pool,
        MatchCriteria criteria,
        int wave,
        DateTime nowUtc,
        TimeSpan pingCooldown,
        IReadOnlySet<ulong> excludedUserIds,
        IReadOnlySet<ulong>? reachable = null)
    {
        var (min, max) = WidenedRange(criteria, wave);

        return pool
            .Where(a => a.ExpiresAtUtc > nowUtc)
            .Where(a => a.UserId != criteria.HostUserId && !excludedUserIds.Contains(a.UserId))
            .Where(a => RoleFits(criteria.Role, a.Role))
            .Where(a => a.Rank >= min && a.Rank <= max)
            .Where(a => a.LastPingedAtUtc is null || nowUtc - a.LastPingedAtUtc.Value >= pingCooldown)
            .OrderBy(a => reachable is null || reachable.Contains(a.UserId) ? 0 : 1)
            .ThenBy(a => RankDistance(a.Rank, criteria.MinRank, criteria.MaxRank))
            .ThenBy(a => a.Role == criteria.Role ? 0 : 1)
            .ThenBy(a => a.CreatedAtUtc)
            .ThenBy(a => a.UserId)
            .Take(WaveSize(wave))
            .ToList();
    }

    /// <summary>Most "Bana haber ver" subscribers notified in one wave.</summary>
    public const int AlertWaveSize = 10;

    /// <summary>Is a local hour inside [from, to)? Windows may wrap past midnight; from == to means all day.</summary>
    public static bool InHours(int fromHour, int toHour, int hour)
    {
        if (fromHour == toHour)
        {
            return true;
        }

        return fromHour < toHour
            ? hour >= fromHour && hour < toHour
            : hour >= fromHour || hour < toHour;
    }

    /// <summary>
    /// Picks "Bana haber ver" subscribers for a wave: same role/rank rules as available players,
    /// plus their hour window, a per-person cooldown and a daily limit. Least recently notified go first.
    /// </summary>
    public static IReadOnlyList<AlertSubscription> FindAlertCandidates(
        IEnumerable<AlertSubscription> subscriptions,
        MatchCriteria criteria,
        int wave,
        DateTime nowUtc,
        DateTime localNow,
        TimeSpan cooldown,
        int dailyLimit,
        IReadOnlySet<ulong> excludedUserIds)
    {
        var (min, max) = WidenedRange(criteria, wave);
        var today = DateOnly.FromDateTime(localNow);

        return subscriptions
            .Where(s => s.Enabled)
            .Where(s => s.UserId != criteria.HostUserId && !excludedUserIds.Contains(s.UserId))
            .Where(s => RoleFits(criteria.Role, s.Role))
            .Where(s => s.Rank >= min && s.Rank <= max)
            .Where(s => InHours(s.FromHour, s.ToHour, localNow.Hour))
            .Where(s => s.LastNotifiedAtUtc is null || nowUtc - s.LastNotifiedAtUtc.Value >= cooldown)
            .Where(s => s.CountDate != today || s.CountToday < dailyLimit)
            .OrderBy(s => RankDistance(s.Rank, criteria.MinRank, criteria.MaxRank))
            .ThenBy(s => s.Role == criteria.Role ? 0 : 1)
            .ThenBy(s => s.LastNotifiedAtUtc ?? DateTime.MinValue)
            .ThenBy(s => s.UserId)
            .Take(AlertWaveSize)
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
