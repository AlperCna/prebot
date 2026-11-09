using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;
using PreBot.Web.Push;

namespace PreBot.Web.Live;

/// <summary>
/// What the website's buttons do. Each action changes the data through <see cref="PreService"/>,
/// sends notification waves where needed, tells every open page to refresh, and — in the background,
/// so no click ever waits for it — notifies the host and keeps the Discord announcement up to date.
/// </summary>
public sealed class PoolActions(
    PreService pre,
    WaveDispatcher waves,
    LiveUpdates live,
    IListingFeed feed,
    HostNotifier host,
    ActionLimiter limiter,
    ModerationService moderation,
    ILogger<PoolActions> logger)
{
    private const ulong Web = Communities.Web;

    public async Task<Availability> SetAvailableAsync(ulong userId, Role role, Rank rank, TimeSpan duration)
    {
        // A page opened before the ban keeps working until it reloads; its buttons must not.
        if (await moderation.IsBannedAsync(userId))
        {
            throw new InvalidOperationException("This account was closed.");
        }

        var entry = await pre.SetAvailableAsync(Web, userId, role, rank, duration);
        live.PublishChanged();

        // A listing may already be waiting for exactly this player: tell them now, not at its next wave.
        if (await pre.MatchNewcomerAsync(Web, userId) is { } wave)
        {
            Background($"newcomer match {wave.ListingId}", () => waves.DispatchAsync(wave));
        }

        return entry;
    }

    public async Task ClearAvailableAsync(ulong userId)
    {
        await pre.ClearAvailableAsync(Web, userId);
        live.PublishChanged();
    }

    /// <param name="withLobby">The host also opens a custom game lobby in the client; notifications point to it.</param>
    /// <param name="lobbyLocked">Give that lobby a password, so only players PreBot told can enter.</param>
    /// <param name="announceChannelId">Discord channel where /pre-lazim was used: the listing is announced there too.</param>
    /// <param name="reuseLobby">Reopening: keep the lobby (name and password) the host already has open in the client.</param>
    public async Task<CreateListingResult> CreateListingAsync(
        ulong hostUserId, Role role, Rank min, Rank max, int slots, string? note,
        bool withLobby = false, bool lobbyLocked = false, ulong? announceChannelId = null, Listing? reuseLobby = null)
    {
        if (!limiter.TryAcquire(LimitedAction.CreateListing, hostUserId, out var retryAfter))
        {
            return new CreateListingResult(null, null, 0, retryAfter);
        }

        if (await moderation.IsBannedAsync(hostUserId))
        {
            return new CreateListingResult(null, null, 0);
        }

        var result = await pre.CreateListingAsync(
            Web, Web, hostUserId, role, min, max, slots, note,
            withLobby ? reuseLobby?.LobbyCode ?? Lobby.NewCode() : null, lobbyLocked, reuseLobby?.LobbyPassword);
        if (result.Listing is not null)
        {
            live.PublishChanged();
            await waves.RunAsync(result.Listing.Id);
            var listing = result.Listing;
            Background($"announce listing {listing.Id}", () => feed.PublishAsync(listing, announceChannelId));
        }

        return result;
    }

    public async Task<ClaimOutcome> ClaimAsync(int listingId, ulong userId)
    {
        if (!limiter.TryAcquire(LimitedAction.Join, userId, out var retryAfter))
        {
            return new ClaimOutcome(ClaimResult.RateLimited, RetryAfter: retryAfter);
        }

        if (await moderation.IsBannedAsync(userId))
        {
            return new ClaimOutcome(ClaimResult.NotFound);
        }

        // Without a Riot ID the host cannot invite them, and a full listing stops looking for anyone else.
        if (await pre.GetRiotIdAsync(userId) is null)
        {
            return new ClaimOutcome(ClaimResult.NeedsRiotId);
        }

        var outcome = await pre.TryClaimAsync(listingId, userId);
        if (outcome.Result is ClaimResult.Claimed or ClaimResult.ClaimedAndFilled)
        {
            live.PublishChanged();
            Background($"notify host of join {listingId}", () => host.JoinedAsync(listingId, userId));
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
        }

        return outcome;
    }

    public async Task<LeaveResult> LeaveAsync(int listingId, ulong userId)
    {
        var result = await pre.LeaveAsync(listingId, userId);
        if (result == LeaveResult.Left)
        {
            live.PublishChanged();
            await waves.RunAsync(listingId);
            Background($"notify host of leave {listingId}", () => host.LeftAsync(listingId, userId));
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
        }

        return result;
    }

    /// <summary>"Maçtayım, bitince haber ver": no notifications for a while, then this listing (or another) again.</summary>
    public async Task<DateTime> ReportInGameAsync(ulong userId, int? listingId)
    {
        var until = await pre.ReportInGameAsync(userId, listingId);
        live.PublishChanged();
        return until;
    }

    /// <summary>"Maç bitti, hazırım": back early; a fitting open listing is offered right away.</summary>
    public async Task MatchFinishedAsync(ulong userId)
    {
        var putOff = await pre.EndInGameAsync(userId);
        if (await pre.MatchAfterMatchAsync(Web, userId, putOff) is { } wave)
        {
            await waves.DispatchAsync(wave);
        }

        live.PublishChanged();
    }

    /// <summary>The host frees one seat: "Gelmedi" (no-show) or "Uygun değil" (declined, not held against them).</summary>
    public async Task<HostActionOutcome> ReleaseByHostAsync(int listingId, ulong hostUserId, ulong userId, SeatReleaseReason reason)
    {
        var outcome = await pre.ReleaseByHostAsync(listingId, hostUserId, userId, reason);
        if (outcome.Result == HostActionResult.Done)
        {
            live.PublishChanged();
            await waves.RunAsync(listingId);
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
            if (reason == SeatReleaseReason.HostDeclined)
            {
                Background($"tell declined player {listingId}", () => host.DeclinedAsync(listingId, userId));
            }
        }

        return outcome;
    }

    public async Task<HostActionOutcome> CancelAsync(int listingId, ulong hostUserId)
    {
        var outcome = await pre.CancelAsync(listingId, hostUserId);
        if (outcome.Result == HostActionResult.Done)
        {
            live.PublishChanged();
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
        }

        return outcome;
    }

    /// <summary>Opens the same listing again (after it expired or was closed). Same rules as a new one.</summary>
    public async Task<CreateListingResult> ReopenAsync(int listingId, ulong hostUserId)
    {
        var view = await pre.GetListingViewAsync(listingId);
        if (view is null || view.Listing.HostUserId != hostUserId || view.Listing.GuildId != Web)
        {
            return new CreateListingResult(null, null, 0);
        }

        var l = view.Listing;
        return await CreateListingAsync(
            hostUserId, l.Role, l.MinRank, l.MaxRank, l.SlotsNeeded, l.Note, l.LobbyCode is not null, l.LobbyLocked, reuseLobby: l);
    }

    /// <summary>Someone came in through the custom game lobby: one seat less to fill (closes when none are left).</summary>
    public async Task<HostActionOutcome> MarkFilledViaLobbyAsync(int listingId, ulong hostUserId)
    {
        var outcome = await pre.MarkFilledViaLobbyAsync(listingId, hostUserId);
        if (outcome.Result == HostActionResult.Done)
        {
            live.PublishChanged();
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
        }

        return outcome;
    }

    /// <summary>
    /// The operator closes an account: it can no longer sign in, its seats go back to their hosts (who get new
    /// players through the next wave) and its own listings close.
    /// </summary>
    public async Task BanAsync(ulong userId)
    {
        await moderation.BanAsync(userId);
        var (left, closed) = await pre.RemoveBannedUserAsync(userId);
        live.PublishChanged();
        foreach (var listingId in left)
        {
            await waves.RunAsync(listingId);
            Background($"notify host of ban {listingId}", () => host.LeftAsync(listingId, userId));
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
        }

        foreach (var listingId in closed)
        {
            Background($"update announcement {listingId}", () => feed.UpdateAsync(listingId));
        }
    }

    private void Background(string what, Func<Task> work) => _ = Task.Run(async () =>
    {
        try
        {
            await work();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Background task failed: {What}", what);
        }
    });
}
