using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;
using PreBot.Core.Matching;

namespace PreBot.Core.Services;

public sealed record HostActionOutcome(HostActionResult Result, ulong? AffectedUserId = null);

public sealed record DeletedListingMessage(ulong ChannelId, ulong MessageId);

public sealed record DeleteUserDataResult(IReadOnlyList<int> LeftListingIds, IReadOnlyList<DeletedListingMessage> RemovedListings);

/// <summary>
/// All bot behaviour that touches the database. Discord-free so it can be unit tested.
/// Every state change that can race (two people clicking "Join" at once, a seat being given back
/// while someone else joins) is done with a single conditional UPDATE inside a transaction.
/// </summary>
public sealed class PreService(IDbContextFactory<PreBotDbContext> dbFactory, TimeProvider time, PreBotOptions options)
{
    private DateTime Now => time.GetUtcNow().UtcDateTime;

    private TimeSpan PingCooldown => TimeSpan.FromMinutes(options.PingCooldownMinutes);

    // ---------- Availability pool ----------

    public async Task<Availability> SetAvailableAsync(
        ulong guildId, ulong userId, Role role, Rank rank, TimeSpan duration, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        await EnsurePlayerAsync(db, userId, now, ct);

        var entry = await db.Availabilities.SingleOrDefaultAsync(a => a.GuildId == guildId && a.UserId == userId, ct);
        if (entry is null)
        {
            entry = new Availability { GuildId = guildId, UserId = userId, CreatedAtUtc = now };
            db.Availabilities.Add(entry);
        }

        // LastPingedAtUtc is kept on purpose: re-joining the pool must not reset the anti-spam cooldown.
        entry.Role = role;
        entry.Rank = rank;
        entry.ExpiresAtUtc = now + duration;
        await db.SaveChangesAsync(ct);
        return entry;
    }

    public async Task<bool> ClearAvailableAsync(ulong guildId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Availabilities
            .Where(a => a.GuildId == guildId && a.UserId == userId)
            .ExecuteDeleteAsync(ct) > 0;
    }

    public async Task<PoolSummary> GetPoolSummaryAsync(ulong guildId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var roles = await db.Availabilities.AsNoTracking()
            .Where(a => a.GuildId == guildId && a.ExpiresAtUtc > now)
            .Select(a => a.Role)
            .ToListAsync(ct);

        var byRole = Enum.GetValues<Role>().ToDictionary(r => r, r => roles.Count(x => x == r));
        return new PoolSummary(roles.Count, byRole);
    }

    // ---------- Players ----------

    public async Task<bool> SetRiotIdAsync(ulong userId, string input, CancellationToken ct = default)
    {
        if (!RiotIdValidator.TryNormalize(input, out var riotId))
        {
            return false;
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var player = await EnsurePlayerAsync(db, userId, Now, ct);
        player.RiotId = riotId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<string?> GetRiotIdAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Players.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => p.RiotId)
            .SingleOrDefaultAsync(ct);
    }

    // ---------- Listings ----------

    public async Task<CreateListingResult> CreateListingAsync(
        ulong guildId,
        ulong channelId,
        ulong hostUserId,
        Role role,
        Rank minRank,
        Rank maxRank,
        int slotsNeeded,
        string? note,
        CancellationToken ct = default)
    {
        if (minRank > maxRank)
        {
            (minRank, maxRank) = (maxRank, minRank);
        }

        slotsNeeded = Math.Clamp(slotsNeeded, 1, options.MaxSlotsPerListing);
        note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        if (note is { Length: > 200 })
        {
            note = note[..200];
        }

        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;

        var existingId = await db.Listings
            .Where(l => l.GuildId == guildId && l.HostUserId == hostUserId
                && l.Status == ListingStatus.Open && l.ExpiresAtUtc > now)
            .Select(l => (int?)l.Id)
            .FirstOrDefaultAsync(ct);
        if (existingId is not null)
        {
            return new CreateListingResult(null, existingId, 0);
        }

        await EnsurePlayerAsync(db, hostUserId, now, ct);

        // The host is organizing, not waiting to be picked.
        await db.Availabilities.Where(a => a.GuildId == guildId && a.UserId == hostUserId).ExecuteDeleteAsync(ct);

        var listing = new Listing
        {
            GuildId = guildId,
            ChannelId = channelId,
            HostUserId = hostUserId,
            Role = role,
            MinRank = minRank,
            MaxRank = maxRank,
            SlotsNeeded = slotsNeeded,
            Status = ListingStatus.Open,
            Note = note,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(options.ListingMinutes),
            NextWaveAtUtc = now,
        };
        db.Listings.Add(listing);
        await db.SaveChangesAsync(ct);
        Record(db, MetricKind.ListingOpened, listing, now, value: slotsNeeded);
        await db.SaveChangesAsync(ct);

        var pool = await db.Availabilities.AsNoTracking()
            .Where(a => a.GuildId == guildId && a.ExpiresAtUtc > now)
            .ToListAsync(ct);
        var potential = Matcher.CountPotentialMatches(pool, Criteria(listing), now);

        return new CreateListingResult(listing, null, potential);
    }

    public async Task AttachMessageAsync(int listingId, ulong messageId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        await db.Listings.Where(l => l.Id == listingId)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.MessageId, messageId), ct);
    }

    public async Task<ListingView?> GetListingViewAsync(int listingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var listing = await db.Listings.AsNoTracking()
            .Include(l => l.Slots)
            .SingleOrDefaultAsync(l => l.Id == listingId, ct);
        if (listing is null)
        {
            return null;
        }

        var active = listing.Slots.Where(s => !s.Released).OrderBy(s => s.ClaimedAtUtc).ThenBy(s => s.Id).ToList();
        var userIds = active.Select(s => s.UserId).Append(listing.HostUserId).Distinct().ToList();
        var riotIds = await db.Players.AsNoTracking()
            .Where(p => userIds.Contains(p.UserId))
            .ToDictionaryAsync(p => p.UserId, p => p.RiotId, ct);

        return new ListingView(
            listing,
            riotIds.GetValueOrDefault(listing.HostUserId),
            active.Select(s => new SlotView(s.UserId, riotIds.GetValueOrDefault(s.UserId))).ToList(),
            listing.Slots.Where(s => s.Released).Select(s => s.UserId).ToHashSet());
    }

    /// <summary>Open, not yet expired listings of a community, newest first.</summary>
    public async Task<IReadOnlyList<ListingView>> GetOpenListingsAsync(ulong communityId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var ids = await db.Listings.AsNoTracking()
            .Where(l => l.GuildId == communityId && l.Status == ListingStatus.Open && l.ExpiresAtUtc > now)
            .OrderByDescending(l => l.CreatedAtUtc)
            .Select(l => l.Id)
            .Take(50)
            .ToListAsync(ct);
        return await GetViewsAsync(ids, ct);
    }

    /// <summary>
    /// Listings a user hosts or holds a seat in that are still relevant (open, or filled recently),
    /// so they can find the Riot IDs they need to invite each other.
    /// </summary>
    public async Task<IReadOnlyList<ListingView>> GetMyListingsAsync(ulong communityId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var since = Now.AddHours(-3);
        var ids = await db.Listings.AsNoTracking()
            .Where(l => l.GuildId == communityId
                && (l.Status == ListingStatus.Open || l.Status == ListingStatus.Filled)
                && l.CreatedAtUtc > since
                && (l.HostUserId == userId || l.Slots.Any(s => s.UserId == userId && !s.Released)))
            .OrderByDescending(l => l.CreatedAtUtc)
            .Select(l => l.Id)
            .Take(10)
            .ToListAsync(ct);
        return await GetViewsAsync(ids, ct);
    }

    public async Task<Availability?> GetAvailabilityAsync(ulong communityId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        return await db.Availabilities.AsNoTracking()
            .SingleOrDefaultAsync(a => a.GuildId == communityId && a.UserId == userId && a.ExpiresAtUtc > now, ct);
    }

    private async Task<IReadOnlyList<ListingView>> GetViewsAsync(IEnumerable<int> listingIds, CancellationToken ct)
    {
        var views = new List<ListingView>();
        foreach (var id in listingIds)
        {
            if (await GetListingViewAsync(id, ct) is { } view)
            {
                views.Add(view);
            }
        }

        return views;
    }

    /// <summary>Takes a seat. Safe when many people click at the same moment: exactly one gets the last seat.</summary>
    public async Task<ClaimOutcome> TryClaimAsync(int listingId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // Write first so SQLite takes the write lock right away. The WHERE clause is the whole rule set.
        var updated = await db.Listings
            .Where(l => l.Id == listingId
                && l.Status == ListingStatus.Open
                && l.ExpiresAtUtc > now
                && l.SlotsFilled < l.SlotsNeeded
                && l.HostUserId != userId
                && !db.ListingSlots.Any(s => s.ListingId == listingId && s.UserId == userId))
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.SlotsFilled, l => l.SlotsFilled + 1), ct);

        if (updated == 0)
        {
            await tx.RollbackAsync(ct);
            return await DiagnoseFailedClaimAsync(db, listingId, userId, now, ct);
        }

        db.ListingSlots.Add(new ListingSlot { ListingId = listingId, UserId = userId, ClaimedAtUtc = now });
        await EnsurePlayerAsync(db, userId, now, ct);
        await db.SaveChangesAsync(ct);

        var filled = await db.Listings
            .Where(l => l.Id == listingId && l.SlotsFilled >= l.SlotsNeeded)
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Status, ListingStatus.Filled)
                .SetProperty(l => l.NextWaveAtUtc, (DateTime?)null), ct);

        var info = await db.Listings.Where(l => l.Id == listingId)
            .Select(l => new { l.GuildId, l.HostUserId, l.CreatedAtUtc })
            .SingleAsync(ct);

        // They are playing now: take them out of this server's pool.
        await db.Availabilities.Where(a => a.GuildId == info.GuildId && a.UserId == userId).ExecuteDeleteAsync(ct);

        var secondsOpen = (int)(now - info.CreatedAtUtc).TotalSeconds;
        Record(db, MetricKind.SeatClaimed, info.GuildId, listingId, now, value: secondsOpen);
        if (filled > 0)
        {
            Record(db, MetricKind.ListingFilled, info.GuildId, listingId, now, value: secondsOpen);
        }

        await db.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
        return new ClaimOutcome(filled > 0 ? ClaimResult.ClaimedAndFilled : ClaimResult.Claimed, info.HostUserId);
    }

    /// <summary>A player gives their seat back. The listing reopens and notifies again.</summary>
    public Task<LeaveResult> LeaveAsync(int listingId, ulong userId, CancellationToken ct = default) =>
        LeaveAsync(listingId, userId, SeatReleaseReason.Voluntary, ct);

    private async Task<LeaveResult> LeaveAsync(int listingId, ulong userId, SeatReleaseReason reason, CancellationToken ct)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        if (await ReleaseSeatAsync(db, listingId, userId, reason, Now, ct))
        {
            await tx.CommitAsync(ct);
            return LeaveResult.Left;
        }

        await tx.RollbackAsync(ct);
        return await db.Listings.AnyAsync(l => l.Id == listingId, ct) ? LeaveResult.NotJoined : LeaveResult.NotFound;
    }

    /// <summary>Host says the last person who joined did not show up: free that seat and look again.</summary>
    public async Task<HostActionOutcome> ReopenForNoShowAsync(int listingId, ulong hostUserId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var host = await db.Listings.Where(l => l.Id == listingId).Select(l => (ulong?)l.HostUserId).SingleOrDefaultAsync(ct);
        if (host is null)
        {
            return new HostActionOutcome(HostActionResult.NotFound);
        }

        if (host != hostUserId)
        {
            return new HostActionOutcome(HostActionResult.NotHost);
        }

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var latest = await db.ListingSlots
            .Where(s => s.ListingId == listingId && !s.Released)
            .OrderByDescending(s => s.ClaimedAtUtc).ThenByDescending(s => s.Id)
            .Select(s => (ulong?)s.UserId)
            .FirstOrDefaultAsync(ct);

        if (latest is null || !await ReleaseSeatAsync(db, listingId, latest.Value, SeatReleaseReason.NoShow, Now, ct))
        {
            await tx.RollbackAsync(ct);
            return new HostActionOutcome(HostActionResult.NothingToDo);
        }

        await tx.CommitAsync(ct);
        return new HostActionOutcome(HostActionResult.Done, latest);
    }

    public async Task<HostActionOutcome> CancelAsync(int listingId, ulong hostUserId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var cancelled = await db.Listings
            .Where(l => l.Id == listingId && l.HostUserId == hostUserId
                && (l.Status == ListingStatus.Open || l.Status == ListingStatus.Filled))
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Status, ListingStatus.Cancelled)
                .SetProperty(l => l.NextWaveAtUtc, (DateTime?)null), ct);
        if (cancelled > 0)
        {
            var community = await db.Listings.Where(l => l.Id == listingId).Select(l => l.GuildId).SingleAsync(ct);
            Record(db, MetricKind.ListingCancelled, community, listingId, Now);
            await db.SaveChangesAsync(ct);
            return new HostActionOutcome(HostActionResult.Done);
        }

        var host = await db.Listings.Where(l => l.Id == listingId).Select(l => (ulong?)l.HostUserId).SingleOrDefaultAsync(ct);
        return new HostActionOutcome(host switch
        {
            null => HostActionResult.NotFound,
            _ when host != hostUserId => HostActionResult.NotHost,
            _ => HostActionResult.NothingToDo,
        });
    }

    // ---------- Notification waves ----------

    /// <summary>
    /// Sends the next wave if it is due. Returns who to notify, or null if nothing is due.
    /// The wave counter is advanced with a conditional UPDATE, so the command handler and the
    /// background worker can never send the same wave twice.
    /// </summary>
    public async Task<WaveResult?> RunNextWaveAsync(int listingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var listing = await db.Listings.AsNoTracking().SingleOrDefaultAsync(l => l.Id == listingId, ct);
        if (listing is null
            || listing.Status != ListingStatus.Open
            || listing.ExpiresAtUtc <= now
            || listing.WavesSent >= Matcher.MaxWaves
            || listing.NextWaveAtUtc is null
            || listing.NextWaveAtUtc > now)
        {
            return null;
        }

        var wave = listing.WavesSent;
        DateTime? nextWaveAt = wave + 1 < Matcher.MaxWaves ? now + Matcher.DelayAfterWave(wave) : null;

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        var advanced = await db.Listings
            .Where(l => l.Id == listingId && l.WavesSent == wave && l.Status == ListingStatus.Open)
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.WavesSent, wave + 1)
                .SetProperty(l => l.NextWaveAtUtc, nextWaveAt), ct);
        if (advanced == 0)
        {
            await tx.RollbackAsync(ct);
            return null;
        }

        var pool = await db.Availabilities
            .Where(a => a.GuildId == listing.GuildId && a.ExpiresAtUtc > now)
            .ToListAsync(ct);
        // Skip anyone who already has (or had) a seat, or already heard about this listing.
        var excluded = (await db.ListingSlots
                .Where(s => s.ListingId == listingId)
                .Select(s => s.UserId)
                .ToListAsync(ct))
            .Concat(await db.ListingNotifications
                .Where(n => n.ListingId == listingId)
                .Select(n => n.UserId)
                .ToListAsync(ct))
            .ToHashSet();

        var criteria = Criteria(listing);
        var notify = new List<ulong>();

        var picked = Matcher.FindCandidates(pool, criteria, wave, now, PingCooldown, excluded);
        foreach (var entry in picked)
        {
            entry.LastPingedAtUtc = now;
            notify.Add(entry.UserId);
        }

        // "Bana haber ver" subscribers come after actively available players:
        // from the second wave on, or right away when nobody available fits.
        if (wave >= 1 || picked.Count == 0)
        {
            excluded.UnionWith(notify);
            var localNow = TurkeyTime.ToLocal(now);
            var today = DateOnly.FromDateTime(localNow);
            var subscriptions = await db.AlertSubscriptions
                .Where(s => s.CommunityId == listing.GuildId && s.Enabled)
                .ToListAsync(ct);
            var alerted = Matcher.FindAlertCandidates(
                subscriptions, criteria, wave, now, localNow,
                TimeSpan.FromMinutes(options.AlertCooldownMinutes), options.AlertDailyLimit, excluded);
            foreach (var s in alerted)
            {
                s.LastNotifiedAtUtc = now;
                s.CountToday = s.CountDate == today ? s.CountToday + 1 : 1;
                s.CountDate = today;
                notify.Add(s.UserId);
            }
        }

        foreach (var userId in notify)
        {
            db.ListingNotifications.Add(new ListingNotification
            {
                ListingId = listingId,
                UserId = userId,
                Wave = wave,
                SentAtUtc = now,
            });
        }

        Record(db, MetricKind.WaveSent, listing, now, wave: wave, value: notify.Count);

        // Nobody fits this wave, but the wider next wave already has someone: don't make the host wait
        // minutes for nothing. If the wider wave is empty too, keep the normal delay so players who
        // become available in the meantime are still caught.
        var skipAhead = notify.Count == 0
            && nextWaveAt is not null
            && await NextWaveHasCandidatesAsync(db, listing, criteria, wave + 1, pool, excluded, now, ct);
        if (skipAhead)
        {
            await db.Listings
                .Where(l => l.Id == listingId)
                .ExecuteUpdateAsync(s => s.SetProperty(l => l.NextWaveAtUtc, now), ct);
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return skipAhead
            ? await RunNextWaveAsync(listingId, ct) ?? new WaveResult(listingId, wave, notify)
            : new WaveResult(listingId, wave, notify);
    }

    private async Task<bool> NextWaveHasCandidatesAsync(
        PreBotDbContext db,
        Listing listing,
        MatchCriteria criteria,
        int wave,
        IReadOnlyList<Availability> pool,
        IReadOnlySet<ulong> excluded,
        DateTime now,
        CancellationToken ct)
    {
        if (Matcher.FindCandidates(pool, criteria, wave, now, PingCooldown, excluded).Count > 0)
        {
            return true;
        }

        var localNow = TurkeyTime.ToLocal(now);
        var subscriptions = await db.AlertSubscriptions.AsNoTracking()
            .Where(s => s.CommunityId == listing.GuildId && s.Enabled)
            .ToListAsync(ct);
        return Matcher.FindAlertCandidates(
            subscriptions, criteria, wave, now, localNow,
            TimeSpan.FromMinutes(options.AlertCooldownMinutes), options.AlertDailyLimit, excluded).Count > 0;
    }

    /// <summary>
    /// Listings whose next notification wave is due. When the website and the Discord bot run in one
    /// process, each worker must only take its own listings (<paramref name="onlyCommunity"/> /
    /// <paramref name="exceptCommunity"/>): running a wave marks it sent, so the wrong worker would
    /// silently swallow it.
    /// </summary>
    public async Task<IReadOnlyList<int>> GetDueWaveListingIdsAsync(
        ulong? onlyCommunity = null, ulong? exceptCommunity = null, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var query = db.Listings.AsNoTracking()
            .Where(l => l.Status == ListingStatus.Open
                && l.NextWaveAtUtc != null && l.NextWaveAtUtc <= now
                && l.ExpiresAtUtc > now
                && l.WavesSent < Matcher.MaxWaves);
        if (onlyCommunity is { } only)
        {
            query = query.Where(l => l.GuildId == only);
        }

        if (exceptCommunity is { } except)
        {
            query = query.Where(l => l.GuildId != except);
        }

        return await query.Select(l => l.Id).ToListAsync(ct);
    }

    // ---------- Expiry ----------

    /// <summary>Removes expired "available" entries and closes expired listings.</summary>
    public async Task<SweepResult> SweepAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;

        var expiredAvailabilities = await db.Availabilities.Where(a => a.ExpiresAtUtc <= now).ExecuteDeleteAsync(ct);

        var expiredIds = await db.Listings
            .Where(l => l.Status == ListingStatus.Open && l.ExpiresAtUtc <= now)
            .Select(l => l.Id)
            .ToListAsync(ct);
        if (expiredIds.Count > 0)
        {
            // Status check again: a listing that got filled in the meantime must stay "Filled".
            await db.Listings
                .Where(l => expiredIds.Contains(l.Id) && l.Status == ListingStatus.Open && l.ExpiresAtUtc <= now)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(l => l.Status, ListingStatus.Expired)
                    .SetProperty(l => l.NextWaveAtUtc, (DateTime?)null), ct);

            var expired = await db.Listings.AsNoTracking()
                .Where(l => expiredIds.Contains(l.Id) && l.Status == ListingStatus.Expired)
                .Select(l => new { l.Id, l.GuildId })
                .ToListAsync(ct);
            foreach (var l in expired)
            {
                Record(db, MetricKind.ListingExpired, l.GuildId, l.Id, now);
            }

            await db.SaveChangesAsync(ct);
        }

        return new SweepResult(expiredAvailabilities, expiredIds);
    }

    // ---------- Privacy (KVKK) ----------

    /// <summary>Deletes everything stored about a user. Seats they hold are given back first.</summary>
    public async Task<DeleteUserDataResult> DeleteUserDataAsync(ulong userId, CancellationToken ct = default)
    {
        List<int> heldSeats;
        await using (var db = await dbFactory.CreateDbContextAsync(ct))
        {
            heldSeats = await db.ListingSlots
                .Where(s => s.UserId == userId && !s.Released)
                .Select(s => s.ListingId)
                .ToListAsync(ct);
        }

        var left = new List<int>();
        foreach (var listingId in heldSeats)
        {
            if (await LeaveAsync(listingId, userId, SeatReleaseReason.AccountDeleted, ct) == LeaveResult.Left)
            {
                left.Add(listingId);
            }
        }

        await using var context = await dbFactory.CreateDbContextAsync(ct);
        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var hosted = await context.Listings
            .Where(l => l.HostUserId == userId)
            .Select(l => new { l.Id, l.ChannelId, l.MessageId })
            .ToListAsync(ct);
        var hostedIds = hosted.Select(h => h.Id).ToList();

        await context.ListingSlots
            .Where(s => s.UserId == userId || hostedIds.Contains(s.ListingId))
            .ExecuteDeleteAsync(ct);
        await context.ListingNotifications
            .Where(n => n.UserId == userId || hostedIds.Contains(n.ListingId))
            .ExecuteDeleteAsync(ct);
        await context.ListingAnnouncements
            .Where(a => hostedIds.Contains(a.ListingId))
            .ExecuteDeleteAsync(ct);
        await context.Listings.Where(l => l.HostUserId == userId).ExecuteDeleteAsync(ct);
        await context.Availabilities.Where(a => a.UserId == userId).ExecuteDeleteAsync(ct);
        await context.AlertSubscriptions.Where(a => a.UserId == userId).ExecuteDeleteAsync(ct);
        await context.PushSubscriptions.Where(p => p.UserId == userId).ExecuteDeleteAsync(ct);
        await context.Players.Where(p => p.UserId == userId).ExecuteDeleteAsync(ct);
        await tx.CommitAsync(ct);

        var removed = hosted
            .Where(h => h.MessageId is not null)
            .Select(h => new DeletedListingMessage(h.ChannelId, h.MessageId!.Value))
            .ToList();
        return new DeleteUserDataResult(left.Except(hostedIds).ToList(), removed);
    }

    // ---------- Helpers ----------

    private static MatchCriteria Criteria(Listing l) => new(l.Role, l.MinRank, l.MaxRank, l.HostUserId);

    private static void Record(PreBotDbContext db, MetricKind kind, Listing listing, DateTime now, int? wave = null, int? value = null) =>
        Record(db, kind, listing.GuildId, listing.Id, now, wave, value);

    private static void Record(
        PreBotDbContext db, MetricKind kind, ulong communityId, int listingId, DateTime now,
        int? wave = null, int? value = null, SeatReleaseReason? reason = null) =>
        db.MetricEvents.Add(new MetricEvent
        {
            AtUtc = now,
            Kind = kind,
            CommunityId = communityId,
            ListingId = listingId,
            Wave = wave,
            Value = value,
            Reason = reason,
        });

    private static async Task<Player> EnsurePlayerAsync(PreBotDbContext db, ulong userId, DateTime now, CancellationToken ct)
    {
        var player = db.Players.Local.FirstOrDefault(p => p.UserId == userId)
            ?? await db.Players.SingleOrDefaultAsync(p => p.UserId == userId, ct);
        if (player is null)
        {
            player = new Player { UserId = userId, ConsentedAtUtc = now };
            db.Players.Add(player);
        }

        return player;
    }

    /// <summary>Gives a seat back and reopens the listing. Must run inside a transaction.</summary>
    private async Task<bool> ReleaseSeatAsync(
        PreBotDbContext db, int listingId, ulong userId, SeatReleaseReason reason, DateTime now, CancellationToken ct)
    {
        var released = await db.ListingSlots
            .Where(s => s.ListingId == listingId && s.UserId == userId && !s.Released)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Released, true)
                .SetProperty(x => x.ReleasedAtUtc, now)
                .SetProperty(x => x.ReleaseReason, reason), ct);
        if (released == 0)
        {
            return false;
        }

        var community = await db.Listings.Where(l => l.Id == listingId).Select(l => l.GuildId).SingleAsync(ct);
        Record(db, MetricKind.SeatReleased, community, listingId, now, reason: reason);
        await db.SaveChangesAsync(ct);

        var minExpiry = now.AddMinutes(options.ReopenMinutes);
        await db.Listings
            .Where(l => l.Id == listingId && l.SlotsFilled > 0)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.SlotsFilled, l => l.SlotsFilled - 1), ct);
        await db.Listings
            .Where(l => l.Id == listingId && (l.Status == ListingStatus.Open || l.Status == ListingStatus.Filled))
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Status, ListingStatus.Open)
                .SetProperty(l => l.ExpiresAtUtc, l => l.ExpiresAtUtc < minExpiry ? minExpiry : l.ExpiresAtUtc)
                .SetProperty(l => l.WavesSent, 0)
                .SetProperty(l => l.NextWaveAtUtc, now), ct);
        return true;
    }

    private static async Task<ClaimOutcome> DiagnoseFailedClaimAsync(
        PreBotDbContext db, int listingId, ulong userId, DateTime now, CancellationToken ct)
    {
        var listing = await db.Listings.AsNoTracking().SingleOrDefaultAsync(l => l.Id == listingId, ct);
        if (listing is null)
        {
            return new ClaimOutcome(ClaimResult.NotFound);
        }

        if (listing.HostUserId == userId)
        {
            return new ClaimOutcome(ClaimResult.IsHost, listing.HostUserId);
        }

        if (await db.ListingSlots.AnyAsync(s => s.ListingId == listingId && s.UserId == userId, ct))
        {
            return new ClaimOutcome(ClaimResult.AlreadyJoined, listing.HostUserId);
        }

        if (listing.Status == ListingStatus.Filled
            || (listing.Status == ListingStatus.Open && listing.SlotsFilled >= listing.SlotsNeeded))
        {
            return new ClaimOutcome(ClaimResult.Full, listing.HostUserId);
        }

        return new ClaimOutcome(ClaimResult.NotOpen, listing.HostUserId);
    }
}
