namespace PreBot.Core.Domain;

/// <summary>
/// Where a pool and its listings live. Discord servers use their guild id (a large snowflake);
/// the website uses the fixed id <see cref="Web"/>, so both can share one database.
/// </summary>
public static class Communities
{
    public const ulong Web = 1;
}

/// <summary>
/// Someone who has used PreBot at least once (and saw the privacy notice).
/// <see cref="UserId"/> is a Discord user id for bot users, or a <see cref="UserAccount"/> id for website users.
/// </summary>
public sealed class Player
{
    public ulong UserId { get; set; }

    /// <summary>Self-declared Riot ID ("Name#TAG"). Not verified against the Riot API in the MVP.</summary>
    public string? RiotId { get; set; }

    public DateTime ConsentedAtUtc { get; set; }
}

/// <summary>"I'm free right now" entry. One per user per server; disappears on its own when it expires.</summary>
public sealed class Availability
{
    public int Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public Role Role { get; set; }
    public Rank Rank { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>Last time the bot pinged this user for any listing. Used for the anti-spam cooldown.</summary>
    public DateTime? LastPingedAtUtc { get; set; }
}

/// <summary>"We are short N players" post.</summary>
public sealed class Listing
{
    public int Id { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong? MessageId { get; set; }
    public ulong HostUserId { get; set; }
    public Role Role { get; set; }
    public Rank MinRank { get; set; }
    public Rank MaxRank { get; set; }
    public int SlotsNeeded { get; set; }
    public int SlotsFilled { get; set; }
    public ListingStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }

    /// <summary>How many notification waves have been sent (0..Matcher.MaxWaves).</summary>
    public int WavesSent { get; set; }

    public DateTime? NextWaveAtUtc { get; set; }

    public List<ListingSlot> Slots { get; set; } = [];
}

/// <summary>A player who took a seat in a listing. Released seats stay for history (and block re-joining).</summary>
public sealed class ListingSlot
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public ulong UserId { get; set; }
    public DateTime ClaimedAtUtc { get; set; }
    public bool Released { get; set; }
    public DateTime? ReleasedAtUtc { get; set; }

    /// <summary>Why the seat was given back (null while held).</summary>
    public SeatReleaseReason? ReleaseReason { get; set; }
}

/// <summary>
/// Append-only, anonymous log of what happened to listings. Statistics are computed from it, so they don't
/// change when a user deletes their data (no user id is stored here).
/// </summary>
public sealed class MetricEvent
{
    public long Id { get; set; }
    public DateTime AtUtc { get; set; }
    public MetricKind Kind { get; set; }
    public ulong CommunityId { get; set; }
    public int ListingId { get; set; }
    public int? Wave { get; set; }

    /// <summary>Kind-specific number: seats needed, people notified, or seconds since the listing opened.</summary>
    public int? Value { get; set; }

    public SeatReleaseReason? Reason { get; set; }
}

/// <summary>A website login. Its <see cref="Id"/> is the user id used everywhere else for website users.</summary>
public sealed class UserAccount
{
    public long Id { get; set; }

    /// <summary>Login provider: "dev" (local testing only), "google", "discord".</summary>
    public string Provider { get; set; } = "";

    /// <summary>The user's id at that provider.</summary>
    public string ProviderKey { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; }

    /// <summary>When the user ticked "I accept the terms and privacy notice, I am 16+" (first time).</summary>
    public DateTime? TermsAcceptedAtUtc { get; set; }

    public ulong UserId => (ulong)Id;
}

/// <summary>Who was already notified about a listing, so nobody hears about the same listing twice.</summary>
public sealed class ListingNotification
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public ulong UserId { get; set; }
    public int Wave { get; set; }
    public DateTime SentAtUtc { get; set; }
}

/// <summary>A browser/device that agreed to receive push notifications (Web Push). A user can have several.</summary>
public sealed class WebPushSubscription
{
    public int Id { get; set; }
    public ulong UserId { get; set; }

    /// <summary>Push service URL for this browser. Unique per browser install.</summary>
    public string Endpoint { get; set; } = "";

    public string P256dh { get; set; } = "";
    public string Auth { get; set; } = "";

    /// <summary>Short label to tell devices apart, e.g. "Android · Chrome".</summary>
    public string? Device { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastSuccessAtUtc { get; set; }
}

/// <summary>
/// "Notify me when a listing fits me" — a standing alert for people who don't press "available" every evening.
/// They are notified after actively available players (from the second wave, or at once if nobody is available).
/// </summary>
public sealed class AlertSubscription
{
    public int Id { get; set; }
    public ulong CommunityId { get; set; }
    public ulong UserId { get; set; }
    public Role Role { get; set; }
    public Rank Rank { get; set; }

    /// <summary>Turkey local time window [FromHour, ToHour). May wrap past midnight (e.g. 20 → 2). Equal = all day.</summary>
    public int FromHour { get; set; }

    public int ToHour { get; set; }

    public bool Enabled { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastNotifiedAtUtc { get; set; }

    /// <summary>Turkey local date that <see cref="CountToday"/> refers to.</summary>
    public DateOnly? CountDate { get; set; }

    public int CountToday { get; set; }
}

/// <summary>A Discord message that announced a website listing, so it can be updated when the listing closes.</summary>
public sealed class ListingAnnouncement
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
    public DateTime PostedAtUtc { get; set; }
}
