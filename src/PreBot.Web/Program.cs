using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Notifications;
using PreBot.Core.Services;
using PreBot.Web.Auth;
using PreBot.Web.Components;
using PreBot.Web.Live;
using PreBot.Web.Push;

var builder = WebApplication.CreateBuilder(args);

// ---------- Data folder ----------
// Database and push keys live outside the source tree. Locally that is %LOCALAPPDATA%\PreBot, because
// synced folders (OneDrive, Dropbox) lock SQLite's -wal/-shm files and must never upload the private key.
var dataDirectory = builder.Configuration["DataDirectory"]
    ?? (builder.Environment.IsDevelopment()
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PreBot")
        : builder.Environment.ContentRootPath);
Directory.CreateDirectory(dataDirectory);
builder.Services.AddSingleton(new DataDirectory(dataDirectory));

// ---------- Core (same library the Discord bot uses) ----------
builder.Services.AddSingleton(builder.Configuration.GetSection("PreBot").Get<PreBotOptions>() ?? new PreBotOptions());
var connectionString = builder.Configuration.GetConnectionString("PreBot")
    ?? $"Data Source={Path.Combine(dataDirectory, "prebot-web.db")}";
builder.Services.AddDbContextFactory<PreBotDbContext>(o => o.UseSqlite(connectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PreService>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<WaveDispatcher>();

// ---------- Live updates & notifications ----------
builder.Services.AddSingleton<LiveUpdates>();
builder.Services.AddSingleton<SubscriptionService>();
builder.Services.AddSingleton<VapidKeys>();
builder.Services.AddHttpClient("webpush", c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddSingleton<PushSender>();
builder.Services.AddSingleton<INotificationChannel, InAppChannel>();   // open pages
builder.Services.AddSingleton<INotificationChannel, WebPushChannel>(); // phones & PCs, even when the site is closed
builder.Services.AddSingleton<PoolActions>();
builder.Services.AddHostedService<PoolWorker>();

// ---------- Auth: cookie session; providers (Google, Discord) sign in through it ----------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/giris";
        o.Cookie.Name = "prebot.auth";
        o.ExpireTimeSpan = TimeSpan.FromDays(30);
        o.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// MVP: create the schema on first run (switch to EF migrations before changing it in production).
// WAL lets readers keep reading while the single writer writes.
await using (var db = await app.Services.GetRequiredService<IDbContextFactory<PreBotDbContext>>().CreateDbContextAsync())
{
    await db.Database.EnsureCreatedAsync();
    await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapAuthEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

/// <summary>Exposed for integration tests (WebApplicationFactory).</summary>
public partial class Program;

/// <summary>Folder for the database and generated keys (never inside a synced or source folder).</summary>
public sealed record DataDirectory(string Path);
