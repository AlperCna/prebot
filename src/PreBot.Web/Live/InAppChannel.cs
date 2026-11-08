using PreBot.Core.Domain;
using PreBot.Core.Notifications;

namespace PreBot.Web.Live;

/// <summary>Shows a wave as an alert on the open pages of the picked players.</summary>
public sealed class InAppChannel(LiveUpdates live) : INotificationChannel
{
    public Task SendAsync(WaveAlert alert, CancellationToken ct)
    {
        if (alert.CommunityId == Communities.Web)
        {
            live.PublishAlert(alert);
        }

        return Task.CompletedTask;
    }
}
