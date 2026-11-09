using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;

namespace PreBot.Tests;

/// <summary>Core features the website needs: accounts, notification dispatch, listing queries.</summary>
public sealed class WebCoreTests : IDisposable
{
    private const ulong Web = Communities.Web;
    private readonly TestDb _t = new();
    private readonly AccountService _accounts;

    public WebCoreTests() => _accounts = new AccountService(_t.Factory, _t.Time, _t.Service);

    public void Dispose() => _t.Dispose();

    private sealed class RecordingChannel : INotificationChannel
    {
        public List<WaveAlert> Alerts { get; } = [];

        public Task SendAsync(WaveAlert alert, CancellationToken ct)
        {
            Alerts.Add(alert);
            return Task.CompletedTask;
        }
    }

    private sealed class BrokenChannel : INotificationChannel
    {
        public Task SendAsync(WaveAlert alert, CancellationToken ct) => throw new InvalidOperationException("push service down");
    }

    // ---------- Discord reachability ----------

    /// <summary>Five website players nobody can reach while the site is closed, then one who uses Discord.</summary>
    private async Task<ulong> PoolWithOneDiscordPlayerAsync()
    {
        for (var i = 0; i < Core.Matching.Matcher.WaveSize(0); i++)
        {
            var quiet = await _accounts.GetOrCreateAsync("dev", $"p{i}", $"P{i}");
            await _t.Service.SetAvailableAsync(Web, quiet.UserId, Role.Support, Rank.Gold, TimeSpan.FromHours(1));
        }

        var discord = await _accounts.GetOrCreateAsync("discord", "555", "Discordcu");
        await _t.Service.SetAvailableAsync(Web, discord.UserId, Role.Support, Rank.Gold, TimeSpan.FromHours(1));
        return discord.UserId;
    }

    private async Task<IReadOnlyList<ulong>> FirstWaveAsync() =>
        (await _t.Service.RunNextWaveAsync(
            (await _t.Service.CreateListingAsync(Web, Web, 999, Role.Support, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id))!.UserIds;

    [Fact]
    public async Task A_Discord_player_who_accepts_DMs_is_reachable_and_asked_first()
    {
        var discord = await PoolWithOneDiscordPlayerAsync();
        Assert.Contains(discord, await FirstWaveAsync());
    }

    [Fact]
    public async Task A_Discord_player_whose_DMs_are_closed_is_not_counted_as_reachable_for_a_week()
    {
        var discord = await PoolWithOneDiscordPlayerAsync();
        await _accounts.SetDiscordDmClosedAsync(555, closed: true);

        Assert.DoesNotContain(discord, await FirstWaveAsync());
        Assert.Empty(await _accounts.GetDiscordIdsAsync([discord], dmOpenOnly: true));
        Assert.Single(await _accounts.GetDiscordIdsAsync([discord])); // still a Discord account

        _t.Time.Advance(AccountService.DmRetryAfter);
        Assert.Single(await _accounts.GetDiscordIdsAsync([discord], dmOpenOnly: true)); // tried again after a week

        await _accounts.SetDiscordDmClosedAsync(555, closed: false);
        Assert.Null((await _accounts.FindAsync(discord))!.DmClosedAtUtc);
    }

    [Fact]
    public async Task Without_the_Discord_bot_a_Discord_only_player_is_not_reachable()
    {
        _t.Settings.DiscordDms = false;
        var discord = await PoolWithOneDiscordPlayerAsync();
        Assert.DoesNotContain(discord, await FirstWaveAsync());
    }

    // ---------- Accounts ----------

    [Fact]
    public async Task Same_login_returns_same_account()
    {
        var first = await _accounts.GetOrCreateAsync("dev", "alper", "Alper");
        var second = await _accounts.GetOrCreateAsync("dev", "alper", "Başka İsim");
        var other = await _accounts.GetOrCreateAsync("dev", "mert", "Mert");

        Assert.Equal(first.Id, second.Id);
        Assert.Equal("Alper", second.DisplayName);
        Assert.NotEqual(first.Id, other.Id);
    }

    [Fact]
    public async Task Display_names_are_cleaned_and_validated()
    {
        Assert.Equal("Ali Veli", AccountService.CleanName("  Ali    Veli  "));
        Assert.Equal(32, AccountService.CleanName(new string('x', 50)).Length);

        var account = await _accounts.GetOrCreateAsync("dev", "a", "Alper");
        Assert.False(await _accounts.RenameAsync(account.UserId, " x "));
        Assert.True(await _accounts.RenameAsync(account.UserId, "Yeni İsim"));
        Assert.Equal("Yeni İsim", (await _accounts.FindAsync(account.UserId))!.DisplayName);
    }

    [Fact]
    public async Task Deleting_account_removes_everything_and_frees_seats()
    {
        var host = await _accounts.GetOrCreateAsync("dev", "host", "Host");
        var me = await _accounts.GetOrCreateAsync("dev", "me", "Me");
        await _t.Service.SetRiotIdAsync(me.UserId, "Ben#TR1");
        var listing = (await _t.Service.CreateListingAsync(Web, Web, host.UserId, Role.Support, Rank.Gold, Rank.Gold, 1, null)).Listing!;
        await _t.Service.TryClaimAsync(listing.Id, me.UserId);

        await _accounts.DeleteAccountAsync(me.UserId);

        Assert.Null(await _accounts.FindAsync(me.UserId));
        Assert.Null(await _t.Service.GetRiotIdAsync(me.UserId));
        var view = await _t.Service.GetListingViewAsync(listing.Id);
        Assert.Equal(ListingStatus.Open, view!.Listing.Status);
        Assert.Empty(view.ActiveSlots);
    }

    [Fact]
    public async Task Display_names_lookup_skips_unknown_ids()
    {
        var a = await _accounts.GetOrCreateAsync("dev", "a", "Ayşe");

        var names = await _accounts.GetDisplayNamesAsync([a.UserId, 999_999UL]);

        Assert.Equal("Ayşe", names[a.UserId]);
        Assert.False(names.ContainsKey(999_999UL));
    }

    // ---------- Notifications ----------

    [Fact]
    public async Task Dispatcher_sends_wave_to_every_channel_even_if_one_fails()
    {
        await _t.Service.SetAvailableAsync(Web, 10, Role.Support, Rank.Gold, TimeSpan.FromHours(1));
        var listing = (await _t.Service.CreateListingAsync(Web, Web, 1, Role.Support, Rank.Gold, Rank.Platinum, 1, null)).Listing!;
        var recorder = new RecordingChannel();
        var dispatcher = new WaveDispatcher(_t.Service, [new BrokenChannel(), recorder], NullLogger<WaveDispatcher>.Instance);

        var alert = await dispatcher.RunAsync(listing.Id);

        Assert.NotNull(alert);
        var sent = Assert.Single(recorder.Alerts);
        Assert.Equal([10UL], sent.UserIds);
        Assert.Equal(Web, sent.CommunityId);
        Assert.Equal(1, sent.SlotsMissing);
        Assert.Equal(Role.Support, sent.Role);
    }

    [Fact]
    public async Task Dispatcher_sends_nothing_when_nobody_matches()
    {
        var listing = (await _t.Service.CreateListingAsync(Web, Web, 1, Role.Support, Rank.Gold, Rank.Gold, 1, null)).Listing!;
        var recorder = new RecordingChannel();
        var dispatcher = new WaveDispatcher(_t.Service, [recorder], NullLogger<WaveDispatcher>.Instance);

        Assert.Null(await dispatcher.RunAsync(listing.Id));
        Assert.Empty(recorder.Alerts);
    }

    // ---------- Listing queries ----------

    [Fact]
    public async Task Open_listings_are_per_community_and_exclude_expired_and_filled()
    {
        var open = (await _t.Service.CreateListingAsync(Web, Web, 1, Role.Mid, Rank.Gold, Rank.Gold, 1, null)).Listing!;
        var filled = (await _t.Service.CreateListingAsync(Web, Web, 2, Role.Top, Rank.Gold, Rank.Gold, 1, null)).Listing!;
        await _t.Service.TryClaimAsync(filled.Id, 20);
        await _t.Service.CreateListingAsync(guildId: 555, channelId: 1, hostUserId: 3, Role.Adc, Rank.Gold, Rank.Gold, 1, null);

        var listings = await _t.Service.GetOpenListingsAsync(Web);

        Assert.Equal([open.Id], listings.Select(v => v.Listing.Id));

        _t.Time.Advance(TimeSpan.FromMinutes(16));
        Assert.Empty(await _t.Service.GetOpenListingsAsync(Web));
    }

    [Fact]
    public async Task My_listings_include_hosted_and_joined_but_not_left()
    {
        var hosted = (await _t.Service.CreateListingAsync(Web, Web, 1, Role.Mid, Rank.Gold, Rank.Gold, 1, null)).Listing!;
        var joined = (await _t.Service.CreateListingAsync(Web, Web, 2, Role.Top, Rank.Gold, Rank.Gold, 2, null)).Listing!;
        var left = (await _t.Service.CreateListingAsync(Web, Web, 3, Role.Adc, Rank.Gold, Rank.Gold, 1, null)).Listing!;
        await _t.Service.TryClaimAsync(joined.Id, 1);
        await _t.Service.TryClaimAsync(left.Id, 1);
        await _t.Service.LeaveAsync(left.Id, 1);

        var mine = await _t.Service.GetMyListingsAsync(Web, 1);

        Assert.Equal(new[] { hosted.Id, joined.Id }.Order(), mine.Select(v => v.Listing.Id).Order());
    }

    [Fact]
    public async Task Availability_lookup_ignores_expired_entries()
    {
        await _t.Service.SetAvailableAsync(Web, 10, Role.Jungle, Rank.Silver, TimeSpan.FromMinutes(30));
        Assert.Equal(Role.Jungle, (await _t.Service.GetAvailabilityAsync(Web, 10))!.Role);

        _t.Time.Advance(TimeSpan.FromMinutes(31));
        Assert.Null(await _t.Service.GetAvailabilityAsync(Web, 10));

        await using var db = _t.NewContext();
        Assert.True(await db.Availabilities.AnyAsync(a => a.UserId == 10)); // still there until the sweep runs
    }
}
