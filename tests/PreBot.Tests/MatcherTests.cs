using PreBot.Core.Domain;
using PreBot.Core.Matching;

namespace PreBot.Tests;

public class MatcherTests
{
    private static readonly DateTime Now = new(2026, 11, 13, 21, 0, 0, DateTimeKind.Utc);
    private static readonly TimeSpan Cooldown = TimeSpan.FromMinutes(15);
    private static readonly HashSet<ulong> None = [];

    private static Availability A(ulong user, Role role, Rank rank, int waitedMinutes = 10, DateTime? lastPinged = null) => new()
    {
        UserId = user,
        Role = role,
        Rank = rank,
        CreatedAtUtc = Now.AddMinutes(-waitedMinutes),
        ExpiresAtUtc = Now.AddMinutes(30),
        LastPingedAtUtc = lastPinged,
    };

    private static MatchCriteria Support(Rank min, Rank max) => new(Role.Support, min, max, HostUserId: 1);

    [Fact]
    public void Wave0_takes_only_matching_role_and_rank()
    {
        var pool = new[]
        {
            A(10, Role.Support, Rank.Gold),
            A(11, Role.Mid, Rank.Gold),         // wrong role
            A(12, Role.Support, Rank.Diamond),  // too high
            A(13, Role.Fill, Rank.Platinum),    // fill fits any role
        };

        var picked = Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Platinum), 0, Now, Cooldown, None);

        Assert.Equal([10UL, 13UL], picked.Select(p => p.UserId).ToArray());
    }

    [Fact]
    public void Exact_role_comes_before_fill_and_longest_waiting_first()
    {
        var pool = new[]
        {
            A(20, Role.Fill, Rank.Gold, waitedMinutes: 50),
            A(21, Role.Support, Rank.Gold, waitedMinutes: 5),
            A(22, Role.Support, Rank.Gold, waitedMinutes: 30),
        };

        var picked = Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Gold), 0, Now, Cooldown, None);

        Assert.Equal([22UL, 21UL, 20UL], picked.Select(p => p.UserId).ToArray());
    }

    [Fact]
    public void Later_waves_widen_rank_range()
    {
        var pool = new[] { A(30, Role.Support, Rank.Silver), A(31, Role.Support, Rank.Bronze) };
        var criteria = Support(Rank.Gold, Rank.Gold);

        Assert.Empty(Matcher.FindCandidates(pool, criteria, 0, Now, Cooldown, None));
        Assert.Equal([30UL], Matcher.FindCandidates(pool, criteria, 1, Now, Cooldown, None).Select(p => p.UserId));
        Assert.Equal([30UL, 31UL], Matcher.FindCandidates(pool, criteria, 2, Now, Cooldown, None).Select(p => p.UserId));
    }

    [Fact]
    public void Widening_never_reaches_master_plus_unless_asked()
    {
        var criteria = Support(Rank.Emerald, Rank.Diamond);
        Assert.Equal((Rank.Gold, Rank.Diamond), Matcher.WidenedRange(criteria, 2));

        // With Master+ in range everyone must be at least Emerald, so widening stops there.
        var withMaster = Support(Rank.Diamond, Rank.MasterPlus);
        Assert.Equal((Rank.Emerald, Rank.MasterPlus), Matcher.WidenedRange(withMaster, 2));

        // The host's own lower bound is kept, it just never widens further down.
        var lowWithMaster = Support(Rank.Gold, Rank.MasterPlus);
        Assert.Equal((Rank.Gold, Rank.MasterPlus), Matcher.WidenedRange(lowWithMaster, 2));
    }

    [Fact]
    public void Reachable_players_are_asked_first()
    {
        var now = new DateTime(2026, 11, 9, 20, 0, 0, DateTimeKind.Utc);
        var pool = new[]
        {
            new Availability { UserId = 10, Role = Role.Support, Rank = Rank.Gold, ExpiresAtUtc = now.AddHours(1), CreatedAtUtc = now.AddMinutes(-30) },
            new Availability { UserId = 11, Role = Role.Support, Rank = Rank.Gold, ExpiresAtUtc = now.AddHours(1), CreatedAtUtc = now.AddMinutes(-5) },
        };

        var picked = Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Gold), 0, now, TimeSpan.FromMinutes(3), new HashSet<ulong>(), reachable: new HashSet<ulong> { 11 });

        Assert.Equal([11UL, 10UL], picked.Select(a => a.UserId)); // 10 waited longer but has no push device
    }

    [Fact]
    public void Recently_pinged_players_are_skipped()
    {
        var pool = new[]
        {
            A(40, Role.Support, Rank.Gold, lastPinged: Now.AddMinutes(-5)),
            A(41, Role.Support, Rank.Gold, lastPinged: Now.AddMinutes(-20)),
        };

        var picked = Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Gold), 0, Now, Cooldown, None);

        Assert.Equal([41UL], picked.Select(p => p.UserId));
    }

    [Fact]
    public void Host_excluded_and_expired_entries_are_skipped()
    {
        var expired = A(51, Role.Support, Rank.Gold);
        expired.ExpiresAtUtc = Now.AddSeconds(-1);
        var pool = new[] { A(1, Role.Support, Rank.Gold), expired, A(52, Role.Support, Rank.Gold), A(53, Role.Support, Rank.Gold) };

        var picked = Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Gold), 0, Now, Cooldown, new HashSet<ulong> { 53 });

        Assert.Equal([52UL], picked.Select(p => p.UserId));
    }

    [Fact]
    public void Wave_size_is_capped()
    {
        var pool = Enumerable.Range(100, 30).Select(i => A((ulong)i, Role.Support, Rank.Gold)).ToArray();

        Assert.Equal(5, Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Gold), 0, Now, Cooldown, None).Count);
        Assert.Equal(10, Matcher.FindCandidates(pool, Support(Rank.Gold, Rank.Gold), 1, Now, Cooldown, None).Count);
    }

    [Theory]
    [InlineData("Faker#KR1", true, "Faker#KR1")]
    [InlineData("  Ali Veli #TR1 ", true, "Ali Veli#TR1")]
    [InlineData("ab#TR1", false, null)]
    [InlineData("NoTagHere", false, null)]
    [InlineData("Name#T", false, null)]
    public void Riot_id_format_is_checked(string input, bool ok, string? expected)
    {
        Assert.Equal(ok, RiotIdValidator.TryNormalize(input, out var normalized));
        if (ok)
        {
            Assert.Equal(expected, normalized);
        }
    }
}
