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

/// <summary>One way of reaching players, from notification to seat.</summary>
public sealed record SourceFunnel(
    NotificationSource Source, int Sent, int Opened, int InGame, int Claimed,
    double? MedianSecondsToOpen, double? MedianSecondsToClaim);

/// <summary>
/// Trial measurements for the operator: which way of reaching players fills seats, how fast people answer,
/// whether players in a match are lost, and how often hosts check or turn down a joiner.
/// </summary>
public sealed record TrialMetrics(
    IReadOnlyList<SourceFunnel> BySource,
    int ClaimsWithoutNotification,
    double? LateClaimShare,
    int Declines,
    int NoShows,
    int ProfileOpens,
    int PushAccepted);

/// <summary>
/// The numbers that decide whether PreBot solves the problem: how many "need 1 more" listings
/// actually got filled, and how fast. Computed from the anonymous <see cref="MetricEvent"/> log, so
/// history does not change when someone deletes their data.
/// </summary>
public sealed class StatsService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time)
{
    /// <summary>A join this long after the notification probably came from someone who was in a match.</summary>
    public static readonly TimeSpan LateClaim = TimeSpan.FromMinutes(8);

    public async Task<TrialMetrics> GetTrialAsync(ulong communityId, TimeSpan window, CancellationToken ct = default)
    {
        var since = time.GetUtcNow().UtcDateTime - window;
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var kinds = new[]
        {
            MetricKind.NotificationsSent, MetricKind.NotificationOpened, MetricKind.InGameReported, MetricKind.SeatClaimed,
            MetricKind.SeatReleased, MetricKind.ProfileOpened, MetricKind.PushAccepted,
        };
        var events = await db.MetricEvents.AsNoTracking()
            .Where(e => e.CommunityId == communityId && e.AtUtc >= since && kinds.Contains(e.Kind))
            .ToListAsync(ct);

        var bySource = Enum.GetValues<NotificationSource>().Select(source =>
        {
            var mine = events.Where(e => e.Source == source).ToList();
            var opened = mine.Where(e => e.Kind == MetricKind.NotificationOpened).ToList();
            var claimed = mine.Where(e => e.Kind == MetricKind.SeatClaimed).ToList();
            return new SourceFunnel(
                source,
                mine.Where(e => e.Kind == MetricKind.NotificationsSent).Sum(e => e.Value ?? 0),
                opened.Count,
                mine.Count(e => e.Kind == MetricKind.InGameReported),
                claimed.Count,
                Median(opened.Select(e => e.SinceNotifiedSeconds)),
                Median(claimed.Select(e => e.SinceNotifiedSeconds)));
        }).ToList();

        var notifiedClaims = events.Where(e => e.Kind == MetricKind.SeatClaimed && e.SinceNotifiedSeconds is not null).ToList();
        return new TrialMetrics(
            bySource,
            events.Count(e => e.Kind == MetricKind.SeatClaimed && e.Source is null),
            notifiedClaims.Count == 0 ? null : (double)notifiedClaims.Count(e => e.SinceNotifiedSeconds > LateClaim.TotalSeconds) / notifiedClaims.Count,
            events.Count(e => e.Kind == MetricKind.SeatReleased && e.Reason == SeatReleaseReason.HostDeclined),
            events.Count(e => e.Kind == MetricKind.SeatReleased && e.Reason == SeatReleaseReason.NoShow),
            events.Count(e => e.Kind == MetricKind.ProfileOpened),
            events.Where(e => e.Kind == MetricKind.PushAccepted).Sum(e => e.Value ?? 0));
    }

    private static double? Median(IEnumerable<int?> values)
    {
        var sorted = values.Where(v => v is not null).Select(v => (double)v!.Value).Order().ToList();
        if (sorted.Count == 0)
        {
            return null;
        }

        var mid = sorted.Count / 2;
        return sorted.Count % 2 == 1 ? sorted[mid] : (sorted[mid - 1] + sorted[mid]) / 2;
    }

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
            var firstFill = listing.Where(e => e.Kind is MetricKind.ListingFilled or MetricKind.FilledViaLobby)
                .Select(e => e.Value ?? 0).DefaultIfEmpty(-1).Min();
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
