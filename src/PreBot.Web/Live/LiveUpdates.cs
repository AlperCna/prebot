using PreBot.Core.Notifications;

namespace PreBot.Web.Live;

/// <summary>
/// In-process event hub. Every open page (Blazor circuit) subscribes; when the pool or a listing
/// changes, pages reload their data. Single server, single process: no Redis or Azure SignalR needed.
/// </summary>
public sealed class LiveUpdates
{
    /// <summary>Something in the pool or the listings changed.</summary>
    public event Action? Changed;

    /// <summary>A notification wave picked some players for a listing.</summary>
    public event Action<WaveAlert>? Alerted;

    public void PublishChanged() => Changed?.Invoke();

    public void PublishAlert(WaveAlert alert) => Alerted?.Invoke(alert);
}
