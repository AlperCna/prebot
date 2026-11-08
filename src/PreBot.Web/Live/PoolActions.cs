using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;

namespace PreBot.Web.Live;

/// <summary>
/// What the website's buttons do. Each action changes the data through <see cref="PreService"/>,
/// sends notification waves where needed, and tells every open page to refresh.
/// </summary>
public sealed class PoolActions(PreService pre, WaveDispatcher waves, LiveUpdates live)
{
    private const ulong Web = Communities.Web;

    public async Task<Availability> SetAvailableAsync(ulong userId, Role role, Rank rank, TimeSpan duration)
    {
        var entry = await pre.SetAvailableAsync(Web, userId, role, rank, duration);
        live.PublishChanged();
        return entry;
    }

    public async Task ClearAvailableAsync(ulong userId)
    {
        await pre.ClearAvailableAsync(Web, userId);
        live.PublishChanged();
    }

    public async Task<CreateListingResult> CreateListingAsync(ulong hostUserId, Role role, Rank min, Rank max, int slots, string? note)
    {
        var result = await pre.CreateListingAsync(Web, Web, hostUserId, role, min, max, slots, note);
        if (result.Listing is not null)
        {
            live.PublishChanged();
            await waves.RunAsync(result.Listing.Id);
        }

        return result;
    }

    public async Task<ClaimOutcome> ClaimAsync(int listingId, ulong userId)
    {
        var outcome = await pre.TryClaimAsync(listingId, userId);
        if (outcome.Result is ClaimResult.Claimed or ClaimResult.ClaimedAndFilled)
        {
            live.PublishChanged();
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
        }

        return result;
    }

    public async Task<HostActionOutcome> ReportNoShowAsync(int listingId, ulong hostUserId)
    {
        var outcome = await pre.ReopenForNoShowAsync(listingId, hostUserId);
        if (outcome.Result == HostActionResult.Done)
        {
            live.PublishChanged();
            await waves.RunAsync(listingId);
        }

        return outcome;
    }

    public async Task<HostActionOutcome> CancelAsync(int listingId, ulong hostUserId)
    {
        var outcome = await pre.CancelAsync(listingId, hostUserId);
        if (outcome.Result == HostActionResult.Done)
        {
            live.PublishChanged();
        }

        return outcome;
    }
}
