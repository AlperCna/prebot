using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;

namespace PreBot.Core.Services;

/// <summary>Push-enabled devices and "Bana haber ver" alerts.</summary>
public sealed class SubscriptionService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time)
{
    private DateTime Now => time.GetUtcNow().UtcDateTime;

    // ---------- Push devices ----------

    /// <summary>
    /// Saves a browser's push subscription for a user. If the same browser was registered by another
    /// account before (shared PC, re-login), it now belongs to this user.
    /// </summary>
    public async Task SavePushSubscriptionAsync(ulong userId, string endpoint, string p256dh, string auth, string? device, CancellationToken ct = default)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("Push endpoint must be an https URL.", nameof(endpoint));
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var existing = await db.PushSubscriptions.SingleOrDefaultAsync(p => p.Endpoint == endpoint, ct);
        if (existing is null)
        {
            existing = new WebPushSubscription { Endpoint = endpoint, CreatedAtUtc = Now };
            db.PushSubscriptions.Add(existing);
        }

        existing.UserId = userId;
        existing.P256dh = p256dh;
        existing.Auth = auth;
        existing.Device = device is { Length: > 64 } ? device[..64] : device;
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<WebPushSubscription>> GetPushSubscriptionsAsync(IEnumerable<ulong> userIds, CancellationToken ct = default)
    {
        var ids = userIds.Distinct().ToList();
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.PushSubscriptions.AsNoTracking().Where(p => ids.Contains(p.UserId)).ToListAsync(ct);
    }

    public async Task<int> CountPushDevicesAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.PushSubscriptions.CountAsync(p => p.UserId == userId, ct);
    }

    /// <summary>The push service says this subscription is gone (404/410), or the user turned it off.</summary>
    public async Task RemovePushSubscriptionAsync(string endpoint, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        await db.PushSubscriptions.Where(p => p.Endpoint == endpoint).ExecuteDeleteAsync(ct);
    }

    public async Task RemoveAllPushSubscriptionsAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        await db.PushSubscriptions.Where(p => p.UserId == userId).ExecuteDeleteAsync(ct);
    }

    public async Task MarkPushDeliveredAsync(IEnumerable<int> subscriptionIds, CancellationToken ct = default)
    {
        var ids = subscriptionIds.ToList();
        if (ids.Count == 0)
        {
            return;
        }

        var now = Now;
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        await db.PushSubscriptions.Where(p => ids.Contains(p.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.LastSuccessAtUtc, now), ct);
    }

    // ---------- "Bana haber ver" ----------

    public async Task<AlertSubscription?> GetAlertAsync(ulong communityId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.AlertSubscriptions.AsNoTracking()
            .SingleOrDefaultAsync(a => a.CommunityId == communityId && a.UserId == userId, ct);
    }

    public async Task<AlertSubscription> SaveAlertAsync(
        ulong communityId, ulong userId, Role role, Rank rank, int fromHour, int toHour, bool enabled, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var alert = await db.AlertSubscriptions.SingleOrDefaultAsync(a => a.CommunityId == communityId && a.UserId == userId, ct);
        if (alert is null)
        {
            alert = new AlertSubscription { CommunityId = communityId, UserId = userId, CreatedAtUtc = Now };
            db.AlertSubscriptions.Add(alert);
        }

        alert.Role = role;
        alert.Rank = rank;
        alert.FromHour = Math.Clamp(fromHour, 0, 23);
        alert.ToHour = Math.Clamp(toHour, 0, 23);
        alert.Enabled = enabled;
        await db.SaveChangesAsync(ct);
        return alert;
    }
}
