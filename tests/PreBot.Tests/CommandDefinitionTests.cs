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

    private async Task<InteractionService> BuildAsync()
    {
        var client = new DiscordSocketClient();
        var provider = new ServiceCollection()
            .AddLogging()
            .AddSingleton(client)
            .AddSingleton(_db.Service)
            .AddSingleton(_db.Settings)
            .AddSingleton<ListingNotifier>()
            .BuildServiceProvider();
        var service = new InteractionService(client);
        await service.AddModulesAsync(typeof(PreModule).Assembly, provider);
        return service;
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
