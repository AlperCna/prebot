using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PreBot.Bot;
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
builder.Services.AddDbContextFactory<PreBotDbContext>(o =>
    o.UsePreBotSqlite(connectionString, builder.Configuration["Database:JournalMode"] ?? "WAL"));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PreService>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<AnnouncementStore>();
builder.Services.AddSingleton<StatsService>();
builder.Services.AddSingleton<WaveDispatcher>();

// ---------- Live updates & notifications ----------
builder.Services.AddSingleton<LiveUpdates>();
builder.Services.AddSingleton<SubscriptionService>();
builder.Services.AddSingleton<VapidKeys>();
builder.Services.AddHttpClient("webpush", c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddSingleton<PushSender>();
builder.Services.AddSingleton<HostNotifier>();
builder.Services.AddSingleton<INotificationChannel, InAppChannel>();   // open pages
builder.Services.AddSingleton<INotificationChannel, WebPushChannel>(); // phones & PCs, even when the site is closed
builder.Services.AddSingleton<PoolActions>();

// Optional: host the Discord bot in this same process (one database, one writer). It keeps all its slash
// commands, and announces website listings in Discord:FeedChannelIds with a link back to the site.
if (DiscordSetup.IsConfigured(builder.Configuration))
{
    builder.Services.AddPreBotDiscord(builder.Configuration);
    builder.Services.AddSingleton<IListingFeed, DiscordListingFeed>();
}
else
{
    builder.Services.AddSingleton<IListingFeed, NoListingFeed>();
}
builder.Services.AddHostedService<PoolWorker>();

// ---------- Auth: cookie session; providers (Google, Discord) sign in through it ----------
var auth = builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/giris";
        o.Cookie.Name = "prebot.auth";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.ExpireTimeSpan = TimeSpan.FromDays(30);
        o.SlidingExpiration = true;
    })
    // Short-lived holding cookie between "Google/Discord said OK" and "PreBot account signed in".
    .AddCookie(ExternalLogin.Scheme, o =>
    {
        o.Cookie.Name = "prebot.external";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });
builder.Services.AddSingleton(ExternalLogin.AddProviders(auth, builder.Configuration));

// Cookie and antiforgery encryption keys must survive restarts, or everyone is logged out on each deploy.
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataDirectory, "keys")))
    .SetApplicationName("PreBot");
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Write Turkish letters (ş, ğ, ı, İ...) as-is instead of &#x15F; escapes: smaller, readable HTML.
builder.Services.Configure<Microsoft.Extensions.WebEncoders.WebEncoderOptions>(o =>
    o.TextEncoderSettings = new System.Text.Encodings.Web.TextEncoderSettings(System.Text.Unicode.UnicodeRanges.All));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// /health: used by Docker to know the site is alive and the database answers.
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

var app = builder.Build();

// Create or upgrade the database schema (EF Core migrations), then enable WAL.
await DatabaseSetup.MigrateAsync(
    app.Services.GetRequiredService<IDbContextFactory<PreBotDbContext>>(),
    app.Configuration["Database:JournalMode"] ?? "WAL");

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
app.MapHealthChecks("/health");
app.MapAuthEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

/// <summary>Exposed for integration tests (WebApplicationFactory).</summary>
public partial class Program;

/// <summary>Folder for the database and generated keys (never inside a synced or source folder).</summary>
public sealed record DataDirectory(string Path);

/// <summary>Healthy when the SQLite database can be opened and queried.</summary>
public sealed class DatabaseHealthCheck(IDbContextFactory<PreBotDbContext> factory)
    : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        return await db.Database.CanConnectAsync(ct)
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("database unavailable");
    }
}
