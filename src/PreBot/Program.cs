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

var connectionString = builder.Configuration.GetConnectionString("PreBot") ?? "Data Source=prebot.db";
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

// MVP: create the SQLite schema on first run. Switch to EF migrations before changing the schema in production.
await using (var db = await host.Services.GetRequiredService<IDbContextFactory<PreBotDbContext>>().CreateDbContextAsync())
{
    await db.Database.EnsureCreatedAsync();
}

await host.RunAsync();
