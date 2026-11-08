using PreBot.Core.Notifications;
using PreBot.Core.Services;

namespace PreBot.Web.Live;

/// <summary>
/// Every few seconds: drops expired "available" entries, closes expired listings, sends due
/// notification waves, and tells open pages to refresh if anything changed.
/// Correctness never depends on this timer — queries also filter on expiry times.
/// </summary>
public sealed class PoolWorker(
    PreService pre,
    WaveDispatcher waves,
    LiveUpdates live,
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
                var sweep = await pre.SweepAsync(stoppingToken);
                var changed = sweep.ExpiredAvailabilities > 0 || sweep.ExpiredListingIds.Count > 0;

                foreach (var listingId in await pre.GetDueWaveListingIdsAsync(stoppingToken))
                {
                    await waves.RunAsync(listingId, stoppingToken);
                }

                if (changed)
                {
                    live.PublishChanged();
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Pool sweep failed");
            }
        }
    }
}
