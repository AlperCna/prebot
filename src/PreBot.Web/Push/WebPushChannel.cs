using PreBot.Core.Domain;
using PreBot.Core.Notifications;

namespace PreBot.Web.Push;

/// <summary>Delivers a notification wave as a system notification on the players' phones and PCs.</summary>
public sealed class WebPushChannel(PushSender sender) : INotificationChannel
{
    public async Task SendAsync(WaveAlert alert, CancellationToken ct)
    {
        if (alert.CommunityId != Communities.Web)
        {
            return;
        }

        await sender.SendAsync(
            alert.UserIds,
            PushSender.ForListing(alert.ListingId, alert.Role, alert.MinRank, alert.MaxRank, alert.SlotsMissing),
            timeToLive: TimeSpan.FromSeconds(120),
            topic: $"listing{alert.ListingId}",
            ct);
    }
}
