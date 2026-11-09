using Microsoft.EntityFrameworkCore;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Tests;

public sealed class ModerationTests : IDisposable
{
    private const ulong Web = Communities.Web;
    private readonly TestDb _t = new();
    private readonly ModerationService _mod;

    public ModerationTests() => _mod = new ModerationService(_t.Factory, _t.Time);

    public void Dispose() => _t.Dispose();

    private async Task<ulong> AccountAsync(string name)
    {
        await using var db = _t.NewContext();
        var account = new UserAccount { Provider = "invite", ProviderKey = name, DisplayName = name, CreatedAtUtc = _t.Time.GetUtcNow().UtcDateTime };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return account.UserId;
    }

    private async Task<int> OpenAsync(ulong host) =>
        (await _t.Service.CreateListingAsync(Web, Web, host, Role.Support, Rank.Iron, Rank.Diamond, 1, null)).Listing!.Id;

    [Fact]
    public async Task Muted_and_banned_players_are_not_notified_until_unmuted()
    {
        var host = await AccountAsync("host");
        var muted = await AccountAsync("muted");
        var banned = await AccountAsync("banned");
        var normal = await AccountAsync("normal");
        foreach (var u in new[] { muted, banned, normal })
        {
            await _t.Service.SetAvailableAsync(Web, u, Role.Support, Rank.Gold, TimeSpan.FromHours(2));
        }

        await _t.Service.MuteAsync(muted, _t.Time.GetUtcNow().UtcDateTime.AddHours(1));
        await _mod.BanAsync(banned);

        var first = await OpenAsync(host);
        Assert.Equal([normal], (await _t.Service.RunNextWaveAsync(first))!.UserIds);
        Assert.NotNull(await _t.Service.GetMutedUntilAsync(muted));

        // An hour later the mute is over; the banned player stays out (and lost their pool entry).
        _t.Time.Advance(TimeSpan.FromMinutes(61));
        Assert.Null(await _t.Service.GetMutedUntilAsync(muted));
        var second = await OpenAsync(await AccountAsync("host2"));
        var wave = await _t.Service.RunNextWaveAsync(second);
        Assert.Contains(muted, wave!.UserIds);
        Assert.DoesNotContain(banned, wave.UserIds);
        Assert.True(await _mod.IsBannedAsync(banned));
    }

    [Fact]
    public async Task Reports_can_be_listed_and_resolved()
    {
        var host = await AccountAsync("host");
        var reporter = await AccountAsync("reporter");
        var id = await OpenAsync(host);

        await _mod.ReportAsync(id, reporter, host, ReportReason.Toxic, "  küfür etti  ");

        var report = Assert.Single(await _mod.GetOpenReportsAsync());
        Assert.Equal((id, reporter, (ulong?)host, ReportReason.Toxic, "küfür etti"),
            (report.ListingId, report.ReporterUserId, report.ReportedUserId, report.Reason, report.Note));

        await _mod.ResolveReportAsync(report.Id);
        Assert.Empty(await _mod.GetOpenReportsAsync());
    }

    [Fact]
    public async Task Host_sees_how_often_a_player_did_not_show_up()
    {
        var player = await AccountAsync("player");
        await _t.Service.SetRiotIdAsync(player, "Player#TR1");
        for (var i = 0; i < 2; i++)
        {
            var host = await AccountAsync($"host{i}");
            var id = await OpenAsync(host);
            await _t.Service.TryClaimAsync(id, player);
            await _t.Service.ReopenForNoShowAsync(id, host);
        }

        var counts = await _mod.GetNoShowCountsAsync([player, 999]);
        Assert.Equal(2, counts[player]);
        Assert.False(counts.ContainsKey(999));
    }

    [Fact]
    public async Task Deleting_your_data_keeps_reports_others_made_about_you_but_not_your_own()
    {
        var host = await AccountAsync("host");
        var reporter = await AccountAsync("reporter");
        var listing = await OpenAsync(host);
        Assert.True(await _mod.ReportAsync(listing, reporter, host, ReportReason.Other, null));
        Assert.True(await _mod.ReportAsync(listing, host, null, ReportReason.Other, "kendi ilanim"));

        await _t.Service.DeleteUserDataAsync(host);

        await using var db = _t.NewContext();
        var left = Assert.Single(await db.ListingReports.ToListAsync());
        Assert.Equal(reporter, left.ReporterUserId);
    }

    [Fact]
    public async Task Being_available_is_capped_at_three_hours()
    {
        var entry = await _t.Service.SetAvailableAsync(Web, 5, Role.Mid, Rank.Gold, TimeSpan.FromHours(10));
        Assert.Equal(_t.Time.GetUtcNow().UtcDateTime.AddHours(3), entry.ExpiresAtUtc);
    }

    [Fact]
    public async Task A_report_can_only_name_the_host_or_someone_who_held_a_seat()
    {
        var host = await AccountAsync("host");
        var player = await AccountAsync("player");
        var stranger = await AccountAsync("stranger");
        var reporter = await AccountAsync("reporter");
        var listing = await OpenAsync(host);
        await _t.Service.SetRiotIdAsync(player, "Player#TR1");
        await _t.Service.TryClaimAsync(listing, player);

        Assert.True(await _mod.ReportAsync(listing, reporter, host, ReportReason.Other, null));
        Assert.True(await _mod.ReportAsync(listing, reporter, player, ReportReason.NoShow, null));
        Assert.False(await _mod.ReportAsync(listing, reporter, stranger, ReportReason.Other, null));
        Assert.False(await _mod.ReportAsync(listing + 100, reporter, null, ReportReason.Other, null));

        await _mod.BanAsync(reporter);
        Assert.False(await _mod.ReportAsync(listing, reporter, host, ReportReason.Other, null));
    }

    [Fact]
    public async Task Ban_gives_back_seats_and_closes_the_banned_players_listings()
    {
        var host = await AccountAsync("host");
        var banned = await AccountAsync("banned");
        var theirs = await OpenAsync(banned);
        var other = await OpenAsync(host);
        await _t.Service.SetRiotIdAsync(banned, "Banned#TR1");
        Assert.Equal(ClaimResult.ClaimedAndFilled, (await _t.Service.TryClaimAsync(other, banned)).Result);

        await _mod.BanAsync(banned);
        var (left, closed) = await _t.Service.RemoveBannedUserAsync(banned);

        Assert.Equal([other], left);
        Assert.Equal([theirs], closed);
        var otherView = (await _t.Service.GetListingViewAsync(other))!;
        Assert.Equal(ListingStatus.Open, otherView.Listing.Status); // the host is looking again
        Assert.Equal(0, otherView.Listing.SlotsFilled);
        Assert.Equal(ListingStatus.Cancelled, (await _t.Service.GetListingViewAsync(theirs))!.Listing.Status);
    }
}
