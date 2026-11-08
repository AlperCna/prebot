using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;

namespace PreBot.Core.Services;

/// <summary>Remembers where website listings were announced (Discord channel + message).</summary>
public sealed class AnnouncementStore(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time)
{
    public async Task SaveAsync(int listingId, ulong channelId, ulong messageId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        db.ListingAnnouncements.Add(new ListingAnnouncement
        {
            ListingId = listingId,
            ChannelId = channelId,
            MessageId = messageId,
            PostedAtUtc = time.GetUtcNow().UtcDateTime,
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<ListingAnnouncement>> GetAsync(int listingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.ListingAnnouncements.AsNoTracking().Where(a => a.ListingId == listingId).ToListAsync(ct);
    }
}
