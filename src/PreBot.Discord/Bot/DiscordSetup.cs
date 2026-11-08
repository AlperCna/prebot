using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace PreBot.Bot;

public static class DiscordSetup
{
    /// <summary>True when a bot token is configured (Discord:Token).</summary>
    public static bool IsConfigured(IConfiguration config) => !string.IsNullOrWhiteSpace(config["Discord:Token"]);

    /// <summary>
    /// Registers the Discord bot: gateway client, slash commands and buttons, and the worker that
    /// expires Discord listings and sends their notification waves. Used by both the standalone bot
    /// and the website (when it hosts the bot).
    /// </summary>
    public static IServiceCollection AddPreBotDiscord(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<DiscordOptions>(config.GetSection("Discord"));

        // Only the Guilds intent: slash commands and buttons need no privileged intents.
        services.AddSingleton(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds,
            LogLevel = LogSeverity.Info,
        });
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton(sp => new InteractionService(
            sp.GetRequiredService<DiscordSocketClient>(),
            new InteractionServiceConfig
            {
                // Handlers run off the gateway thread so a slow DB call never blocks Discord events.
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info,
                UseCompiledLambda = true,
            }));
        services.AddSingleton<ListingNotifier>();
        services.AddSingleton<DiscordFeed>();

        services.AddHostedService<DiscordBotService>();
        services.AddHostedService<ExpiryWorker>();
        return services;
    }
}
