using Discord;
using Discord.WebSocket;
using PreBot.Core.Services;

namespace PreBot.Bot;

/// <summary>
/// Every few seconds: drops expired "available" entries, closes expired listings,
/// and sends notification waves that have become due.
/// Correctness never depends on this timer — queries also filter on expiry times.
/// </summary>
public sealed class ExpiryWorker(
    DiscordSocketClient client,
    PreService pre,
    ListingNotifier notifier,
    PreBotOptions options,
    ILogger<ExpiryWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(5, options.SweepIntervalSeconds)));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            if (client.ConnectionState != ConnectionState.Connected)
            {
                continue;
            }

            try
            {
                var sweep = await pre.SweepAsync(stoppingToken);
                foreach (var listingId in sweep.ExpiredListingIds)
                {
                    await notifier.RefreshAsync(listingId, stoppingToken);
                }

                foreach (var listingId in await pre.GetDueWaveListingIdsAsync(stoppingToken))
                {
                    await notifier.RunWaveAsync(listingId, stoppingToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Sweep failed");
            }
        }
    }
}
