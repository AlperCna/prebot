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
    int AvailableNow,
    double? P90MinutesToFill = null,
    int SeatsTaken = 0,
    int NoShows = 0);

/// <summary>
/// The numbers that decide whether PreBot solves the problem: how many "need 1 more" listings
/// actually got filled, and how fast. Computed from the anonymous <see cref="MetricEvent"/> log, so
/// history does not change when someone deletes their data.
/// </summary>
public sealed class StatsService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time)
{
    public async Task<StatsSummary> GetAsync(ulong communityId, TimeSpan window, CancellationToken ct = default)
    {
        var now = time.GetUtcNow().UtcDateTime;
        var since = now - window;
        await using var db = await dbFactory.CreateDbContextAsync(ct);

        var openedIds = await db.MetricEvents.AsNoTracking()
            .Where(e => e.CommunityId == communityId && e.Kind == MetricKind.ListingOpened && e.AtUtc >= since)
            .Select(e => e.ListingId)
            .ToListAsync(ct);

        var events = await db.MetricEvents.AsNoTracking()
            .Where(e => openedIds.Contains(e.ListingId))
            .Select(e => new { e.ListingId, e.Kind, e.Value, e.Reason })
            .ToListAsync(ct);
        var byListing = events.ToLookup(e => e.ListingId);

        var filled = new List<double>();
        var finished = 0;
        foreach (var id in openedIds)
        {
            var listing = byListing[id].ToList();
            // A listing that filled once counts as filled, even if the host closed it afterwards
            // (they were playing) or a no-show reopened it.
            var firstFill = listing.Where(e => e.Kind == MetricKind.ListingFilled).Select(e => e.Value ?? 0).DefaultIfEmpty(-1).Min();
            if (firstFill >= 0)
            {
                filled.Add(firstFill / 60.0);
                finished++;
            }
            else if (listing.Any(e => e.Kind is MetricKind.ListingExpired or MetricKind.ListingCancelled))
            {
                finished++;
            }
        }

        filled.Sort();
        var notifications = events.Where(e => e.Kind == MetricKind.WaveSent).Sum(e => e.Value ?? 0);
        var seatsTaken = events.Count(e => e.Kind == MetricKind.SeatClaimed);
        var noShows = events.Count(e => e.Kind == MetricKind.SeatReleased && e.Reason == SeatReleaseReason.NoShow);

        // Who took part is only known from the live tables (the event log stores no user ids).
        var players = await db.Listings.AsNoTracking()
            .Where(l => openedIds.Contains(l.Id))
            .Select(l => l.HostUserId)
            .Concat(db.ListingSlots.Where(s => openedIds.Contains(s.ListingId)).Select(s => s.UserId))
            .Distinct()
            .CountAsync(ct);
        var availableNow = await db.Availabilities.CountAsync(a => a.GuildId == communityId && a.ExpiresAtUtc > now, ct);

        return new StatsSummary(
            openedIds.Count,
            finished,
            filled.Count,
            finished == 0 ? null : (double)filled.Count / finished,
            Percentile(filled, 0.5),
            filled.Count == 0 ? null : filled[0],
            notifications,
            players,
            availableNow,
            Percentile(filled, 0.9),
            seatsTaken,
            noShows);
    }

    /// <summary>Nearest-rank percentile, with the median averaging the two middle values.</summary>
    private static double? Percentile(IReadOnlyList<double> sorted, double p)
    {
        if (sorted.Count == 0)
        {
            return null;
        }

        if (p == 0.5)
        {
            var n = sorted.Count;
            return n % 2 == 1 ? sorted[n / 2] : (sorted[(n / 2) - 1] + sorted[n / 2]) / 2;
        }

        var rank = (int)Math.Ceiling(p * sorted.Count);
        return sorted[Math.Clamp(rank - 1, 0, sorted.Count - 1)];
    }
}
