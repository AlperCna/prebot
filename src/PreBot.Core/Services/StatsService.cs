using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;

namespace PreBot.Core.Services;

/// <summary>Anonymous totals that answer "does it work?": no names, no ids.</summary>
public sealed record StatsSummary(
    int ListingsOpened,
    int ListingsFinished,
    int ListingsFilled,
    double? FillRate,
    double? MedianMinutesToFill,
    double? FastestMinutesToFill,
    int NotificationsSent,
    int ActivePlayers,
    int AvailableNow);

/// <summary>
/// The numbers that decide whether PreBot solves the problem: how many "need 1 more" listings
/// actually got filled, and how fast.
/// </summary>
public sealed class StatsService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time)
{
    public async Task<StatsSummary> GetAsync(ulong communityId, TimeSpan window, CancellationToken ct = default)
    {
        var now = time.GetUtcNow().UtcDateTime;
        var since = now - window;
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var listings = await db.Listings.AsNoTracking()
            .Where(l => l.GuildId == communityId && l.CreatedAtUtc >= since)
            .Select(l => new
            {
                l.Id,
                l.HostUserId,
                l.Status,
                l.CreatedAtUtc,
                Seats = l.Slots.Select(s => new { s.UserId, s.ClaimedAtUtc, s.Released }).ToList(),
            })
            .ToListAsync(ct);

        // "Finished" = no longer waiting: filled, expired or closed. Open listings are still in play.
        var finished = listings.Where(l => l.Status != ListingStatus.Open).ToList();
        var filled = finished.Where(l => l.Status == ListingStatus.Filled).ToList();

        var minutesToFill = filled
            .Select(l => l.Seats.Where(s => !s.Released).Select(s => s.ClaimedAtUtc).DefaultIfEmpty(l.CreatedAtUtc).Max())
            .Zip(filled, (lastSeat, l) => (lastSeat - l.CreatedAtUtc).TotalMinutes)
            .Order()
            .ToList();

        var listingIds = listings.Select(l => l.Id).ToList();
        var notifications = await db.ListingNotifications.CountAsync(n => listingIds.Contains(n.ListingId), ct);
        var players = listings.Select(l => l.HostUserId)
            .Concat(listings.SelectMany(l => l.Seats.Select(s => s.UserId)))
            .Distinct()
            .Count();
        var availableNow = await db.Availabilities.CountAsync(a => a.GuildId == communityId && a.ExpiresAtUtc > now, ct);

        return new StatsSummary(
            listings.Count,
            finished.Count,
            filled.Count,
            finished.Count == 0 ? null : (double)filled.Count / finished.Count,
            Median(minutesToFill),
            minutesToFill.Count == 0 ? null : minutesToFill[0],
            notifications,
            players,
            availableNow);
    }

    private static double? Median(IReadOnlyList<double> sorted) => sorted.Count switch
    {
        0 => null,
        var n when n % 2 == 1 => sorted[n / 2],
        var n => (sorted[(n / 2) - 1] + sorted[n / 2]) / 2,
    };
}
