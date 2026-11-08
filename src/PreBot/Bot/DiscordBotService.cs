using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace PreBot.Bot;

public sealed class DiscordOptions
{
    /// <summary>Bot token. Keep it out of source control: use user-secrets or the Discord__Token env variable.</summary>
    public string? Token { get; set; }

    /// <summary>
    /// Optional test server id. When set, commands are registered to that server only and show up instantly.
    /// When empty, commands are registered globally (can take a while to appear).
    /// </summary>
    public ulong? GuildId { get; set; }
}

/// <summary>Connects to Discord, registers slash commands, and routes interactions to <see cref="PreModule"/>.</summary>
public sealed class DiscordBotService(
    DiscordSocketClient client,
    InteractionService interactions,
    IServiceProvider services,
    IOptions<DiscordOptions> options,
    ILogger<DiscordBotService> logger) : IHostedService
{
    private bool _modulesLoaded;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = options.Value.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Discord bot token'ı bulunamadı. Şunlardan biriyle ayarla:\n" +
                "  dotnet user-secrets set \"Discord:Token\" \"<TOKEN>\" --project src/PreBot\n" +
                "  veya ortam değişkeni: Discord__Token=<TOKEN>");
        }

        client.Log += LogAsync;
        interactions.Log += LogAsync;
        client.Ready += OnReadyAsync;
        client.InteractionCreated += OnInteractionAsync;
        interactions.InteractionExecuted += OnInteractionExecutedAsync;

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StopAsync();
        await client.LogoutAsync();
    }

    private async Task OnReadyAsync()
    {
        // Ready fires again after reconnects; modules and commands only need to be set up once.
        if (_modulesLoaded)
        {
            return;
        }

        _modulesLoaded = true;
        await interactions.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

        if (options.Value.GuildId is { } guildId)
        {
            await interactions.RegisterCommandsToGuildAsync(guildId);
            logger.LogInformation("Slash commands registered to test server {GuildId}", guildId);
        }
        else
        {
            await interactions.RegisterCommandsGloballyAsync();
            logger.LogInformation("Slash commands registered globally");
        }
    }

    private async Task OnInteractionAsync(SocketInteraction interaction)
    {
        var context = new SocketInteractionContext(client, interaction);
        await interactions.ExecuteCommandAsync(context, services);
    }

    private async Task OnInteractionExecutedAsync(ICommandInfo command, IInteractionContext context, IResult result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        logger.LogError("Interaction {Command} failed: {Error} {Reason}", command?.Name, result.Error, result.ErrorReason);
        const string text = "Bir şeyler ters gitti, birazdan tekrar dene.";
        try
        {
            if (context.Interaction.HasResponded)
            {
                await context.Interaction.FollowupAsync(text, ephemeral: true);
            }
            else
            {
                await context.Interaction.RespondAsync(text, ephemeral: true);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not send error reply");
        }
    }

    private Task LogAsync(LogMessage message)
    {
        var level = message.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Debug,
            _ => LogLevel.Trace,
        };
        logger.Log(level, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        return Task.CompletedTask;
    }
}
