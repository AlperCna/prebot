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
    public async Task Deleting_your_data_also_deletes_reports_about_you()
    {
        var host = await AccountAsync("host");
        var reporter = await AccountAsync("reporter");
        await _mod.ReportAsync(await OpenAsync(host), reporter, host, ReportReason.Other, null);

        await _t.Service.DeleteUserDataAsync(host);

        await using var db = _t.NewContext();
        Assert.Empty(await db.ListingReports.ToListAsync());
    }
}
