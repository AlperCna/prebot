using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using PreBot.Core.Services;
using PreBot.Web.Auth;
using PreBot.Web.Push;

namespace PreBot.Web.Tests;

/// <summary>Google / Discord login, with fake client ids (no real provider is contacted).</summary>
public sealed class ExternalLoginTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"prebot-oauth-test-{Guid.NewGuid():N}.db");
    private readonly List<WebApplicationFactory<Program>> _factories = [];

    public void Dispose()
    {
        foreach (var f in _factories)
        {
            f.Dispose();
        }

        SqliteConnection.ClearAllPools();
        foreach (var file in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
        {
            try { File.Delete(file); } catch (IOException) { }
        }
    }

    private WebApplicationFactory<Program> Factory(bool configured, string environment = "Production")
    {
        var (pub, priv) = VapidKeys.Generate();
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environment);
            // Tests never use local user-secrets: no real Discord bot, no real Google keys.
            b.UseSetting("Discord:Token", "");
            b.UseSetting("Discord:Enabled", "false");
            b.UseSetting("Database:JournalMode", "MEMORY");
            b.UseSetting("Authentication:Google:ClientId", "");
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
            b.UseSetting("DataDirectory", Path.GetTempPath());
            b.UseSetting("Push:PublicKey", pub);
            b.UseSetting("Push:PrivateKey", priv);
            if (configured)
            {
                b.UseSetting("Authentication:Google:ClientId", "test-google-id");
                b.UseSetting("Authentication:Google:ClientSecret", "test-google-secret");
                b.UseSetting("Authentication:Discord:ClientId", "123456789");
                b.UseSetting("Authentication:Discord:ClientSecret", "test-discord-secret");
            }
        });
        _factories.Add(factory);
        return factory;
    }

    private static HttpClient NoRedirect(WebApplicationFactory<Program> f) =>
        f.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    [Fact]
    public async Task Google_login_redirects_to_google_without_asking_for_email()
    {
        var response = await NoRedirect(Factory(configured: true)).GetAsync("/auth/login/google?returnUrl=%2Filan%2F5&kabul=1");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = Uri.UnescapeDataString(response.Headers.Location!.ToString());
        Assert.StartsWith("https://accounts.google.com/o/oauth2/v2/auth", location);
        Assert.Contains("client_id=test-google-id", location);
        Assert.Contains("redirect_uri=http://localhost/signin-google", location);
        Assert.Contains("scope=openid profile", location);
        Assert.DoesNotContain("email", location);
    }

    [Fact]
    public async Task Discord_login_redirects_to_discord_with_identify_only()
    {
        var response = await NoRedirect(Factory(configured: true)).GetAsync("/auth/login/discord?kabul=1");

        var location = Uri.UnescapeDataString(response.Headers.Location!.ToString());
        Assert.StartsWith("https://discord.com/", location);
        Assert.Contains("/oauth2/authorize", location);
        Assert.Contains("client_id=123456789", location);
        Assert.Contains("redirect_uri=http://localhost/signin-discord", location);
        Assert.Contains("scope=identify", location);
        Assert.DoesNotContain("email", location);
    }

    [Fact]
    public async Task Login_page_shows_only_configured_providers()
    {
        var withProviders = await NoRedirect(Factory(configured: true)).GetStringAsync("/giris");
        Assert.Contains("Google ile giriş", withProviders);
        Assert.Contains("Discord ile giriş", withProviders);
        Assert.Contains("VPN", withProviders);

        var without = await NoRedirect(Factory(configured: false)).GetStringAsync("/giris");
        Assert.DoesNotContain("Google ile giriş", without);
        Assert.Contains("Giriş henüz yapılandırılmadı", without);
    }

    [Fact]
    public async Task Login_requires_ticking_the_terms_box_first()
    {
        var response = await NoRedirect(Factory(configured: true)).GetAsync("/auth/login/google?returnUrl=%2Filan%2F5");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/giris?hata=kabul&returnUrl=%2Filan%2F5", response.Headers.Location!.ToString());

        var page = await NoRedirect(Factory(configured: true)).GetStringAsync("/giris");
        Assert.Contains("name=\"kabul\"", page);
        Assert.Contains("16 yaşından büyüğüm", page);
    }

    [Fact]
    public async Task Unknown_or_unconfigured_provider_is_404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await NoRedirect(Factory(configured: true)).GetAsync("/auth/login/facebook")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await NoRedirect(Factory(configured: false)).GetAsync("/auth/login/google")).StatusCode);
    }

    [Fact]
    public async Task Callback_without_a_confirmed_identity_goes_back_to_login()
    {
        var response = await NoRedirect(Factory(configured: true)).GetAsync("/auth/callback?provider=google&returnUrl=%2Fhavuz");

        Assert.Equal("/giris?hata=dis", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Confirmed_identity_creates_one_account_per_provider_user_and_signs_in()
    {
        var factory = Factory(configured: true);
        NoRedirect(factory); // start the host
        using var scope = factory.Services.CreateScope();
        var accounts = scope.ServiceProvider.GetRequiredService<AccountService>();

        async Task<(HttpContext Http, int Status, string? Location)> CompleteAsync(params Claim[] claims)
        {
            var http = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Google"));
            var result = await ExternalLogin.CompleteAsync(http, principal, "google", accounts, "/ilan/9");
            await result.ExecuteAsync(http);
            return (http, http.Response.StatusCode, http.Response.Headers.Location.ToString());
        }

        var first = await CompleteAsync(new Claim(ClaimTypes.NameIdentifier, "google-123"), new Claim(ClaimTypes.GivenName, "Alper"));
        var again = await CompleteAsync(new Claim(ClaimTypes.NameIdentifier, "google-123"), new Claim(ClaimTypes.GivenName, "Alper"));

        Assert.Equal(302, first.Status);
        Assert.Equal("/ilan/9", first.Location);
        Assert.Contains("prebot.auth=", first.Http.Response.Headers.SetCookie.ToString());

        var account = await accounts.GetOrCreateAsync("google", "google-123", "ignored");
        Assert.Equal("Alper", account.DisplayName);
        Assert.Equal("/ilan/9", again.Location);

        // Same id at a different provider is a different person.
        var discordUser = await accounts.GetOrCreateAsync("discord", "google-123", "Başkası");
        Assert.NotEqual(account.Id, discordUser.Id);
    }

    [Fact]
    public async Task Identity_without_id_is_rejected()
    {
        var factory = Factory(configured: true);
        NoRedirect(factory);
        using var scope = factory.Services.CreateScope();
        var http = new DefaultHttpContext { RequestServices = scope.ServiceProvider };

        var result = await ExternalLogin.CompleteAsync(
            http, new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "x")], "Google")),
            "google", scope.ServiceProvider.GetRequiredService<AccountService>(), "/havuz");
        await result.ExecuteAsync(http);

        Assert.Equal("/giris?hata=dis", http.Response.Headers.Location.ToString());
    }
}
