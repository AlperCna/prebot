using Microsoft.EntityFrameworkCore;
using PreBot.Core.Domain;
using PreBot.Core.Matching;

namespace PreBot.Tests;

public sealed class PreServiceTests : IDisposable
{
    private const ulong Host = 1;
    private readonly TestDb _t = new();

    public void Dispose() => _t.Dispose();

    private async Task<int> OpenListingAsync(int slots = 1, Role role = Role.Support, Rank min = Rank.Gold, Rank max = Rank.Platinum)
    {
        var result = await _t.Service.CreateListingAsync(TestDb.Guild, TestDb.Channel, Host, role, min, max, slots, "mikrofon şart");
        Assert.NotNull(result.Listing);
        return result.Listing!.Id;
    }

    private Task AvailableAsync(ulong user, Role role = Role.Support, Rank rank = Rank.Gold) =>
        _t.Service.SetAvailableAsync(TestDb.Guild, user, role, rank, TimeSpan.FromMinutes(60));

    [Fact]
    public async Task Claim_fills_single_seat_and_removes_player_from_pool()
    {
        var id = await OpenListingAsync();
        await AvailableAsync(10);

        var outcome = await _t.Service.TryClaimAsync(id, 10);

        Assert.Equal(ClaimResult.ClaimedAndFilled, outcome.Result);
        Assert.Equal(Host, outcome.HostUserId);
        await using var db = _t.NewContext();
        var listing = await db.Listings.SingleAsync(l => l.Id == id);
        Assert.Equal(ListingStatus.Filled, listing.Status);
        Assert.Equal(1, listing.SlotsFilled);
        Assert.False(await db.Availabilities.AnyAsync(a => a.UserId == 10));
    }

    [Fact]
    public async Task Claim_rejects_host_double_join_and_full_listing()
    {
        var id = await OpenListingAsync(slots: 2);

        Assert.Equal(ClaimResult.IsHost, (await _t.Service.TryClaimAsync(id, Host)).Result);
        Assert.Equal(ClaimResult.Claimed, (await _t.Service.TryClaimAsync(id, 10)).Result);
        Assert.Equal(ClaimResult.AlreadyJoined, (await _t.Service.TryClaimAsync(id, 10)).Result);
        Assert.Equal(ClaimResult.ClaimedAndFilled, (await _t.Service.TryClaimAsync(id, 11)).Result);
        Assert.Equal(ClaimResult.Full, (await _t.Service.TryClaimAsync(id, 12)).Result);
        Assert.Equal(ClaimResult.NotFound, (await _t.Service.TryClaimAsync(999, 12)).Result);
    }

    [Fact]
    public async Task Many_people_clicking_join_at_once_get_exactly_one_seat()
    {
        var id = await OpenListingAsync(slots: 1);

        var results = await Task.WhenAll(
            Enumerable.Range(100, 12).Select(u => Task.Run(() => _t.Service.TryClaimAsync(id, (ulong)u))));

        Assert.Single(results, r => r.Result is ClaimResult.Claimed or ClaimResult.ClaimedAndFilled);
        await using var db = _t.NewContext();
        Assert.Equal(1, await db.ListingSlots.CountAsync(s => s.ListingId == id));
        Assert.Equal(1, (await db.Listings.SingleAsync(l => l.Id == id)).SlotsFilled);
    }

    [Fact]
    public async Task Host_can_have_only_one_open_listing()
    {
        var first = await OpenListingAsync();

        var second = await _t.Service.CreateListingAsync(TestDb.Guild, TestDb.Channel, Host, Role.Mid, Rank.Gold, Rank.Gold, 1, null);

        Assert.Null(second.Listing);
        Assert.Equal(first, second.ExistingOpenListingId);
    }

    [Fact]
    public async Task Leaving_reopens_listing_and_restarts_waves()
    {
        var id = await OpenListingAsync();
        await _t.Service.TryClaimAsync(id, 10);

        Assert.Equal(LeaveResult.Left, await _t.Service.LeaveAsync(id, 10));
        Assert.Equal(LeaveResult.NotJoined, await _t.Service.LeaveAsync(id, 10));

        await using var db = _t.NewContext();
        var listing = await db.Listings.SingleAsync(l => l.Id == id);
        Assert.Equal(ListingStatus.Open, listing.Status);
        Assert.Equal(0, listing.SlotsFilled);
        Assert.Equal(0, listing.WavesSent);

        // Someone who left cannot jump back into the same listing.
        Assert.Equal(ClaimResult.AlreadyJoined, (await _t.Service.TryClaimAsync(id, 10)).Result);
        Assert.Equal(ClaimResult.ClaimedAndFilled, (await _t.Service.TryClaimAsync(id, 11)).Result);
    }

    [Fact]
    public async Task No_show_reopen_is_host_only_and_frees_latest_seat()
    {
        var id = await OpenListingAsync(slots: 2);
        await _t.Service.TryClaimAsync(id, 10);
        _t.Time.Advance(TimeSpan.FromSeconds(30));
        await _t.Service.TryClaimAsync(id, 11);

        Assert.Equal(HostActionResult.NotHost, (await _t.Service.ReopenForNoShowAsync(id, 10)).Result);

        var outcome = await _t.Service.ReopenForNoShowAsync(id, Host);

        Assert.Equal(HostActionResult.Done, outcome.Result);
        Assert.Equal(11UL, outcome.AffectedUserId);
        var view = await _t.Service.GetListingViewAsync(id);
        Assert.Equal(ListingStatus.Open, view!.Listing.Status);
        Assert.Equal([10UL], view.ActiveSlots.Select(s => s.UserId));
    }

    [Fact]
    public async Task Waves_notify_matching_players_once_and_respect_cooldown()
    {
        await AvailableAsync(10, Role.Support, Rank.Gold);
        await AvailableAsync(11, Role.Support, Rank.Silver);   // only fits from wave 1 (widened)
        await AvailableAsync(12, Role.Top, Rank.Gold);         // wrong role
        var id = await OpenListingAsync(min: Rank.Gold, max: Rank.Gold);

        var wave0 = await _t.Service.RunNextWaveAsync(id);
        Assert.NotNull(wave0);
        Assert.Equal(0, wave0!.Wave);
        Assert.Equal([10UL], wave0.UserIds);

        // Running again right away does nothing: next wave is not due yet.
        Assert.Null(await _t.Service.RunNextWaveAsync(id));

        _t.Time.Advance(Matcher.DelayAfterWave(0));
        Assert.Equal([id], await _t.Service.GetDueWaveListingIdsAsync());
        var wave1 = await _t.Service.RunNextWaveAsync(id);
        Assert.Equal([11UL], wave1!.UserIds); // 10 is on cooldown, 11 now fits the widened range
    }

    [Fact]
    public async Task Sweep_expires_old_availability_and_open_listings_but_keeps_filled()
    {
        await AvailableAsync(10);
        var open = await OpenListingAsync();

        _t.Time.Advance(TimeSpan.FromMinutes(5));
        var filled = (await _t.Service.CreateListingAsync(TestDb.Guild, TestDb.Channel, 2, Role.Mid, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;
        await _t.Service.TryClaimAsync(filled, 20);

        _t.Time.Advance(TimeSpan.FromMinutes(61));
        var sweep = await _t.Service.SweepAsync();

        Assert.Equal(1, sweep.ExpiredAvailabilities);
        Assert.Equal([open], sweep.ExpiredListingIds);
        await using var db = _t.NewContext();
        Assert.Equal(ListingStatus.Expired, (await db.Listings.SingleAsync(l => l.Id == open)).Status);
        Assert.Equal(ListingStatus.Filled, (await db.Listings.SingleAsync(l => l.Id == filled)).Status);
    }

    [Fact]
    public async Task Rejoining_pool_keeps_ping_cooldown()
    {
        await AvailableAsync(10);
        var id = await OpenListingAsync();
        await _t.Service.RunNextWaveAsync(id);

        await AvailableAsync(10, Role.Fill, Rank.Gold);

        await using var db = _t.NewContext();
        Assert.NotNull((await db.Availabilities.SingleAsync(a => a.UserId == 10)).LastPingedAtUtc);
    }

    [Fact]
    public async Task Delete_user_data_gives_seats_back_and_removes_everything()
    {
        await _t.Service.SetRiotIdAsync(10, "Oyuncu#TR1");
        var other = await OpenListingAsync();
        await _t.Service.TryClaimAsync(other, 10);
        var own = (await _t.Service.CreateListingAsync(TestDb.Guild, TestDb.Channel, 10, Role.Top, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;
        await _t.Service.AttachMessageAsync(own, 555);

        var result = await _t.Service.DeleteUserDataAsync(10);

        Assert.Equal([other], result.LeftListingIds);
        Assert.Single(result.RemovedListings, r => r.MessageId == 555);
        await using var db = _t.NewContext();
        Assert.False(await db.Players.AnyAsync(p => p.UserId == 10));
        Assert.False(await db.ListingSlots.AnyAsync(s => s.UserId == 10));
        Assert.False(await db.Listings.AnyAsync(l => l.Id == own));
        Assert.Equal(ListingStatus.Open, (await db.Listings.SingleAsync(l => l.Id == other)).Status);
    }

    [Fact]
    public async Task Listing_view_shows_riot_ids()
    {
        await _t.Service.SetRiotIdAsync(Host, "Kaptan#TR1");
        await _t.Service.SetRiotIdAsync(10, "Destekci#TR1");
        var id = await OpenListingAsync();
        await _t.Service.TryClaimAsync(id, 10);

        var view = await _t.Service.GetListingViewAsync(id);

        Assert.Equal("Kaptan#TR1", view!.HostRiotId);
        Assert.Equal("Destekci#TR1", view.ActiveSlots.Single().RiotId);
    }
}
