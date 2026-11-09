using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Tests;

public sealed class StatsTests : IDisposable
{
    private const ulong Web = Communities.Web;
    private readonly TestDb _t = new();
    private readonly StatsService _stats;

    public StatsTests() => _stats = new StatsService(_t.Factory, _t.Time);

    public void Dispose() => _t.Dispose();

    private async Task<int> OpenAsync(ulong host) =>
        (await _t.Service.CreateListingAsync(Web, Web, host, Role.Support, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;

    [Fact]
    public async Task Trial_measures_where_each_join_came_from_and_how_long_it_took()
    {
        const ulong host = 1, pooled = 10, shared = 11;
        await _t.Service.SetAvailableAsync(Web, pooled, Role.Support, Rank.Gold, TimeSpan.FromHours(1));
        var listing = (await _t.Service.CreateListingAsync(Web, Web, host, Role.Support, Rank.Gold, Rank.Gold, 2, null)).Listing!.Id;
        Assert.Contains(pooled, (await _t.Service.RunNextWaveAsync(listing))!.UserIds);

        _t.Time.Advance(TimeSpan.FromMinutes(2));
        await _t.Service.RecordNotificationOpenedAsync(listing, pooled);
        await _t.Service.RecordNotificationOpenedAsync(listing, pooled); // counted once
        _t.Time.Advance(TimeSpan.FromMinutes(1));
        await _t.Service.TryClaimAsync(listing, pooled);
        await _t.Service.TryClaimAsync(listing, shared);                 // came through a shared link, never notified

        var trial = await _stats.GetTrialAsync(Web, TimeSpan.FromDays(1));
        var pool = trial.BySource.Single(f => f.Source == NotificationSource.Pool);
        Assert.Equal((1, 1, 1), (pool.Sent, pool.Opened, pool.Claimed));
        Assert.Equal(120, pool.MedianSecondsToOpen);
        Assert.Equal(180, pool.MedianSecondsToClaim);
        Assert.Equal(1, trial.ClaimsWithoutNotification);
        Assert.Equal(0, trial.LateClaimShare);
    }

    [Fact]
    public async Task Player_in_a_match_is_skipped_and_offered_the_listing_again_afterwards()
    {
        const ulong host = 1, gamer = 20, alertOnly = 21;
        await _t.Service.SetAvailableAsync(Web, gamer, Role.Support, Rank.Gold, TimeSpan.FromHours(2));
        await using (var db = _t.NewContext())
        {
            db.AlertSubscriptions.Add(new AlertSubscription
            {
                CommunityId = Web, UserId = alertOnly, Role = Role.Support, Rank = Rank.Gold,
                FromHour = 0, ToHour = 0, Enabled = true, CreatedAtUtc = _t.Time.GetUtcNow().UtcDateTime,
            });
            await db.SaveChangesAsync();
        }

        var listing = (await _t.Service.CreateListingAsync(Web, Web, host, Role.Support, Rank.Gold, Rank.Gold, 2, null)).Listing!.Id;
        Assert.Contains(gamer, (await _t.Service.RunNextWaveAsync(listing))!.UserIds);

        // "Maçtayım" from the notification: quiet for a while, and the host can see someone is coming back.
        var until = await _t.Service.ReportInGameAsync(gamer, listing);
        Assert.Equal(_t.Time.GetUtcNow().UtcDateTime + PreService.InGameFor, until);
        Assert.Equal(1, (await _t.Service.GetWaitingAfterMatchAsync(listing)).Count);
        await _t.Service.ReportInGameAsync(alertOnly, listing);
        _t.Time.Advance(TimeSpan.FromMinutes(5));
        var next = await _t.Service.RunNextWaveAsync(listing);
        Assert.DoesNotContain(gamer, next?.UserIds ?? []);
        Assert.DoesNotContain(alertOnly, next?.UserIds ?? []);
        Assert.Empty(await _t.Service.TakeFinishedMatchesAsync()); // not over yet

        // The match ends early ("Maç bitti"): the same listing comes back, marked as an after-match notification.
        Assert.Equal(listing, await _t.Service.EndInGameAsync(gamer));
        var again = await _t.Service.MatchAfterMatchAsync(Web, gamer, listing);
        Assert.Equal(listing, again!.ListingId);
        Assert.Equal([gamer], again.UserIds);

        // The other one waits out the timer. By then the listing has usually expired (15 minutes), so the most
        // urgent listing that fits is offered instead; a "Bana haber ver" subscriber is matched on that setting.
        _t.Time.Advance(PreService.InGameFor);
        var finished = Assert.Single(await _t.Service.TakeFinishedMatchesAsync());
        Assert.Equal((alertOnly, (int?)listing), finished);
        Assert.Empty(await _t.Service.TakeFinishedMatchesAsync()); // handed out once
        await _t.Service.SweepAsync();
        var fresh = (await _t.Service.CreateListingAsync(Web, Web, 2, Role.Support, Rank.Silver, Rank.Platinum, 1, null)).Listing!.Id;
        Assert.Equal(fresh, (await _t.Service.MatchAfterMatchAsync(Web, alertOnly, finished.ListingId))!.ListingId);

        var trial = await _stats.GetTrialAsync(Web, TimeSpan.FromDays(1));
        Assert.Equal(1, trial.BySource.Single(f => f.Source == NotificationSource.Pool).InGame);
        Assert.Equal(2, trial.BySource.Single(f => f.Source == NotificationSource.AfterMatch).Sent);
    }

    [Fact]
    public async Task Empty_community_has_no_rates()
    {
        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(7));

        Assert.Equal(0, s.ListingsOpened);
        Assert.Null(s.FillRate);
        Assert.Null(s.MedianMinutesToFill);
    }

    [Fact]
    public async Task Fill_rate_and_median_time_to_fill_are_measured_on_finished_listings()
    {
        // Filled after 2 minutes.
        var a = await OpenAsync(1);
        _t.Time.Advance(TimeSpan.FromMinutes(2));
        await _t.Service.TryClaimAsync(a, 10);

        // Filled after 6 minutes.
        var b = await OpenAsync(2);
        _t.Time.Advance(TimeSpan.FromMinutes(6));
        await _t.Service.TryClaimAsync(b, 11);

        // Closed without anyone joining.
        var c = await OpenAsync(3);
        await _t.Service.CancelAsync(c, 3);

        // Still open: not counted as finished.
        await OpenAsync(4);

        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(7));

        Assert.Equal(4, s.ListingsOpened);
        Assert.Equal(3, s.ListingsFinished);
        Assert.Equal(2, s.ListingsFilled);
        Assert.Equal(2.0 / 3, s.FillRate!.Value, 3);
        Assert.Equal(4, s.MedianMinutesToFill!.Value, 3); // median of 2 and 6
        Assert.Equal(2, s.FastestMinutesToFill!.Value, 3);
        Assert.Equal(6, s.ActivePlayers); // 4 hosts + 2 joiners
    }

    [Fact]
    public async Task History_survives_when_the_host_deletes_their_data()
    {
        var a = await OpenAsync(1);
        _t.Time.Advance(TimeSpan.FromMinutes(3));
        await _t.Service.TryClaimAsync(a, 10);

        await _t.Service.DeleteUserDataAsync(1);

        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(7));
        Assert.Equal(1, s.ListingsOpened);
        Assert.Equal(1, s.ListingsFilled);
        Assert.Equal(3, s.MedianMinutesToFill!.Value, 3);
    }

    [Fact]
    public async Task No_shows_are_counted_apart_from_players_who_leave_on_their_own()
    {
        var a = await OpenAsync(1);
        await _t.Service.TryClaimAsync(a, 10);
        await _t.Service.ReopenForNoShowAsync(a, 1); // 10 did not show up
        await _t.Service.TryClaimAsync(a, 11);
        await _t.Service.LeaveAsync(a, 11);          // 11 changed their mind

        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(7));

        Assert.Equal(2, s.SeatsTaken);
        Assert.Equal(1, s.NoShows);
        Assert.Equal(1, s.ListingsFilled); // it did fill once
        await using var db = _t.NewContext();
        Assert.Equal(
            [SeatReleaseReason.NoShow, SeatReleaseReason.Voluntary],
            db.ListingSlots.OrderBy(x => x.Id).Select(x => x.ReleaseReason!.Value).ToList());
    }

    [Fact]
    public async Task Closed_listings_are_deleted_after_90_days_but_statistics_stay()
    {
        var old = await OpenAsync(1);
        await _t.Service.TryClaimAsync(old, 10);
        _t.Time.Advance(TimeSpan.FromDays(91));
        var open = await OpenAsync(2);

        Assert.Equal(1, await _t.Service.PurgeOldListingsAsync());

        await using var db = _t.NewContext();
        Assert.Equal([open], db.Listings.Select(l => l.Id).ToList());
        Assert.Empty(db.ListingSlots.Where(s => s.ListingId == old));
        Assert.Equal(1, (await _stats.GetAsync(Web, TimeSpan.FromDays(120))).ListingsFilled);
    }

    [Fact]
    public async Task Player_found_through_the_custom_lobby_counts_as_filled_and_stops_the_listing()
    {
        var id = (await _t.Service.CreateListingAsync(Web, Web, 1, Role.Support, Rank.Iron, Rank.Diamond, 1, null, lobbyCode: "4821", lobbyLocked: true)).Listing!.Id;
        _t.Time.Advance(TimeSpan.FromMinutes(4));

        Assert.Equal(HostActionResult.NotHost, (await _t.Service.MarkFilledViaLobbyAsync(id, 99)).Result);
        Assert.Equal(HostActionResult.Done, (await _t.Service.MarkFilledViaLobbyAsync(id, 1)).Result);
        Assert.Equal(HostActionResult.NothingToDo, (await _t.Service.MarkFilledViaLobbyAsync(id, 1)).Result);

        await using var db = _t.NewContext();
        var listing = db.Listings.Single(l => l.Id == id);
        Assert.Equal(ListingStatus.Filled, listing.Status);
        Assert.Null(listing.NextWaveAtUtc);
        Assert.Equal(("4821", true), (listing.LobbyCode, listing.LobbyLocked));
        Assert.Matches("^[a-z2-9]{6}$", listing.LobbyPassword); // not the code that is in the public name
        var s = await _stats.GetAsync(Web, TimeSpan.FromDays(1));
        Assert.Equal(1, s.ListingsFilled);
        Assert.Equal(4, s.MedianMinutesToFill!.Value, 3);
    }

    [Fact]
    public async Task A_lobby_join_takes_one_seat_and_only_the_last_one_closes_the_listing()
    {
        var id = (await _t.Service.CreateListingAsync(Web, Web, 1, Role.Support, Rank.Iron, Rank.Diamond, 2, null, lobbyCode: "4821")).Listing!.Id;

        Assert.Equal(HostActionResult.Done, (await _t.Service.MarkFilledViaLobbyAsync(id, 1)).Result);
        var half = (await _t.Service.GetListingViewAsync(id))!.Listing;
        Assert.Equal((ListingStatus.Open, 1), (half.Status, half.SlotsFilled)); // still looking for one more
        Assert.Equal(0, (await _stats.GetAsync(Web, TimeSpan.FromDays(1))).ListingsFilled);

        Assert.Equal(HostActionResult.Done, (await _t.Service.MarkFilledViaLobbyAsync(id, 1)).Result);
        var full = (await _t.Service.GetListingViewAsync(id))!.Listing;
        Assert.Equal((ListingStatus.Filled, 2), (full.Status, full.SlotsFilled));
        Assert.Equal(1, (await _stats.GetAsync(Web, TimeSpan.FromDays(1))).ListingsFilled);
    }

    [Fact]
    public void Lobby_name_is_plain_ascii_and_notification_line_mentions_the_password_only_when_locked()
    {
        Assert.Equal("PREBOT SUP 4821", Lobby.Name(Role.Support, "4821"));
        Assert.Equal("PREBOT JG 1000", Lobby.Name(Role.Jungle, "1000"));
        Assert.EndsWith("PREBOT MID 1234 (şifre k7m2qx)", Lobby.Hint(Role.Mid, "1234", "k7m2qx"));
        Assert.EndsWith("PREBOT MID 1234", Lobby.Hint(Role.Mid, "1234", null));
        Assert.All(Enum.GetValues<Role>(), r => Assert.InRange(Lobby.Name(r, "1234").Length, 3, 30)); // client limit
        Assert.DoesNotContain("k7m2qx", Lobby.PublicHint(Role.Mid, "1234", locked: true));
        Assert.Contains("şifreli", Lobby.PublicHint(Role.Mid, "1234", locked: true));
        Assert.InRange(int.Parse(Lobby.NewCode()), 1000, 9999);
    }

    [Fact]
    public async Task Old_listings_fall_out_of_the_window_and_other_communities_are_ignored()
    {
        await OpenAsync(1);
        await _t.Service.CreateListingAsync(guildId: 555, channelId: 1, hostUserId: 2, Role.Top, Rank.Gold, Rank.Gold, 1, null);
        _t.Time.Advance(TimeSpan.FromDays(2));

        Assert.Equal(0, (await _stats.GetAsync(Web, TimeSpan.FromDays(1))).ListingsOpened);
        Assert.Equal(1, (await _stats.GetAsync(Web, TimeSpan.FromDays(7))).ListingsOpened);
    }
}
