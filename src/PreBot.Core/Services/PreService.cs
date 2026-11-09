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

    /// <summary>Longest "I'm available" (2 hours + one "+1 saat"), so the pool does not fill with people who left.</summary>
    public static readonly TimeSpan MaxAvailability = TimeSpan.FromHours(3);

    public async Task<Availability> SetAvailableAsync(
        ulong guildId, ulong userId, Role role, Rank rank, TimeSpan duration, CancellationToken ct = default)
    {
        if (duration > MaxAvailability)
        {
            duration = MaxAvailability;
        }

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
        string? lobbyCode = null,
        bool lobbyLocked = false,
        string? lobbyPassword = null,
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
            LobbyCode = lobbyCode,
            LobbyLocked = lobbyCode is not null && lobbyLocked,
            LobbyPassword = lobbyCode is not null && lobbyLocked ? lobbyPassword ?? Lobby.NewPassword() : null,
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
            active.Select(s => new SlotView(
                s.UserId, riotIds.GetValueOrDefault(s.UserId), s.DeclaredRole, s.DeclaredRank, s.ClaimedAtUtc)).ToList(),
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
    /// <summary>The host's most recent listing, so the form can start with the same lobby choice.</summary>
    public async Task<Listing?> GetLastHostedAsync(ulong communityId, ulong hostUserId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.Listings.AsNoTracking()
            .Where(l => l.GuildId == communityId && l.HostUserId == hostUserId)
            .OrderByDescending(l => l.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);
    }

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
                && !db.ListingSlots.Any(s => s.ListingId == listingId && s.UserId == userId)
                && !db.ListingSlots.Any(s => s.UserId == userId && !s.Released && s.ListingId != listingId
                    && db.Listings.Any(o => o.Id == s.ListingId && o.Status == ListingStatus.Open)))
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.SlotsFilled, l => l.SlotsFilled + 1), ct);

        if (updated == 0)
        {
            await tx.RollbackAsync(ct);
            return await DiagnoseFailedClaimAsync(db, listingId, userId, now, ct);
        }

        // What the player told us about themselves (pool entry first, else their "notify me" setting), so the
        // host sees role and rank next to the Riot ID.
        var community = await db.Listings.Where(l => l.Id == listingId).Select(l => l.GuildId).SingleAsync(ct);
        var declared = await db.Availabilities.AsNoTracking()
                .Where(a => a.GuildId == community && a.UserId == userId)
                .Select(a => new { a.Role, a.Rank })
                .FirstOrDefaultAsync(ct)
            ?? await db.AlertSubscriptions.AsNoTracking()
                .Where(a => a.CommunityId == community && a.UserId == userId && a.Enabled)
                .OrderByDescending(a => a.CreatedAtUtc)
                .Select(a => new { a.Role, a.Rank })
                .FirstOrDefaultAsync(ct);

        db.ListingSlots.Add(new ListingSlot
        {
            ListingId = listingId,
            UserId = userId,
            ClaimedAtUtc = now,
            DeclaredRole = declared?.Role,
            DeclaredRank = declared?.Rank,
        });
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
        var heard = await db.ListingNotifications.AsNoTracking()
            .Where(n => n.ListingId == listingId && n.UserId == userId)
            .Select(n => new { n.Source, n.SentAtUtc })
            .FirstOrDefaultAsync(ct);
        Record(db, MetricKind.SeatClaimed, info.GuildId, listingId, now, value: secondsOpen,
            source: heard?.Source, sinceNotified: heard is null ? null : (int)(now - heard.SentAtUtc).TotalSeconds);
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

    /// <summary>"Gelmedi" only after this long: the player needs a moment to accept the friend request and invite.</summary>
    public static readonly TimeSpan NoShowAfter = TimeSpan.FromMinutes(3);

    /// <summary>How many players a host may turn down ("Uygun değil") in one listing.</summary>
    public const int MaxDeclinesPerListing = 3;

    /// <summary>
    /// The host frees one specific seat: <see cref="SeatReleaseReason.NoShow"/> ("Gelmedi", counted against the
    /// player) or <see cref="SeatReleaseReason.HostDeclined"/> ("Uygun değil", not counted). The listing reopens;
    /// the player cannot take a seat in it again and is not notified about it again.
    /// </summary>
    public async Task<HostActionOutcome> ReleaseByHostAsync(
        int listingId, ulong hostUserId, ulong userId, SeatReleaseReason reason, CancellationToken ct = default)
    {
        if (reason is not (SeatReleaseReason.NoShow or SeatReleaseReason.HostDeclined))
        {
            throw new ArgumentOutOfRangeException(nameof(reason));
        }

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

        var now = Now;
        var claimedAt = await db.ListingSlots
            .Where(s => s.ListingId == listingId && s.UserId == userId && !s.Released)
            .Select(s => (DateTime?)s.ClaimedAtUtc)
            .SingleOrDefaultAsync(ct);
        if (claimedAt is null)
        {
            return new HostActionOutcome(HostActionResult.NothingToDo);
        }

        if (reason == SeatReleaseReason.NoShow && now - claimedAt.Value < NoShowAfter)
        {
            return new HostActionOutcome(HostActionResult.TooEarly);
        }

        if (reason == SeatReleaseReason.HostDeclined
            && await db.ListingSlots.CountAsync(s => s.ListingId == listingId && s.ReleaseReason == SeatReleaseReason.HostDeclined, ct)
                >= MaxDeclinesPerListing)
        {
            return new HostActionOutcome(HostActionResult.LimitReached);
        }

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        if (!await ReleaseSeatAsync(db, listingId, userId, reason, now, ct))
        {
            await tx.RollbackAsync(ct);
            return new HostActionOutcome(HostActionResult.NothingToDo);
        }

        await tx.CommitAsync(ct);
        return new HostActionOutcome(HostActionResult.Done, userId);
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

    /// <summary>
    /// Someone joined through the custom game lobby: one more seat is taken. When that was the last missing
    /// seat, the listing is done (counted as filled) and stops notifying people; otherwise it keeps looking.
    /// </summary>
    public async Task<HostActionOutcome> MarkFilledViaLobbyAsync(int listingId, ulong hostUserId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var listing = await db.Listings.AsNoTracking().SingleOrDefaultAsync(l => l.Id == listingId, ct);
        if (listing is null)
        {
            return new HostActionOutcome(HostActionResult.NotFound);
        }

        if (listing.HostUserId != hostUserId)
        {
            return new HostActionOutcome(HostActionResult.NotHost);
        }

        var counted = await db.Listings
            .Where(l => l.Id == listingId && l.Status == ListingStatus.Open && l.SlotsFilled < l.SlotsNeeded)
            .ExecuteUpdateAsync(s => s.SetProperty(l => l.SlotsFilled, l => l.SlotsFilled + 1), ct);
        if (counted == 0)
        {
            return new HostActionOutcome(HostActionResult.NothingToDo);
        }

        var filled = await db.Listings
            .Where(l => l.Id == listingId && l.Status == ListingStatus.Open && l.SlotsFilled >= l.SlotsNeeded)
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Status, ListingStatus.Filled)
                .SetProperty(l => l.NextWaveAtUtc, (DateTime?)null), ct);
        if (filled > 0)
        {
            Record(db, MetricKind.FilledViaLobby, listing.GuildId, listingId, now, value: (int)(now - listing.CreatedAtUtc).TotalSeconds);
            await db.SaveChangesAsync(ct);
        }

        return new HostActionOutcome(HostActionResult.Done);
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
            || listing.NextWaveAtUtc is null
            || listing.NextWaveAtUtc > now)
        {
            return null;
        }

        var wave = listing.WavesSent;
        // After the widening waves the listing keeps listening for newcomers until it fills or expires.
        DateTime? nextWaveAt = now + Matcher.DelayAfterWave(wave);

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

        excluded.UnionWith(await SilencedAsync(db, now, ct));

        var criteria = Criteria(listing);
        var notify = new List<ulong>();
        var sources = new Dictionary<ulong, NotificationSource>();
        var reachable = listing.GuildId == Communities.Web ? await ReachableAsync(db, pool.Select(a => a.UserId), ct) : null;

        var picked = Matcher.FindCandidates(pool, criteria, wave, now, PingCooldown, excluded, reachable);
        foreach (var entry in picked)
        {
            entry.LastPingedAtUtc = now;
            notify.Add(entry.UserId);
            sources[entry.UserId] = NotificationSource.Pool;
        }

        // "Bana haber ver" subscribers come after actively available players: from the second wave on, or
        // right away when the active pool cannot fill the first wave (in a small community it rarely can).
        if (wave >= 1 || picked.Count < Matcher.WaveSize(wave))
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
                sources[s.UserId] = NotificationSource.Alert;
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
                Source = sources[userId],
            });
        }

        Record(db, MetricKind.WaveSent, listing, now, wave: wave, value: notify.Count);
        foreach (var bySource in sources.Values.GroupBy(s => s))
        {
            Record(db, MetricKind.NotificationsSent, listing.GuildId, listingId, now, wave: wave, value: bySource.Count(), source: bySource.Key);
        }

        // Nobody fits this wave, but the wider next wave already has someone: don't make the host wait
        // minutes for nothing. If the wider wave is empty too, keep the normal delay so players who
        // become available in the meantime are still caught.
        var skipAhead = notify.Count == 0
            && wave + 1 < Matcher.MaxWaves
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

    /// <summary>
    /// Someone just said "available": if an open listing already fits them, tell them now instead of
    /// waiting for its next wave. Picks the most urgent one (fewest missing seats, then oldest).
    /// </summary>
    public async Task<WaveResult?> MatchNewcomerAsync(ulong communityId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var me = await db.Availabilities.SingleOrDefaultAsync(a => a.GuildId == communityId && a.UserId == userId && a.ExpiresAtUtc > now, ct);
        if (me is null || (me.LastPingedAtUtc is { } last && now - last < PingCooldown)
            || (await SilencedAsync(db, now, ct)).Contains(userId))
        {
            return null;
        }

        var open = await db.Listings.AsNoTracking()
            .Where(l => l.GuildId == communityId && l.Status == ListingStatus.Open && l.ExpiresAtUtc > now
                && l.HostUserId != userId
                && !db.ListingSlots.Any(s => s.ListingId == l.Id && s.UserId == userId)
                && !db.ListingNotifications.Any(n => n.ListingId == l.Id && n.UserId == userId))
            .ToListAsync(ct);

        var best = open
            .Where(l => Matcher.FindCandidates([me], Criteria(l), Math.Clamp(l.WavesSent - 1, 0, Matcher.MaxWaves - 1), now, PingCooldown, new HashSet<ulong>()).Count > 0)
            .OrderBy(l => l.SlotsNeeded - l.SlotsFilled)
            .ThenBy(l => l.CreatedAtUtc)
            .FirstOrDefault();
        if (best is null)
        {
            return null;
        }

        var wave = Math.Max(0, best.WavesSent - 1);
        me.LastPingedAtUtc = now;
        db.ListingNotifications.Add(new ListingNotification
        {
            ListingId = best.Id, UserId = userId, Wave = wave, SentAtUtc = now, Source = NotificationSource.Newcomer,
        });
        Record(db, MetricKind.WaveSent, best, now, wave: wave, value: 1);
        Record(db, MetricKind.NotificationsSent, best.GuildId, best.Id, now, wave: wave, value: 1, source: NotificationSource.Newcomer);
        await db.SaveChangesAsync(ct);
        return new WaveResult(best.Id, wave, [userId]);
    }

    // ---------- "Maçtayım, bitince haber ver" ----------

    /// <summary>
    /// How long a player who says "I'm in a match" is left alone. A game lasts about 30 minutes and the
    /// notification arrives at a random point in it, so 25 minutes covers most of the rest plus the end screen.
    /// </summary>
    public static readonly TimeSpan InGameFor = TimeSpan.FromMinutes(25);

    /// <summary>
    /// The player is in a match: no listing notifications until it is over, then the listing they were looking
    /// at (if still open) or another one that fits. Returns when they will hear from PreBot again.
    /// </summary>
    public async Task<DateTime> ReportInGameAsync(ulong userId, int? listingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var player = await EnsurePlayerAsync(db, userId, now, ct);
        player.InGameUntilUtc = now + InGameFor;
        player.InGameListingId = listingId;

        if (listingId is { } id
            && await db.Listings.Where(l => l.Id == id).Select(l => (ulong?)l.GuildId).SingleOrDefaultAsync(ct) is { } community)
        {
            var heard = await db.ListingNotifications.FirstOrDefaultAsync(n => n.ListingId == id && n.UserId == userId, ct);
            Record(db, MetricKind.InGameReported, community, id, now,
                source: heard?.Source, sinceNotified: heard is null ? null : (int)(now - heard.SentAtUtc).TotalSeconds);
            if (heard is not null)
            {
                // Forget that they were told, so this listing can be offered to them again after the match.
                db.ListingNotifications.Remove(heard);
            }
        }

        await db.SaveChangesAsync(ct);
        return player.InGameUntilUtc.Value;
    }

    public async Task<DateTime?> GetInGameUntilAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        return await db.Players.Where(p => p.UserId == userId && p.InGameUntilUtc > now).Select(p => p.InGameUntilUtc).SingleOrDefaultAsync(ct);
    }

    /// <summary>"Maç bitti, hazırım": back before the time ran out. Returns the listing they had put off, if any.</summary>
    public async Task<int?> EndInGameAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var listingId = await db.Players.Where(p => p.UserId == userId && p.InGameUntilUtc != null)
            .Select(p => p.InGameListingId).FirstOrDefaultAsync(ct);
        await db.Players.Where(p => p.UserId == userId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.InGameUntilUtc, (DateTime?)null)
                .SetProperty(p => p.InGameListingId, (int?)null), ct);
        return listingId;
    }

    /// <summary>Players whose match should be over now. Each one is returned once (the mark is cleared).</summary>
    public async Task<IReadOnlyList<(ulong UserId, int? ListingId)>> TakeFinishedMatchesAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var due = await db.Players.AsNoTracking()
            .Where(p => p.InGameUntilUtc != null && p.InGameUntilUtc <= now)
            .Select(p => new { p.UserId, p.InGameUntilUtc, p.InGameListingId })
            .ToListAsync(ct);

        var taken = new List<(ulong, int?)>();
        foreach (var p in due)
        {
            var cleared = await db.Players
                .Where(x => x.UserId == p.UserId && x.InGameUntilUtc == p.InGameUntilUtc)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.InGameUntilUtc, (DateTime?)null)
                    .SetProperty(x => x.InGameListingId, (int?)null), ct);
            if (cleared == 1)
            {
                taken.Add((p.UserId, p.InGameListingId));
            }
        }

        return taken;
    }

    /// <summary>
    /// The player's match is over: offer the listing they put off if it is still open and fits them, otherwise
    /// the most urgent open listing that fits their pool entry or "Bana haber ver" setting.
    /// </summary>
    public async Task<WaveResult?> MatchAfterMatchAsync(ulong communityId, ulong userId, int? preferredListingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        if ((await SilencedAsync(db, now, ct)).Contains(userId))
        {
            return null;
        }

        var entry = await db.Availabilities.SingleOrDefaultAsync(a => a.GuildId == communityId && a.UserId == userId && a.ExpiresAtUtc > now, ct);
        var me = entry;
        if (me is null)
        {
            var alert = await db.AlertSubscriptions.AsNoTracking()
                .Where(a => a.CommunityId == communityId && a.UserId == userId && a.Enabled)
                .OrderByDescending(a => a.CreatedAtUtc)
                .FirstOrDefaultAsync(ct);
            if (alert is null)
            {
                return null;
            }

            me = new Availability { GuildId = communityId, UserId = userId, Role = alert.Role, Rank = alert.Rank, ExpiresAtUtc = now.AddHours(1) };
        }

        var open = await db.Listings.AsNoTracking()
            .Where(l => l.GuildId == communityId && l.Status == ListingStatus.Open && l.ExpiresAtUtc > now
                && l.HostUserId != userId
                && !db.ListingSlots.Any(s => s.ListingId == l.Id && s.UserId == userId)
                && !db.ListingNotifications.Any(n => n.ListingId == l.Id && n.UserId == userId))
            .ToListAsync(ct);
        var fits = open
            .Where(l => Matcher.FindCandidates([me], Criteria(l), Math.Clamp(l.WavesSent - 1, 0, Matcher.MaxWaves - 1), now, TimeSpan.Zero, new HashSet<ulong>()).Count > 0)
            .ToList();
        var best = fits.FirstOrDefault(l => l.Id == preferredListingId)
            ?? fits.OrderBy(l => l.SlotsNeeded - l.SlotsFilled).ThenBy(l => l.CreatedAtUtc).FirstOrDefault();
        if (best is null)
        {
            return null;
        }

        var wave = Math.Max(0, best.WavesSent - 1);
        if (entry is not null)
        {
            entry.LastPingedAtUtc = now;
        }

        db.ListingNotifications.Add(new ListingNotification
        {
            ListingId = best.Id, UserId = userId, Wave = wave, SentAtUtc = now, Source = NotificationSource.AfterMatch,
        });
        Record(db, MetricKind.NotificationsSent, best.GuildId, best.Id, now, wave: wave, value: 1, source: NotificationSource.AfterMatch);
        await db.SaveChangesAsync(ct);
        return new WaveResult(best.Id, wave, [userId]);
    }

    /// <summary>For the host: how many notified players are in a match and asked to hear about this listing afterwards.</summary>
    public async Task<(int Count, DateTime? EarliestBackUtc)> GetWaitingAfterMatchAsync(int listingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var times = await db.Players
            .Where(p => p.InGameListingId == listingId && p.InGameUntilUtc > now)
            .Select(p => p.InGameUntilUtc!.Value)
            .ToListAsync(ct);
        return (times.Count, times.Count == 0 ? null : times.Min());
    }

    /// <summary>Notified players who did not get a seat: they hear "Doldu / Kapandı" when the listing closes.</summary>
    public async Task<IReadOnlyList<ulong>> GetNotifiedUserIdsAsync(int listingId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        return await db.ListingNotifications
            .Where(n => n.ListingId == listingId
                && !db.ListingSlots.Any(s => s.ListingId == listingId && s.UserId == n.UserId && !s.Released))
            .Select(n => n.UserId)
            .Distinct()
            .ToListAsync(ct);
    }

    /// <summary>
    /// A notified player tapped "Katıl" too late. Counted for the trial, and a "Bana haber ver" alert they lost
    /// this way does not use up their daily limit or cooldown: they did answer, someone was just faster.
    /// </summary>
    public async Task RecordLostRaceAsync(int listingId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var heard = await db.ListingNotifications.FirstOrDefaultAsync(n => n.ListingId == listingId && n.UserId == userId && n.LostRaceAtUtc == null, ct);
        if (heard is null)
        {
            return; // not invited to this race, or already counted
        }

        heard.LostRaceAtUtc = now;
        var community = await db.Listings.Where(l => l.Id == listingId).Select(l => l.GuildId).SingleAsync(ct);
        Record(db, MetricKind.LostRace, community, listingId, now,
            source: heard.Source, sinceNotified: (int)(now - heard.SentAtUtc).TotalSeconds);
        await db.SaveChangesAsync(ct);

        if (heard.Source == NotificationSource.Alert)
        {
            var today = DateOnly.FromDateTime(TurkeyTime.ToLocal(now));
            await db.AlertSubscriptions
                .Where(s => s.CommunityId == community && s.UserId == userId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.LastNotifiedAtUtc, (DateTime?)null)
                    .SetProperty(x => x.CountToday, x => x.CountDate == today && x.CountToday > 0 ? x.CountToday - 1 : x.CountToday), ct);
        }
    }

    // ---------- Measurements for the trial ----------

    /// <summary>A player opened a listing notification (counted once per notification).</summary>
    public async Task RecordNotificationOpenedAsync(int listingId, ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        var heard = await db.ListingNotifications.FirstOrDefaultAsync(n => n.ListingId == listingId && n.UserId == userId && n.OpenedAtUtc == null, ct);
        if (heard is null)
        {
            return;
        }

        heard.OpenedAtUtc = now;
        var community = await db.Listings.Where(l => l.Id == listingId).Select(l => l.GuildId).SingleAsync(ct);
        Record(db, MetricKind.NotificationOpened, community, listingId, now,
            source: heard.Source, sinceNotified: (int)(now - heard.SentAtUtc).TotalSeconds);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Records an anonymous event about a listing (e.g. a profile opened, push deliveries accepted).</summary>
    public async Task RecordAsync(MetricKind kind, int listingId, int? wave = null, int? value = null, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var community = await db.Listings.Where(l => l.Id == listingId).Select(l => (ulong?)l.GuildId).SingleOrDefaultAsync(ct);
        if (community is null)
        {
            return;
        }

        Record(db, kind, community.Value, listingId, Now, wave, value);
        await db.SaveChangesAsync(ct);
    }

    /// <summary>Players who must not be notified right now: muted ("sustur"), in a match ("Maçtayım") or banned.</summary>
    private static async Task<HashSet<ulong>> SilencedAsync(PreBotDbContext db, DateTime now, CancellationToken ct)
    {
        var muted = await db.Players
            .Where(p => (p.MutedUntilUtc != null && p.MutedUntilUtc > now) || (p.InGameUntilUtc != null && p.InGameUntilUtc > now))
            .Select(p => p.UserId)
            .ToListAsync(ct);
        var banned = await db.Accounts.Where(a => a.BannedAtUtc != null).Select(a => a.Id).ToListAsync(ct);
        return muted.Concat(banned.Select(b => (ulong)b)).ToHashSet();
    }

    /// <summary>Stop (or resume, with null) notifications about other people's listings until the given time.</summary>
    public async Task MuteAsync(ulong userId, DateTime? untilUtc, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var player = await EnsurePlayerAsync(db, userId, Now, ct);
        player.MutedUntilUtc = untilUtc;
        await db.SaveChangesAsync(ct);
    }

    public async Task<DateTime?> GetMutedUntilAsync(ulong userId, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;
        return await db.Players.Where(p => p.UserId == userId && p.MutedUntilUtc > now).Select(p => p.MutedUntilUtc).SingleOrDefaultAsync(ct);
    }

    /// <summary>
    /// Website players we can reach while the site is closed: a push device, or a Discord account that accepts
    /// DMs (only when the bot runs here; DMs refused recently don't count).
    /// </summary>
    private async Task<IReadOnlySet<ulong>> ReachableAsync(PreBotDbContext db, IEnumerable<ulong> userIds, CancellationToken ct)
    {
        var ids = userIds.Distinct().ToList();
        var accountIds = ids.Select(u => (long)u).ToList();
        var withPush = await db.PushSubscriptions.Where(p => ids.Contains(p.UserId)).Select(p => p.UserId).ToListAsync(ct);
        var retryBefore = Now - AccountService.DmRetryAfter;
        var discord = options.DiscordDms
            ? await db.Accounts
                .Where(a => accountIds.Contains(a.Id) && a.Provider == "discord" && (a.DmClosedAtUtc == null || a.DmClosedAtUtc <= retryBefore))
                .Select(a => a.Id)
                .ToListAsync(ct)
            : [];
        return withPush.Concat(discord.Select(d => (ulong)d)).ToHashSet();
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
                && l.ExpiresAtUtc > now);
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

    /// <summary>
    /// Removes expired "available" entries and closes expired listings. Like the waves, each worker only
    /// closes its own community's listings, so the one that reacts (host push, announcement) sees them.
    /// </summary>
    public async Task<SweepResult> SweepAsync(ulong? onlyCommunity = null, ulong? exceptCommunity = null, CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var now = Now;

        var availabilities = db.Availabilities.Where(a => a.ExpiresAtUtc <= now);
        var listings = db.Listings.Where(l => l.Status == ListingStatus.Open && l.ExpiresAtUtc <= now);
        if (onlyCommunity is { } only)
        {
            availabilities = availabilities.Where(a => a.GuildId == only);
            listings = listings.Where(l => l.GuildId == only);
        }

        if (exceptCommunity is { } except)
        {
            availabilities = availabilities.Where(a => a.GuildId != except);
            listings = listings.Where(l => l.GuildId != except);
        }

        var expiredAvailabilities = await availabilities.ExecuteDeleteAsync(ct);
        var expiredIds = await listings.Select(l => l.Id).ToListAsync(ct);
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

    /// <summary>How long closed listings (with their seats and notification records) are kept.</summary>
    public static readonly TimeSpan ClosedListingRetention = TimeSpan.FromDays(90);

    /// <summary>Deletes closed listings older than <see cref="ClosedListingRetention"/>. Returns how many.</summary>
    public async Task<int> PurgeOldListingsAsync(CancellationToken ct = default)
    {
        await using var db = await dbFactory.CreateDbContextAsync(ct);
        var cutoff = Now - ClosedListingRetention;
        var ids = await db.Listings
            .Where(l => l.CreatedAtUtc < cutoff && l.Status != ListingStatus.Open)
            .Select(l => l.Id)
            .ToListAsync(ct);
        if (ids.Count == 0)
        {
            return 0;
        }

        await using var tx = await db.Database.BeginTransactionAsync(ct);
        await db.ListingSlots.Where(s => ids.Contains(s.ListingId)).ExecuteDeleteAsync(ct);
        await db.ListingNotifications.Where(n => ids.Contains(n.ListingId)).ExecuteDeleteAsync(ct);
        await db.ListingAnnouncements.Where(a => ids.Contains(a.ListingId)).ExecuteDeleteAsync(ct);
        await db.Listings.Where(l => ids.Contains(l.Id)).ExecuteDeleteAsync(ct);
        await tx.CommitAsync(ct);
        return ids.Count;
    }

    /// <summary>
    /// The operator closed this account: give back the seats it holds (the hosts get a new player), close the
    /// listings it hosts so nobody is notified about them any more, and forget its notification devices.
    /// </summary>
    public async Task<(IReadOnlyList<int> LeftListingIds, IReadOnlyList<int> ClosedListingIds)> RemoveBannedUserAsync(
        ulong userId, CancellationToken ct = default)
    {
        List<int> held;
        List<int> hosted;
        await using (var db = await dbFactory.CreateDbContextAsync(ct))
        {
            held = await db.ListingSlots
                .Where(s => s.UserId == userId && !s.Released)
                .Select(s => s.ListingId)
                .ToListAsync(ct);
            hosted = await db.Listings
                .Where(l => l.HostUserId == userId && (l.Status == ListingStatus.Open || l.Status == ListingStatus.Filled))
                .Select(l => l.Id)
                .ToListAsync(ct);
            await db.PushSubscriptions.Where(p => p.UserId == userId).ExecuteDeleteAsync(ct);
            await db.Availabilities.Where(a => a.UserId == userId).ExecuteDeleteAsync(ct);
            await db.AlertSubscriptions.Where(a => a.UserId == userId).ExecuteDeleteAsync(ct);
        }

        var left = new List<int>();
        foreach (var listingId in held.Except(hosted))
        {
            if (await LeaveAsync(listingId, userId, SeatReleaseReason.Banned, ct) == LeaveResult.Left)
            {
                left.Add(listingId);
            }
        }

        var closed = new List<int>();
        foreach (var listingId in hosted)
        {
            if ((await CancelAsync(listingId, userId, ct)).Result == HostActionResult.Done)
            {
                closed.Add(listingId);
            }
        }

        return (left, closed);
    }

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
        // Reports other people filed about this user or their listings stay for the operator to review
        // (abuse evidence, otherwise deleting the account would erase it); reports they filed themselves go.
        await context.ListingReports
            .Where(r => r.ReporterUserId == userId)
            .ExecuteDeleteAsync(ct);
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
        int? wave = null, int? value = null, SeatReleaseReason? reason = null,
        NotificationSource? source = null, int? sinceNotified = null) =>
        db.MetricEvents.Add(new MetricEvent
        {
            AtUtc = now,
            Kind = kind,
            CommunityId = communityId,
            ListingId = listingId,
            Wave = wave,
            Value = value,
            Reason = reason,
            Source = source,
            SinceNotifiedSeconds = sinceNotified,
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

        // A seat is free again: players asked earlier may say yes now (the per-player cooldown still applies).
        await db.ListingNotifications.Where(n => n.ListingId == listingId).ExecuteDeleteAsync(ct);

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

        if (await db.ListingSlots.AnyAsync(s => s.UserId == userId && !s.Released && s.ListingId != listingId
                && db.Listings.Any(o => o.Id == s.ListingId && o.Status == ListingStatus.Open), ct))
        {
            return new ClaimOutcome(ClaimResult.HoldsOtherSeat, listing.HostUserId);
        }

        if (listing.Status == ListingStatus.Filled
            || (listing.Status == ListingStatus.Open && listing.SlotsFilled >= listing.SlotsNeeded))
        {
            return new ClaimOutcome(ClaimResult.Full, listing.HostUserId);
        }

        return new ClaimOutcome(ClaimResult.NotOpen, listing.HostUserId);
    }
}
