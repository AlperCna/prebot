using Microsoft.EntityFrameworkCore;
using PreBot.Core.Domain;
using PreBot.Core.Matching;
using PreBot.Core.Services;

namespace PreBot.Tests;

/// <summary>"Bana haber ver" standing alerts and push device storage.</summary>
public sealed class AlertTests : IDisposable
{
    private const ulong Web = Communities.Web;
    private const ulong Host = 1;
    private readonly TestDb _t = new();
    private readonly SubscriptionService _subs;

    // TestDb clock: 2026-11-13 21:00 UTC = 2026-11-14 00:00 in Turkey.
    public AlertTests() => _subs = new SubscriptionService(_t.Factory, _t.Time);

    public void Dispose() => _t.Dispose();

    private Task<int> OpenAsync(Rank min = Rank.Gold, Rank max = Rank.Gold) =>
        _t.Service.CreateListingAsync(Web, Web, Host, Role.Support, min, max, 1, null)
            .ContinueWith(t => t.Result.Listing!.Id);

    private Task AlertAsync(ulong user, Role role = Role.Support, Rank rank = Rank.Gold, int from = 0, int to = 0) =>
        _subs.SaveAlertAsync(Web, user, role, rank, from, to, enabled: true);

    [Theory]
    [InlineData(20, 2, 21, true)]
    [InlineData(20, 2, 1, true)]
    [InlineData(20, 2, 2, false)]
    [InlineData(20, 2, 19, false)]
    [InlineData(18, 23, 22, true)]
    [InlineData(18, 23, 23, false)]
    [InlineData(5, 5, 13, true)]
    public void Hour_windows_can_wrap_past_midnight(int from, int to, int hour, bool expected) =>
        Assert.Equal(expected, Matcher.InHours(from, to, hour));

    [Fact]
    public void Turkey_time_is_utc_plus_three()
    {
        var local = TurkeyTime.ToLocal(new DateTime(2026, 11, 13, 21, 0, 0, DateTimeKind.Utc));
        Assert.Equal(new DateTime(2026, 11, 14, 0, 0, 0), local);
    }

    [Fact]
    public async Task Subscribers_wait_for_wave_two_when_someone_available_fits()
    {
        await _t.Service.SetAvailableAsync(Web, 10, Role.Support, Rank.Gold, TimeSpan.FromHours(1));
        await AlertAsync(20);
        var id = await OpenAsync();

        Assert.Equal([10UL], (await _t.Service.RunNextWaveAsync(id))!.UserIds);

        _t.Time.Advance(Matcher.DelayAfterWave(0));
        Assert.Equal([20UL], (await _t.Service.RunNextWaveAsync(id))!.UserIds);
    }

    [Fact]
    public async Task Subscribers_are_notified_at_once_when_nobody_available_fits()
    {
        await AlertAsync(20);
        var id = await OpenAsync();

        Assert.Equal([20UL], (await _t.Service.RunNextWaveAsync(id))!.UserIds);
    }

    [Fact]
    public async Task Subscribers_outside_their_hours_or_disabled_or_wrong_role_are_skipped()
    {
        await AlertAsync(20, from: 18, to: 23);                    // local time is 00:00 → outside
        await AlertAsync(21, role: Role.Top);                      // wrong role
        await _subs.SaveAlertAsync(Web, 22, Role.Support, Rank.Gold, 0, 0, enabled: false);
        await AlertAsync(23, from: 22, to: 2);                     // wraps midnight → inside
        var id = await OpenAsync();

        Assert.Equal([23UL], (await _t.Service.RunNextWaveAsync(id))!.UserIds);
    }

    [Fact]
    public async Task Subscriber_cooldown_and_daily_limit_are_respected()
    {
        _t.Settings.AlertCooldownMinutes = 30;
        _t.Settings.AlertDailyLimit = 2;
        await AlertAsync(20);

        // 1st listing: notified.
        Assert.Equal([20UL], (await _t.Service.RunNextWaveAsync(await OpenAsync()))!.UserIds);
        await _t.Service.CancelAsync((await _t.Service.GetOpenListingsAsync(Web)).Single().Listing.Id, Host);

        // 10 minutes later: still in cooldown.
        _t.Time.Advance(TimeSpan.FromMinutes(10));
        var second = await OpenAsync();
        Assert.Empty((await _t.Service.RunNextWaveAsync(second))!.UserIds);
        await _t.Service.CancelAsync(second, Host);

        // 31 minutes after the first: notified again (2nd today).
        _t.Time.Advance(TimeSpan.FromMinutes(21));
        var third = await OpenAsync();
        Assert.Equal([20UL], (await _t.Service.RunNextWaveAsync(third))!.UserIds);
        await _t.Service.CancelAsync(third, Host);

        // Cooldown over again, but the daily limit (2) is reached.
        _t.Time.Advance(TimeSpan.FromMinutes(31));
        Assert.Empty((await _t.Service.RunNextWaveAsync(await OpenAsync()))!.UserIds);
    }

    [Fact]
    public async Task Push_subscriptions_move_to_the_last_user_of_a_browser_and_can_be_removed()
    {
        const string endpoint = "https://fcm.googleapis.com/fcm/send/abc";
        await _subs.SavePushSubscriptionAsync(10, endpoint, "p256", "auth", "Windows · Chrome");
        await _subs.SavePushSubscriptionAsync(10, "https://fcm.googleapis.com/fcm/send/phone", "p", "a", "Android · Chrome");
        Assert.Equal(2, await _subs.CountPushDevicesAsync(10));

        await _subs.SavePushSubscriptionAsync(11, endpoint, "p256", "auth", "Windows · Chrome");

        Assert.Equal(1, await _subs.CountPushDevicesAsync(10));
        Assert.Equal(1, await _subs.CountPushDevicesAsync(11));

        await _subs.RemovePushSubscriptionAsync(endpoint);
        Assert.Equal(0, await _subs.CountPushDevicesAsync(11));
    }

    [Fact]
    public async Task Push_endpoint_must_be_https()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _subs.SavePushSubscriptionAsync(10, "http://evil.example/push", "p", "a", null));
    }

    [Fact]
    public async Task Deleting_user_data_removes_alerts_and_devices()
    {
        await AlertAsync(10);
        await _subs.SavePushSubscriptionAsync(10, "https://push.example/x", "p", "a", null);

        await _t.Service.DeleteUserDataAsync(10);

        await using var db = _t.NewContext();
        Assert.False(await db.AlertSubscriptions.AnyAsync(a => a.UserId == 10));
        Assert.False(await db.PushSubscriptions.AnyAsync(p => p.UserId == 10));
    }
}
