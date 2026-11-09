using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;

namespace PreBot.Core.Services;

public sealed record ReportView(
    int Id, int ListingId, ulong ReporterUserId, ulong? ReportedUserId, ReportReason Reason, string? Note, DateTime CreatedAtUtc);

/// <summary>
/// Keeping a small community usable: players report a listing or a player, the operator reviews reports
/// and can ban an account (it can no longer sign in or be notified).
/// </summary>
public sealed class ModerationService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time)
{
    private DateTime Now => time.GetUtcNow().UtcDateTime;

    public async Task ReportAsync(int listingId, ulong reporterUserId, ulong? reportedUserId, ReportReason reason, string? note, CancellationToken ct = default)
    {
        note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (note is { Length: > 300 })
        {
            note = note[..300];
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        db.ListingReports.Add(new ListingReport
        {
            ListingId = listingId,
            ReporterUserId = reporterUserId,
            ReportedUserId = reportedUserId,
            Reason = reason,
            Note = note,
            CreatedAtUtc = Now,
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ReportView>> GetOpenReportsAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.ListingReports.AsNoTracking()
            .Where(r => r.ResolvedAtUtc == null)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Take(100)
            .Select(r => new ReportView(r.Id, r.ListingId, r.ReporterUserId, r.ReportedUserId, r.Reason, r.Note, r.CreatedAtUtc))
            .ToListAsync(ct);
    }

    public async Task ResolveReportAsync(int reportId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        await db.ListingReports.Where(r => r.Id == reportId && r.ResolvedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.ResolvedAtUtc, now), ct);
    }

    /// <summary>Bans an account: it is taken out of the pool and "notify me", and can no longer sign in.</summary>
    public async Task BanAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var id = (long)userId;
        await db.Accounts.Where(a => a.Id == id && a.BannedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.BannedAtUtc, now), ct);
        await db.Availabilities.Where(a => a.UserId == userId).ExecuteDeleteAsync(ct);
        await db.AlertSubscriptions.Where(a => a.UserId == userId).ExecuteDeleteAsync(ct);
    }

    public async Task UnbanAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var id = (long)userId;
        await db.Accounts.Where(a => a.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.BannedAtUtc, (DateTime?)null), ct);
    }

    public async Task<bool> IsBannedAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var id = (long)userId;
        return await db.Accounts.AnyAsync(a => a.Id == id && a.BannedAtUtc != null, ct);
    }

    /// <summary>Most recent accounts (for the admin page), banned ones included.</summary>
    public async Task<IReadOnlyList<UserAccount>> GetRecentAccountsAsync(int count = 50, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Accounts.AsNoTracking().OrderByDescending(a => a.CreatedAtUtc).Take(count).ToListAsync(ct);
    }

    /// <summary>How many times each player was reported as "did not show up" by a host (seat released as no-show).</summary>
    public async Task<IReadOnlyDictionary<ulong, int>> GetNoShowCountsAsync(IEnumerable<ulong> userIds, CancellationToken ct = default)
    {
        var ids = userIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new Dictionary<ulong, int>();
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.ListingSlots.AsNoTracking()
            .Where(s => ids.Contains(s.UserId) && s.ReleaseReason == SeatReleaseReason.NoShow)
            .GroupBy(s => s.UserId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, ct);
    }
}
