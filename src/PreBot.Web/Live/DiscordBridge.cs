using Discord;
using Discord.Net;
using Discord.WebSocket;
using PreBot.Bot;
using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;
using PreBot.Web.Auth;

namespace PreBot.Web.Live;

/// <summary>Sends a personal message to a user outside the website (Discord DM). Never throws.</summary>
public interface IDirectMessenger
{
    /// <summary>Users (website ids) this messenger can reach and who would otherwise get nothing (no push device).</summary>
    Task<IReadOnlyDictionary<ulong, ulong>> ReachableWithoutPushAsync(IEnumerable<ulong> userIds, CancellationToken ct = default);

    /// <summary>"X lazım!" with a Katıl button that takes the seat right from Discord.</summary>
    Task SendListingAsync(ulong discordUserId, WaveAlert alert, string listingUrl, CancellationToken ct = default);

    Task SendTextAsync(ulong discordUserId, string title, string body, string? url, CancellationToken ct = default);
}

/// <summary>No Discord bot: nobody is reachable.</summary>
public sealed class NoDirectMessenger : IDirectMessenger
{
    public Task<IReadOnlyDictionary<ulong, ulong>> ReachableWithoutPushAsync(IEnumerable<ulong> userIds, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyDictionary<ulong, ulong>>(new Dictionary<ulong, ulong>());

    public Task SendListingAsync(ulong discordUserId, WaveAlert alert, string listingUrl, CancellationToken ct = default) => Task.CompletedTask;

    public Task SendTextAsync(ulong discordUserId, string title, string body, string? url, CancellationToken ct = default) => Task.CompletedTask;
}

/// <summary>
/// Discord DMs for players who joined through Discord. Only used for people without a push device, so
/// nobody gets the same alert twice. Users with DMs closed are remembered (in the database, so matching
/// does not count them as reachable either) and tried again after a week.
/// </summary>
public sealed class DiscordDirectMessenger(
    DiscordSocketClient client,
    AccountService accounts,
    SubscriptionService subscriptions,
    ILogger<DiscordDirectMessenger> logger) : IDirectMessenger
{
    public async Task<IReadOnlyDictionary<ulong, ulong>> ReachableWithoutPushAsync(IEnumerable<ulong> userIds, CancellationToken ct = default)
    {
        var ids = userIds.ToList();
        var discord = await accounts.GetDiscordIdsAsync(ids, dmOpenOnly: true, ct);
        if (discord.Count == 0)
        {
            return discord;
        }

        var withPush = (await subscriptions.GetPushSubscriptionsAsync(discord.Keys, ct)).Select(s => s.UserId).ToHashSet();
        return discord
            .Where(kv => !withPush.Contains(kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value);
    }

    public Task SendListingAsync(ulong discordUserId, WaveAlert alert, string listingUrl, CancellationToken ct = default)
    {
        var embed = new EmbedBuilder()
            .WithTitle($"📣 {TurkishNames.Of(alert.Role)} lazım!")
            .WithDescription((alert.SlotsMissing == 1
                ? $"4 kişi seni bekliyor · {TurkishNames.RankRange(alert.MinRank, alert.MaxRank)}. İlk basan alır."
                : $"{TurkishNames.RankRange(alert.MinRank, alert.MaxRank)} · {alert.SlotsMissing} kişi eksik. İlk basan alır.")
                + (alert.LobbyHint is null ? "" : $"\n🎮 {alert.LobbyHint}"))
            .WithColor(new Color(0xC8AA6E))
            .WithFooter(ListingRenderer.Disclaimer)
            .Build();
        var buttons = new ComponentBuilder()
            .WithButton("Katıl", WebPoolModule.JoinPrefix + alert.ListingId, ButtonStyle.Success, new Emoji("✋"))
            .WithButton("İlanı aç", url: listingUrl, style: ButtonStyle.Link)
            .Build();
        return SendAsync(discordUserId, embed, buttons);
    }

    public Task SendTextAsync(ulong discordUserId, string title, string body, string? url, CancellationToken ct = default)
    {
        var embed = new EmbedBuilder().WithTitle(title).WithDescription(body).WithColor(new Color(0xC8AA6E)).Build();
        var buttons = url is null ? null : new ComponentBuilder().WithButton("İlanı aç", url: url, style: ButtonStyle.Link).Build();
        return SendAsync(discordUserId, embed, buttons);
    }

    private async Task SendAsync(ulong discordUserId, Embed embed, MessageComponent? components)
    {
        if (client.ConnectionState != ConnectionState.Connected)
        {
            return;
        }

        try
        {
            var user = await client.GetUserAsync(discordUserId);
            if (user is null)
            {
                return;
            }

            await user.SendMessageAsync(embed: embed, components: components);
            await accounts.SetDiscordDmClosedAsync(discordUserId, closed: false); // works again (after a retry)
        }
        catch (HttpException ex) when (ex.DiscordCode == DiscordErrorCode.CannotSendMessageToUser)
        {
            // DMs closed (privacy setting) or no shared server: skip them for a week.
            await accounts.SetDiscordDmClosedAsync(discordUserId, closed: true);
            logger.LogInformation("Discord user {DiscordUserId} does not accept DMs", discordUserId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Discord DM to {DiscordUserId} failed", discordUserId);
        }
    }
}

/// <summary>Delivers notification waves of website listings to Discord users as DMs.</summary>
public sealed class DiscordDmChannel(IDirectMessenger messenger, IConfiguration config) : INotificationChannel
{
    public async Task SendAsync(WaveAlert alert, CancellationToken ct)
    {
        if (alert.CommunityId != Communities.Web)
        {
            return;
        }

        var url = $"{(config["PublicUrl"] ?? "http://localhost:5215").TrimEnd('/')}/ilan/{alert.ListingId}";
        foreach (var discordId in (await messenger.ReachableWithoutPushAsync(alert.UserIds, ct)).Values)
        {
            await messenger.SendListingAsync(discordId, alert, url, ct);
        }
    }
}

/// <summary>The website's pool for the Discord commands (see <see cref="IWebPool"/>).</summary>
public sealed class WebPoolForDiscord(
    PoolActions actions,
    PreService pre,
    AccountService accounts,
    LiveUpdates live,
    ModerationService moderation,
    IConfiguration config) : IWebPool
{
    private const ulong Web = Communities.Web;

    public async Task<ulong> AccountForDiscordUserAsync(ulong discordUserId, string displayName)
    {
        var name = AccountService.CleanName(displayName);
        var account = await accounts.GetOrCreateAsync(ExternalLogin.DiscordProvider, discordUserId.ToString(), name.Length >= 2 ? name : "Oyuncu");
        return account.UserId;
    }

    public Task<Availability> SetAvailableAsync(ulong userId, Role role, Rank rank, TimeSpan duration) =>
        actions.SetAvailableAsync(userId, role, rank, duration);

    public Task ClearAvailableAsync(ulong userId) => actions.ClearAvailableAsync(userId);

    public Task<PoolSummary> GetPoolSummaryAsync() => pre.GetPoolSummaryAsync(Web);

    public Task<CreateListingResult> CreateListingAsync(
        ulong hostUserId, Role role, Rank min, Rank max, int slots, string? note, bool withLobby, bool lobbyLocked, ulong announceChannelId) =>
        actions.CreateListingAsync(hostUserId, role, min, max, slots, note, withLobby, lobbyLocked, announceChannelId);

    public Task<ClaimOutcome> ClaimAsync(int listingId, ulong userId) => actions.ClaimAsync(listingId, userId);

    public Task<LeaveResult> LeaveAsync(int listingId, ulong userId) => actions.LeaveAsync(listingId, userId);

    public async Task<ListingView?> GetListingAsync(int listingId) =>
        await pre.GetListingViewAsync(listingId) is { Listing.GuildId: Web } view ? view : null;

    public Task<string?> GetRiotIdAsync(ulong userId) => pre.GetRiotIdAsync(userId);

    public async Task<bool> SetRiotIdAsync(ulong userId, string riotId)
    {
        var ok = await pre.SetRiotIdAsync(userId, riotId);
        live.PublishChanged();
        return ok;
    }

    public async Task DeleteAccountAsync(ulong userId)
    {
        await accounts.DeleteAccountAsync(userId);
        live.PublishChanged();
    }

    public Task<bool> IsBannedAsync(ulong userId) => moderation.IsBannedAsync(userId);

    public string ListingUrl(int listingId) => $"{(config["PublicUrl"] ?? "http://localhost:5215").TrimEnd('/')}/ilan/{listingId}";
}
