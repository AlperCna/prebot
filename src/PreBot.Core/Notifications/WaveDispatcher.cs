using Microsoft.Extensions.Logging;
using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Core.Notifications;

/// <summary>"These players fit an open listing — tell them." One alert per notification wave.</summary>
public sealed record WaveAlert(
    int ListingId,
    int Wave,
    ulong CommunityId,
    ulong HostUserId,
    Role Role,
    Rank MinRank,
    Rank MaxRank,
    int SlotsMissing,
    IReadOnlyList<ulong> UserIds,
    string? LobbyHint = null);

/// <summary>
/// A way to reach players: an in-page alert, a phone push notification, a Discord mention...
/// Matching never knows how an alert is delivered; it only hands a <see cref="WaveAlert"/> to every channel.
/// </summary>
public interface INotificationChannel
{
    Task SendAsync(WaveAlert alert, CancellationToken ct);
}

/// <summary>Runs the next due wave of a listing and hands the result to every notification channel.</summary>
public sealed class WaveDispatcher(PreService pre, IEnumerable<INotificationChannel> channels, ILogger<WaveDispatcher> logger)
{
    public async Task<WaveAlert?> RunAsync(int listingId, CancellationToken ct = default) =>
        await pre.RunNextWaveAsync(listingId, ct) is { } wave ? await DispatchAsync(wave, ct) : null;

    /// <summary>Hands an already chosen set of players (a wave, or one newcomer) to every channel.</summary>
    public async Task<WaveAlert?> DispatchAsync(WaveResult wave, CancellationToken ct = default)
    {
        var listingId = wave.ListingId;
        if (wave.UserIds.Count == 0)
        {
            return null;
        }

        var view = await pre.GetListingViewAsync(listingId, ct);
        if (view is null)
        {
            return null;
        }

        var l = view.Listing;
        var alert = new WaveAlert(
            l.Id, wave.Wave, l.GuildId, l.HostUserId, l.Role, l.MinRank, l.MaxRank,
            l.SlotsNeeded - l.SlotsFilled, wave.UserIds,
            l.LobbyCode is { } code ? Lobby.Hint(l.Role, code, l.LobbyLocked) : null);

        foreach (var channel in channels)
        {
            try
            {
                await channel.SendAsync(alert, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // One broken channel (e.g. push service down) must not stop the others.
                logger.LogWarning(ex, "Notification channel {Channel} failed for listing {ListingId}", channel.GetType().Name, listingId);
            }
        }

        return alert;
    }
}
