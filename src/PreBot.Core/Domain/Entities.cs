namespace PreBot.Core.Domain;

/// <summary>A Discord user who has used the bot at least once (and saw the privacy notice).</summary>
public sealed class Player
{
    public ulong DiscordUserId { get; set; }

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
