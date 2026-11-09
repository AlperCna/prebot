using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace PreBot.Web.Tests;

/// <summary>Runs the real web app in memory (real SQLite file) and talks to it over HTTP.</summary>
public sealed partial class WebAppTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"prebot-web-test-{Guid.NewGuid():N}.db");
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

    private HttpClient Client(string environment = "Development")
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment(environment);
            // Tests never use local user-secrets: no real Discord bot, no real Google keys.
            b.UseSetting("Discord:Token", "");
            b.UseSetting("Discord:Enabled", "false");
            b.UseSetting("Database:JournalMode", "MEMORY");
            b.UseSetting("Authentication:Google:ClientId", "");
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
            b.UseSetting("Push:PublicKey", TestKeys.PublicKey);
            b.UseSetting("Push:PrivateKey", TestKeys.PrivateKey);
        });
        _factories.Add(factory);
        return factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryField();

    /// <summary>Signs in through the real login form (antiforgery token included). Returns the redirect response.</summary>
    private static async Task<HttpResponseMessage> DevLoginAsync(HttpClient client, string name, string? returnUrl = null)
    {
        var page = await client.GetStringAsync("/giris");
        var token = AntiforgeryField().Match(page).Groups[1].Value;
        var form = new Dictionary<string, string> { ["name"] = name, ["__RequestVerificationToken"] = token };
        if (returnUrl is not null)
        {
            form["returnUrl"] = returnUrl;
        }

        return await client.PostAsync("/auth/dev-login", new FormUrlEncodedContent(form));
    }

    [Theory]
    [InlineData("/", "5. kişiyi dakikalar içinde bulun")]
    [InlineData("/giris", "Giriş yap")]
    [InlineData("/gizlilik", "KVKK")]
    [InlineData("/kosullar", "Kullanım Koşulları")]
    [InlineData("/health", "Healthy")]
    [InlineData("/istatistik", "İşe yarıyor mu?")]
    [InlineData("/iletisim", "Bir ilanı ya da oyuncuyu şikâyet etmek")]
    public async Task Public_pages_load(string path, string expectedText)
    {
        var response = await Client().GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expectedText, await response.Content.ReadAsStringAsync());
    }

    private static readonly (string PublicKey, string PrivateKey) TestKeys = PreBot.Web.Push.VapidKeys.Generate();

    [Fact]
    public async Task Pages_send_security_headers_and_health_endpoints_split_live_and_ready()
    {
        var client = Client();

        var home = await client.GetAsync("/");
        Assert.Equal("nosniff", home.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Contains("frame-ancestors 'none'", home.Headers.GetValues("Content-Security-Policy-Report-Only").Single());

        Assert.Equal("Healthy", await client.GetStringAsync("/health/live"));
        Assert.Equal("Healthy", await client.GetStringAsync("/health/ready"));
    }

    [Fact]
    public async Task Logout_needs_a_post_with_antiforgery_and_get_cannot_log_out_a_live_account()
    {
        var client = Client();
        await DevLoginAsync(client, "Çıkışçı");

        // A link or image on another site (GET) must not log the user out.
        var get = await client.GetAsync("/auth/logout");
        Assert.Equal("/profil", get.Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/profil")).StatusCode);

        // A POST without the antiforgery token is refused.
        var forged = await client.PostAsync("/auth/logout", new FormUrlEncodedContent([]));
        Assert.Equal(HttpStatusCode.BadRequest, forged.StatusCode);

        var page = await client.GetStringAsync("/giris");
        var token = AntiforgeryField().Match(page).Groups[1].Value;
        var real = await client.PostAsync("/auth/logout",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token }));
        Assert.Equal("/", real.Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/profil")).StatusCode); // back to login
    }

    [Theory]
    [InlineData("/manifest.webmanifest", "standalone")]
    [InlineData("/sw.js", "notificationclick")]
    [InlineData("/js/prebot.js", "pushManager")]
    public async Task Pwa_files_are_served(string path, string expectedText)
    {
        var response = await Client().GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expectedText, await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("/icons/icon-192.png")]
    [InlineData("/icons/icon-512.png")]
    [InlineData("/icons/icon-maskable-512.png")]
    [InlineData("/icons/badge-72.png")]
    public async Task Pwa_icons_are_png(string path)
    {
        var response = await Client().GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void Generated_vapid_keys_have_the_right_shape()
    {
        var (publicKey, privateKey) = PreBot.Web.Push.VapidKeys.Generate();

        var pub = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(publicKey);
        var priv = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(privateKey);
        Assert.Equal(65, pub.Length);   // uncompressed P-256 point
        Assert.Equal(0x04, pub[0]);
        Assert.Equal(32, priv.Length);
    }

    [Fact]
    public async Task Login_endpoints_are_rate_limited_per_ip()
    {
        var client = Client();

        for (var i = 0; i < 20; i++)
        {
            var ok = await client.GetAsync("/auth/login/google");
            Assert.NotEqual(HttpStatusCode.TooManyRequests, ok.StatusCode);
        }

        var limited = await client.GetAsync("/auth/login/google");
        Assert.Equal(HttpStatusCode.TooManyRequests, limited.StatusCode);
        Assert.True(limited.Headers.Contains("Retry-After"));

        // Normal pages are not affected by the login limit.
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/")).StatusCode);
    }

    [Fact]
    public async Task Unknown_page_shows_turkish_not_found()
    {
        var response = await Client().GetAsync("/boyle-bir-sayfa-yok");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("Sayfa bulunamadı", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("/havuz")]
    [InlineData("/ilan-ac")]
    [InlineData("/ilan/1")]
    [InlineData("/profil")]
    [InlineData("/bildirimler")]
    public async Task Signed_in_pages_redirect_to_login_and_remember_where_you_were(string path)
    {
        var response = await Client().GetAsync(path);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location!.ToString();
        Assert.Contains("/giris", location);
        Assert.Contains(Uri.EscapeDataString(path), location);
    }

    [Fact]
    public async Task Dev_login_signs_in_and_returns_to_shared_link()
    {
        var client = Client();

        var login = await DevLoginAsync(client, "Alper", returnUrl: "/ilan/7");

        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        Assert.Equal("/ilan/7", login.Headers.Location!.ToString());
        Assert.Contains(login.Headers.GetValues("Set-Cookie"), c => c.StartsWith("prebot.auth="));

        // The cookie handler of the test client keeps the session.
        var pool = await client.GetAsync("/havuz");
        Assert.Equal(HttpStatusCode.OK, pool.StatusCode);
        var html = await pool.Content.ReadAsStringAsync();
        Assert.Contains("Havuz", html);
        Assert.Contains("Alper", html);
    }

    [Fact]
    public async Task Dev_login_blocks_open_redirects()
    {
        var login = await DevLoginAsync(Client(), "Alper", returnUrl: "//evil.example/steal");

        Assert.Equal("/havuz", login.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Dev_login_rejects_too_short_names()
    {
        var login = await DevLoginAsync(Client(), " x ");

        Assert.Equal("/giris?hata=isim", login.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Dev_login_requires_antiforgery_token()
    {
        var response = await Client().PostAsync("/auth/dev-login",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "Alper" }));

        Assert.False(response.Headers.TryGetValues("Set-Cookie", out var cookies)
            && cookies.Any(c => c.StartsWith("prebot.auth=")));
        Assert.NotEqual(HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task Dev_login_does_not_exist_in_production()
    {
        var client = Client("Production");

        var page = await client.GetStringAsync("/giris");
        Assert.DoesNotContain("/auth/dev-login", page);

        var response = await client.PostAsync("/auth/dev-login",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "Hacker" }));
        Assert.False(response.Headers.TryGetValues("Set-Cookie", out var cookies)
            && cookies.Any(c => c.StartsWith("prebot.auth=")));
    }

    [Fact]
    public async Task Logout_ends_the_session()
    {
        var client = Client();
        await DevLoginAsync(client, "Alper");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/havuz")).StatusCode);

        var token = AntiforgeryField().Match(await client.GetStringAsync("/giris")).Groups[1].Value;
        var logout = await client.PostAsync("/auth/logout",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token }));

        Assert.Equal("/", logout.Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/havuz")).StatusCode);
    }
}
