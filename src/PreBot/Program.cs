using Microsoft.EntityFrameworkCore;
using PreBot.Bot;
using PreBot.Core.Data;
using PreBot.Core.Services;

// Standalone Discord bot. (The website can also host the bot itself when Discord:Token is set.)
var builder = Host.CreateApplicationBuilder(args);

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

builder.Services.AddPreBotDiscord(builder.Configuration);

var host = builder.Build();

// Create or upgrade the database schema (EF Core migrations), then enable WAL.
await DatabaseSetup.MigrateAsync(host.Services.GetRequiredService<IDbContextFactory<PreBotDbContext>>());

await host.RunAsync();
