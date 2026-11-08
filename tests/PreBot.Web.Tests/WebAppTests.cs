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
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
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
    public async Task Public_pages_load(string path, string expectedText)
    {
        var response = await Client().GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(expectedText, await response.Content.ReadAsStringAsync());
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

        var logout = await client.GetAsync("/auth/logout");

        Assert.Equal("/", logout.Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/havuz")).StatusCode);
    }
}
