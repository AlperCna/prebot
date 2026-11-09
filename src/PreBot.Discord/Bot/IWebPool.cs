using PreBot.Core.Domain;
using PreBot.Core.Services;

namespace PreBot.Bot;

/// <summary>
/// The website's single player pool, as seen from Discord. When the bot runs inside the website it
/// registers an implementation, and the slash commands work on the same pool, listings and accounts as
/// the site (see <see cref="WebPoolModule"/>): a Discord user and a site user can fill each other's
/// listings. The standalone bot has no implementation and keeps its own per-server pools (<see cref="PreModule"/>).
/// </summary>
public interface IWebPool
{
    /// <summary>The website account of a Discord user (the same one "Discord ile giriş" uses), created if needed.</summary>
    Task<ulong> AccountForDiscordUserAsync(ulong discordUserId, string displayName);

    Task<Availability> SetAvailableAsync(ulong userId, Role role, Rank rank, TimeSpan duration);

    Task ClearAvailableAsync(ulong userId);

    Task<PoolSummary> GetPoolSummaryAsync();

    /// <summary>Opens a listing, notifies the pool and announces it (also in <paramref name="announceChannelId"/>).</summary>
    Task<CreateListingResult> CreateListingAsync(
        ulong hostUserId, Role role, Rank min, Rank max, int slots, string? note, bool withLobby, bool lobbyLocked, ulong announceChannelId);

    Task<ClaimOutcome> ClaimAsync(int listingId, ulong userId);

    Task<LeaveResult> LeaveAsync(int listingId, ulong userId);

    Task<ListingView?> GetListingAsync(int listingId);

    Task<string?> GetRiotIdAsync(ulong userId);

    Task<bool> SetRiotIdAsync(ulong userId, string riotId);

    Task DeleteAccountAsync(ulong userId);

    /// <summary>Absolute link to a listing page on the website.</summary>
    string ListingUrl(int listingId);
}
