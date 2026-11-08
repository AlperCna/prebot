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
