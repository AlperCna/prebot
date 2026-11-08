using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PreBot.Bot;
using PreBot.Core.Data;
using PreBot.Core.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<DiscordOptions>(builder.Configuration.GetSection("Discord"));
builder.Services.AddSingleton(builder.Configuration.GetSection("PreBot").Get<PreBotOptions>() ?? new PreBotOptions());

// Locally the database lives in %LOCALAPPDATA%\PreBot: synced folders (OneDrive, Dropbox) break SQLite's lock files.
var dataDirectory = builder.Configuration["DataDirectory"]
    ?? (builder.Environment.IsDevelopment()
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PreBot")
        : builder.Environment.ContentRootPath);
Directory.CreateDirectory(dataDirectory);
var connectionString = builder.Configuration.GetConnectionString("PreBot")
    ?? $"Data Source={Path.Combine(dataDirectory, "prebot-discord.db")}";
builder.Services.AddDbContextFactory<PreBotDbContext>(o => o.UseSqlite(connectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PreService>();

// Only the Guilds intent: slash commands and buttons need no privileged intents.
builder.Services.AddSingleton(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.Guilds,
    LogLevel = LogSeverity.Info,
});
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(sp => new InteractionService(
    sp.GetRequiredService<DiscordSocketClient>(),
    new InteractionServiceConfig
    {
        // Handlers run off the gateway thread so a slow DB call never blocks Discord events.
        DefaultRunMode = RunMode.Async,
        LogLevel = LogSeverity.Info,
        UseCompiledLambda = true,
    }));
builder.Services.AddSingleton<ListingNotifier>();

builder.Services.AddHostedService<DiscordBotService>();
builder.Services.AddHostedService<ExpiryWorker>();

var host = builder.Build();

// Create or upgrade the database schema (EF Core migrations), then enable WAL.
await DatabaseSetup.MigrateAsync(host.Services.GetRequiredService<IDbContextFactory<PreBotDbContext>>());

await host.RunAsync();
