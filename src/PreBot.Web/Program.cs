using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
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

// One JSON object per log line in production: easy to search and to ship to a log tool later.
if (!builder.Environment.IsDevelopment())
{
    builder.Logging.AddJsonConsole(o =>
    {
        o.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        o.UseUtcTimestamp = true;
    });
}

// ---------- Data folder ----------
// Database and push keys live outside the source tree. Locally that is %LOCALAPPDATA%\PreBot, because
// synced folders (OneDrive, Dropbox) lock SQLite's -wal/-shm files and must never upload the private key.
var dataDirectory = builder.Configuration["DataDirectory"]
    ?? (builder.Environment.IsDevelopment()
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PreBot")
        : builder.Environment.ContentRootPath);
Directory.CreateDirectory(dataDirectory);
builder.Services.AddSingleton(new DataDirectory(dataDirectory));
builder.Services.AddSingleton(PreBot.Web.SiteInfo.From(builder.Configuration));

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
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ActionLimiter>();
builder.Services.AddSingleton<PreBot.Web.Auth.LoginFailures>();
builder.Services.AddSingleton<ModerationService>();

// ---------- Live updates & notifications ----------
builder.Services.AddSingleton<LiveUpdates>();
builder.Services.AddSingleton<SubscriptionService>();
builder.Services.AddSingleton<VapidKeys>();
builder.Services.AddHttpClient("webpush", c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddSingleton<JoinTokens>();
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

    // One pool for the site and Discord: slash commands use the site's pool and accounts, and players who
    // joined through Discord (and have no push device) get notifications and host updates by DM.
    builder.Services.AddSingleton<IWebPool, WebPoolForDiscord>();
    builder.Services.AddSingleton<IDirectMessenger, DiscordDirectMessenger>();
    builder.Services.AddSingleton<INotificationChannel, DiscordDmChannel>();
}
else
{
    builder.Services.AddSingleton<IListingFeed, NoListingFeed>();
    builder.Services.AddSingleton<IDirectMessenger, NoDirectMessenger>();
}
builder.Services.AddHostedService<PoolWorker>();

// Nightly backup at 05:00 Turkey time (database + cookie keys + push key). Off in Development.
builder.Services.AddHttpClient("heartbeat", c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddSingleton<PreBot.Web.Ops.BackupRunner>();
if (builder.Configuration.GetValue("Backup:Enabled", !builder.Environment.IsDevelopment()))
{
    builder.Services.AddHostedService<PreBot.Web.Ops.BackupScheduler>();
}

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
        // A 30-day cookie must not outlive a ban or a deleted account: check the account (cached for a minute).
        o.Events.OnValidatePrincipal = async context =>
        {
            if (context.Principal?.GetUserId() is not { } userId)
            {
                return;
            }

            var services = context.HttpContext.RequestServices;
            var ok = await services.GetRequiredService<IMemoryCache>()
                .GetOrCreateAsync(("account-ok", userId), async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                    var account = await services.GetRequiredService<AccountService>().FindAsync(userId);
                    return account is { BannedAtUtc: null };
                });
            if (!ok)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        };
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

// ---------- Reverse proxy (Caddy) ----------
// Trust X-Forwarded-For/Proto only from the proxy's own network, so the client IP (used by rate limits and
// logs) and https scheme are real and cannot be faked by a header. ReverseProxy:KnownNetworks is a comma
// separated list of CIDRs, e.g. the fixed Docker network in docker-compose.yml. Loopback is always trusted.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var cidr in (builder.Configuration["ReverseProxy:KnownNetworks"] ?? "")
                 .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        o.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(cidr));
    }
});

// ---------- HTTP rate limits (per client IP) ----------
// Generous on purpose: Turkish mobile carriers put many players behind one IP (CGNAT).
// Button clicks inside live pages don't pass through here; ActionLimiter covers those per user.
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.OnRejected = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("RateLimit")
            .LogWarning("Rate limit hit: {Path} from {Ip}", context.HttpContext.Request.Path, context.HttpContext.Connection.RemoteIpAddress);
        return ValueTask.CompletedTask;
    };
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
        RateLimitPartition.GetFixedWindowLimiter(
            http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 600, Window = TimeSpan.FromMinutes(1) }));
    o.AddPolicy("auth", http =>
        RateLimitPartition.GetFixedWindowLimiter(
            http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions { PermitLimit = 20, Window = TimeSpan.FromMinutes(1) }));
});

// /health: used by Docker to know the site is alive and the database answers.
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);

var app = builder.Build();

app.UseForwardedHeaders();

// Behind a tunnel that terminates HTTPS but may not send X-Forwarded-Proto (e.g. Tailscale Funnel), the app
// still has to build https:// links (share links, notification links, secure cookies). Only for such setups.
if (app.Configuration.GetValue("ReverseProxy:AssumeHttps", false))
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
}

// Security headers. The CSP is report-only for now: violations show in the browser console without
// breaking anything; switch to enforcing once a week of real use shows none (HSTS is set by Caddy).
// Known report today: Blazor's inline <script type="importmap"> needs its sha256 hash in script-src first.
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers.XContentTypeOptions = "nosniff";
    headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    headers["Content-Security-Policy-Report-Only"] =
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; " +
        "font-src 'self'; connect-src 'self' ws: wss:; object-src 'none'; base-uri 'self'; " +
        "form-action 'self' https://accounts.google.com https://discord.com; frame-ancestors 'none'";
    await next();
});

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

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapHealthChecks("/health");                                                     // database included
app.MapHealthChecks("/health/live", new() { Predicate = _ => false });             // process is up
app.MapHealthChecks("/health/ready", new() { Predicate = c => c.Tags.Contains("ready") });
app.MapAuthEndpoints();

// "1 saat sustur" button on a notification: the service worker posts the personal token from the payload.
app.MapPost("/api/sustur", async (string t, JoinTokens tokens, PreService pre, AccountService accounts, TimeProvider time) =>
{
    if (tokens.MuteUser(t) is not { } userId)
    {
        return Results.BadRequest();
    }

    // An old notification of a deleted account: nothing to mute, and nothing must be created again.
    if (await accounts.FindAsync(userId) is null)
    {
        return Results.NoContent();
    }

    await pre.MuteAsync(userId, time.GetUtcNow().UtcDateTime.AddHours(1));
    return Results.NoContent();
}).RequireRateLimiting("auth").DisableAntiforgery();

// Listing notification buttons: "opened" (trial measurement) and "Maçtayım, sonra". The service worker posts the
// personal join token from the payload, so only the player it was sent to can act, and only on themselves.
app.MapPost("/api/bildirim/{eylem}", async (string eylem, string t, JoinTokens tokens, PreService pre, AccountService accounts, LiveUpdates live) =>
{
    if (tokens.Read(t) is not { } claim)
    {
        return Results.BadRequest();
    }

    if (await accounts.FindAsync(claim.UserId) is null)
    {
        return Results.NoContent(); // a deleted account: nothing to record or create
    }

    switch (eylem)
    {
        case "acildi":
            await pre.RecordNotificationOpenedAsync(claim.ListingId, claim.UserId);
            break;
        case "oyundayim":
            await pre.ReportInGameAsync(claim.UserId, claim.ListingId);
            live.PublishChanged();
            break;
        default:
            return Results.BadRequest();
    }

    return Results.NoContent();
}).DisableAntiforgery();

// "op.gg" next to a joiner: counted (how often hosts check a stranger before inviting), then off to the stats
// site. Only for people in that listing, and the address is built here from the joiner's Riot ID.
app.MapGet("/git/opgg/{listingId:int}/{userId}", async (int listingId, ulong userId, HttpContext http, PreService pre) =>
{
    var view = await pre.GetListingViewAsync(listingId);
    var me = http.User.GetUserId();
    if (view is null || me is null || (view.Listing.HostUserId != me && view.ActiveSlots.All(s => s.UserId != me)))
    {
        return Results.NotFound();
    }

    if (PreBot.Core.Domain.ProfileLinks.OpGg(view.ActiveSlots.FirstOrDefault(s => s.UserId == userId)?.RiotId) is not { } url)
    {
        return Results.NotFound();
    }

    await pre.RecordAsync(PreBot.Core.Domain.MetricKind.ProfileOpened, listingId);
    return Results.Redirect(url);
}).RequireAuthorization();

// Riot Developer Portal proves domain ownership with a text file at the site root (production key
// application). Set Riot:VerificationCode to the code shown in the portal; nothing is served otherwise.
if (app.Configuration["Riot:VerificationCode"] is { Length: > 0 } riotCode)
{
    app.MapGet(app.Configuration["Riot:VerificationPath"] ?? "/riot.txt", () => Results.Text(riotCode));
}
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(o => o.ContentSecurityFrameAncestorsPolicy = "'none'");

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
