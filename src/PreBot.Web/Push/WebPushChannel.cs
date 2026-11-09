using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;

namespace PreBot.Web.Push;

/// <summary>Delivers a notification wave as a system notification on the players' phones and PCs.</summary>
public sealed class WebPushChannel(PushSender sender, JoinTokens tokens, PreService pre, TimeProvider time) : INotificationChannel
{
    public async Task SendAsync(WaveAlert alert, CancellationToken ct)
    {
        if (alert.CommunityId != Communities.Web)
        {
            return;
        }

        var accepted = await sender.SendAsync(
            alert.UserIds,
            userId => PushSender.ForListing(
                alert.ListingId, alert.Role, alert.MinRank, alert.MaxRank, alert.SlotsMissing,
                tokens.Create(alert.ListingId, userId), alert.LobbyHint, tokens.CreateMute(userId)),
            // Kept by the push service while the listing is open (a phone offline for a few minutes still gets
            // it), at most 10 minutes; if it fills first, "Doldu" with the same topic replaces it unseen.
            timeToLive: TimeToLive(alert),
            topic: $"listing{alert.ListingId}",
            ct);

        // Delivered to the push service, not yet seen: compare with "opened" in the trial measurements.
        if (accepted > 0)
        {
            await pre.RecordAsync(MetricKind.PushAccepted, alert.ListingId, alert.Wave, accepted, ct);
        }
    }

    private TimeSpan TimeToLive(WaveAlert alert)
    {
        var left = alert.ExpiresAtUtc is { } end ? end - time.GetUtcNow().UtcDateTime : TimeSpan.FromMinutes(2);
        return TimeSpan.FromSeconds(Math.Clamp(left.TotalSeconds, 60, 600));
    }
}
