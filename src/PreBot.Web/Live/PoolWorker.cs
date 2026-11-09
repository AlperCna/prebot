using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;
using PreBot.Web.Push;

namespace PreBot.Web.Live;

/// <summary>
/// Every few seconds: drops expired "available" entries, closes expired listings (and tells their hosts),
/// sends due notification waves, and tells open pages to refresh if anything changed.
/// Correctness never depends on this timer — queries also filter on expiry times.
/// </summary>
public sealed class PoolWorker(
    PreService pre,
    WaveDispatcher waves,
    LiveUpdates live,
    HostNotifier host,
    IListingFeed feed,
    PreBotOptions options,
    ILogger<PoolWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Clamp(options.SweepIntervalSeconds, 5, 60)));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var sweep = await pre.SweepAsync(onlyCommunity: Communities.Web, ct: stoppingToken);
                var changed = sweep.ExpiredAvailabilities > 0 || sweep.ExpiredListingIds.Count > 0;

                foreach (var listingId in sweep.ExpiredListingIds)
                {
                    // Both check the community themselves: Discord-server listings are the bot's business.
                    await Safe(() => host.ExpiredAsync(listingId, stoppingToken), "expired push", listingId);
                    await Safe(() => feed.UpdateAsync(listingId, stoppingToken), "announcement update", listingId);
                }

                foreach (var listingId in await pre.GetDueWaveListingIdsAsync(onlyCommunity: Communities.Web, ct: stoppingToken))
                {
                    await waves.RunAsync(listingId, stoppingToken);
                }

                // "Maçtayım": their match should be over, offer the listing again (or another one that fits).
                foreach (var (userId, putOff) in await pre.TakeFinishedMatchesAsync(stoppingToken))
                {
                    if (await pre.MatchAfterMatchAsync(Communities.Web, userId, putOff, stoppingToken) is { } wave)
                    {
                        await waves.DispatchAsync(wave, stoppingToken);
                    }

                    changed = true;
                }

                if (changed)
                {
                    live.PublishChanged();
                }

                if (DateTime.UtcNow - _lastPurge > TimeSpan.FromHours(1))
                {
                    _lastPurge = DateTime.UtcNow;
                    var purged = await pre.PurgeOldListingsAsync(stoppingToken);
                    if (purged > 0)
                    {
                        logger.LogInformation("Deleted {Count} closed listing(s) older than 90 days", purged);
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Pool sweep failed");
            }
        }
    }

    private DateTime _lastPurge = DateTime.MinValue;

    private async Task Safe(Func<Task> work, string what, int listingId)
    {
        try
        {
            await work();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "{What} failed for listing {ListingId}", what, listingId);
        }
    }
}
