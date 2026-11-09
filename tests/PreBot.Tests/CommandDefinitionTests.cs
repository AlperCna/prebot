using System.Text.RegularExpressions;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using PreBot.Bot;

namespace PreBot.Tests;

/// <summary>Builds the slash command definitions offline, the same way the bot does on startup.</summary>
public sealed class CommandDefinitionTests : IDisposable
{
    private static readonly Regex DiscordName = new(@"^[-_\p{Ll}\p{N}]{1,32}$");
    private readonly TestDb _db = new();

    public void Dispose() => _db.Dispose();

    private async Task<InteractionService> BuildAsync(bool webPool = false)
    {
        var client = new DiscordSocketClient();
        var provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(client)
            .AddSingleton(_db.Service)
            .AddSingleton(_db.Settings)
            .AddSingleton<ListingNotifier>()
            .AddSingleton<IWebPool>(new NoWebPool())
            .BuildServiceProvider();
        var service = new InteractionService(client);
        // Same choice the bot makes on startup: exactly one of the two modules.
        if (webPool)
        {
            await service.AddModuleAsync<WebPoolModule>(provider);
        }
        else
        {
            await service.AddModuleAsync<PreModule>(provider);
        }

        return service;
    }

    [Fact]
    public async Task Website_pool_module_has_the_same_commands_and_a_join_button()
    {
        using var standalone = await BuildAsync();
        using var web = await BuildAsync(webPool: true);

        Assert.Equal(
            standalone.SlashCommands.Select(c => c.Name).Order(),
            web.SlashCommands.Select(c => c.Name).Order());
        Assert.Contains(WebPoolModule.JoinPrefix + "*", web.ComponentCommands.Select(c => c.Name));
        foreach (var command in web.SlashCommands)
        {
            Assert.Matches(DiscordName, command.Name);
            Assert.InRange(command.Description.Length, 1, 100);
            foreach (var p in command.Parameters)
            {
                Assert.Matches(DiscordName, p.Name);
                Assert.InRange(p.Description.Length, 1, 100);
                Assert.True(p.Choices.Count <= 25);
            }
        }
    }

    /// <summary>Only needed so the module can be built; never called.</summary>
    private sealed class NoWebPool : IWebPool
    {
        public Task<ulong> AccountForDiscordUserAsync(ulong discordUserId, string displayName) => throw new NotSupportedException();
        public Task<PreBot.Core.Domain.Availability> SetAvailableAsync(ulong userId, PreBot.Core.Domain.Role role, PreBot.Core.Domain.Rank rank, TimeSpan duration) => throw new NotSupportedException();
        public Task ClearAvailableAsync(ulong userId) => throw new NotSupportedException();
        public Task<PreBot.Core.Services.PoolSummary> GetPoolSummaryAsync() => throw new NotSupportedException();
        public Task<PreBot.Core.Services.CreateListingResult> CreateListingAsync(ulong hostUserId, PreBot.Core.Domain.Role role, PreBot.Core.Domain.Rank min, PreBot.Core.Domain.Rank max, int slots, string? note, ulong announceChannelId) => throw new NotSupportedException();
        public Task<PreBot.Core.Services.ClaimOutcome> ClaimAsync(int listingId, ulong userId) => throw new NotSupportedException();
        public Task<PreBot.Core.Domain.LeaveResult> LeaveAsync(int listingId, ulong userId) => throw new NotSupportedException();
        public Task<PreBot.Core.Services.ListingView?> GetListingAsync(int listingId) => throw new NotSupportedException();
        public Task<string?> GetRiotIdAsync(ulong userId) => throw new NotSupportedException();
        public Task<bool> SetRiotIdAsync(ulong userId, string riotId) => throw new NotSupportedException();
        public Task DeleteAccountAsync(ulong userId) => throw new NotSupportedException();
        public string ListingUrl(int listingId) => "";
    }

    [Fact]
    public async Task All_commands_are_registered_with_valid_names()
    {
        using var service = await BuildAsync();

        var names = service.SlashCommands.Select(c => c.Name).Order().ToArray();

        Assert.Equal(
            ["gizlilik", "havuz", "musait-degilim", "musaitim", "pre-lazim", "profil", "verilerimi-sil"],
            names);
        foreach (var command in service.SlashCommands)
        {
            Assert.Matches(DiscordName, command.Name);
            Assert.InRange(command.Description.Length, 1, 100);
            foreach (var p in command.Parameters)
            {
                Assert.Matches(DiscordName, p.Name);
                Assert.InRange(p.Description.Length, 1, 100);
                Assert.True(p.Choices.Count <= 25);
            }
        }
    }

    [Fact]
    public async Task Listing_buttons_have_handlers()
    {
        using var service = await BuildAsync();

        var patterns = service.ComponentCommands.Select(c => c.Name).ToHashSet();

        Assert.Contains(ListingRenderer.JoinPrefix + "*", patterns);
        Assert.Contains(ListingRenderer.LeavePrefix + "*", patterns);
        Assert.Contains(ListingRenderer.NoShowPrefix + "*", patterns);
        Assert.Contains(ListingRenderer.CancelPrefix + "*", patterns);
    }
}
