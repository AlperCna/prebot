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

    /// <summary>
    /// Channels where listings opened on the website are announced with a "Sitede katıl" link:
    /// comma-separated channel ids (Discord:FeedChannels = "123,456"). Empty = no announcements.
    /// A plain string so an empty value in .env never breaks startup.
    /// </summary>
    public string? FeedChannels { get; set; }

    public ulong[] FeedChannelIds =>
        (FeedChannels ?? "")
            .Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => ulong.TryParse(s, out var id) ? id : 0)
            .Where(id => id != 0)
            .Distinct()
            .ToArray();
}

/// <summary>How the bot is hosted: alone (a bad token stops the program) or inside the website (only the bot stops).</summary>
public sealed record DiscordHosting(bool Standalone);

/// <summary>Connects to Discord, registers slash commands, and routes interactions to <see cref="PreModule"/>.</summary>
public sealed class DiscordBotService(
    DiscordSocketClient client,
    InteractionService interactions,
    IServiceProvider services,
    IOptions<DiscordOptions> options,
    DiscordHosting hosting,
    IHostApplicationLifetime lifetime,
    ILogger<DiscordBotService> logger) : IHostedService
{
    private const string WrongTokenHelp =
        "Discord bot token'ı geçersiz. Developer Portal → PreBot → Bot → Reset Token ile aldığın değeri kaydet " +
        "(OAuth2 sayfasındaki 'Client Secret' değil): powershell -ExecutionPolicy Bypass -File .\\scripts\\token-ayarla.ps1 -Proje web";

    private bool _modulesLoaded;
    private int _stopped;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var token = options.Value.Token;
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Discord bot token'ı bulunamadı. Şunlardan biriyle ayarla:\n" +
                "  dotnet user-secrets set \"Discord:Token\" \"<TOKEN>\" --project src/PreBot\n" +
                "  veya ortam değişkeni: Discord__Token=<TOKEN>\n" +
                "  Docker'da: .env dosyasına DISCORD_BOT_TOKEN=<TOKEN>");
        }

        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, token);
        }
        catch (ArgumentException)
        {
            // Wrong kind of value (e.g. an OAuth client secret): don't even try to connect.
            Fail();
            return;
        }

        client.Log += LogAsync;
        client.Disconnected += OnDisconnectedAsync;
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

    /// <summary>Discord rejected the token (401): retrying forever only fills the log.</summary>
    private Task OnDisconnectedAsync(Exception? ex)
    {
        if (ex is Discord.Net.HttpException { HttpCode: System.Net.HttpStatusCode.Unauthorized })
        {
            Fail();
            _ = Task.Run(async () =>
            {
                try
                {
                    await client.StopAsync();
                }
                catch (Exception stopEx)
                {
                    logger.LogDebug(stopEx, "Stopping the Discord client failed");
                }
            });
        }

        return Task.CompletedTask;
    }

    private void Fail()
    {
        if (Interlocked.Exchange(ref _stopped, 1) == 1)
        {
            return;
        }

        if (hosting.Standalone)
        {
            logger.LogCritical("{Help}", WrongTokenHelp);
            lifetime.StopApplication();
        }
        else
        {
            logger.LogError("Discord botu kapatıldı, site çalışmaya devam ediyor. {Help}", WrongTokenHelp);
        }
    }

    private async Task OnReadyAsync()
    {
        // Ready fires again after reconnects; modules and commands only need to be set up once.
        if (_modulesLoaded)
        {
            return;
        }

        _modulesLoaded = true;

        // Inside the website the commands use the site's single pool; the standalone bot keeps per-server pools.
        // Both modules have the same command names, so exactly one of them is loaded.
        if (services.GetService(typeof(IWebPool)) is not null)
        {
            await interactions.AddModuleAsync<WebPoolModule>(services);
            logger.LogInformation("Discord commands use the website's pool (one pool for site and Discord)");
        }
        else
        {
            await interactions.AddModuleAsync<PreModule>(services);
        }

        // Helps setup: which servers the bot is in, and the ids of text channels it can post to
        // (for Discord:FeedChannels). Server names and channel ids are not secret.
        foreach (var guild in client.Guilds)
        {
            var channels = guild.TextChannels
                .Where(c => guild.CurrentUser.GetPermissions(c) is { SendMessages: true, EmbedLinks: true })
                .OrderBy(c => c.Position)
                .Select(c => $"#{c.Name}={c.Id}");
            logger.LogInformation("Server {Guild} ({GuildId}) – postable channels: {Channels}",
                guild.Name, guild.Id, string.Join(", ", channels));
        }

        var feed = options.Value.FeedChannelIds;
        if (feed.Length > 0)
        {
            logger.LogInformation("Website listings will be announced in {Count} channel(s): {Channels}", feed.Length, string.Join(", ", feed));
        }

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
