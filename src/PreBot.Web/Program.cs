using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Notifications;
using PreBot.Core.Services;
using PreBot.Web.Auth;
using PreBot.Web.Components;
using PreBot.Web.Live;

var builder = WebApplication.CreateBuilder(args);

// ---------- Core (same library the Discord bot uses) ----------
builder.Services.AddSingleton(builder.Configuration.GetSection("PreBot").Get<PreBotOptions>() ?? new PreBotOptions());
var connectionString = builder.Configuration.GetConnectionString("PreBot") ?? "Data Source=prebot-web.db";
builder.Services.AddDbContextFactory<PreBotDbContext>(o => o.UseSqlite(connectionString));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PreService>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<WaveDispatcher>();

// ---------- Live updates & notifications ----------
builder.Services.AddSingleton<LiveUpdates>();
builder.Services.AddSingleton<INotificationChannel, InAppChannel>();
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
