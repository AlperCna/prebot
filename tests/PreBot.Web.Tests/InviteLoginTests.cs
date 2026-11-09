using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace PreBot.Web.Tests;

/// <summary>Closed test with friends: "name + invite code" login, only when Site:InviteCode is set.</summary>
public sealed partial class InviteLoginTests : IDisposable
{
    private static readonly (string PublicKey, string PrivateKey) TestKeys = PreBot.Web.Push.VapidKeys.Generate();
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"prebot-invite-test-{Guid.NewGuid():N}.db");
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

    private HttpClient Client(string? inviteCode)
    {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Production"); // no dev login here
            b.UseSetting("Discord:Token", "");
            b.UseSetting("Discord:Enabled", "false");
            b.UseSetting("Database:JournalMode", "MEMORY");
            b.UseSetting("Authentication:Google:ClientId", "");
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
            b.UseSetting("DataDirectory", Path.GetTempPath());
            b.UseSetting("Backup:Enabled", "false");
            b.UseSetting("Push:PublicKey", TestKeys.PublicKey);
            b.UseSetting("Push:PrivateKey", TestKeys.PrivateKey);
            b.UseSetting("Site:InviteCode", inviteCode ?? "");
        });
        _factories.Add(factory);
        return factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\" value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryField();

    private static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string name, string code, bool accept = true, string secret = "gizli-sifre")
    {
        var page = await client.GetStringAsync("/giris");
        var form = new Dictionary<string, string>
        {
            ["name"] = name,
            ["code"] = code,
            ["sifre"] = secret,
            ["returnUrl"] = "/havuz",
            ["__RequestVerificationToken"] = AntiforgeryField().Match(page).Groups[1].Value,
        };
        if (accept)
        {
            form["kabul"] = "1";
        }

        return await client.PostAsync("/auth/invite-login", new FormUrlEncodedContent(form));
    }

    [Fact]
    public async Task Right_code_signs_in_and_wrong_code_or_no_consent_does_not()
    {
        var client = Client("flex-2026");
        Assert.Contains("Davet kodu", await client.GetStringAsync("/giris"));

        Assert.Equal("/giris?hata=kod&returnUrl=%2Fhavuz", (await LoginAsync(client, "Mert", "yanlis")).Headers.Location!.ToString());
        Assert.Equal("/giris?hata=kabul&returnUrl=%2Fhavuz", (await LoginAsync(client, "Mert", "flex-2026", accept: false)).Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/havuz")).StatusCode); // still signed out

        var ok = await LoginAsync(client, "Mert", " flex-2026 ");
        Assert.Equal("/havuz", ok.Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/havuz")).StatusCode);
    }

    [Fact]
    public async Task Knowing_the_invite_code_is_not_enough_to_sign_in_as_someone_else()
    {
        var owner = Client("flex-2026");
        Assert.Equal("/havuz", (await LoginAsync(owner, "Alper", "flex-2026", secret: "alperin-sifresi")).Headers.Location!.ToString());

        // Another browser types the same name (any spelling) with a different password: refused.
        var other = _factories[^1].CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        Assert.Equal("/giris?hata=sifre&returnUrl=%2Fhavuz", (await LoginAsync(other, " alper ", "flex-2026", secret: "tahmin-123")).Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await other.GetAsync("/havuz")).StatusCode);

        // Too short a password is refused before anything is created.
        Assert.Equal("/giris?hata=sifrekisa&returnUrl=%2Fhavuz", (await LoginAsync(other, "Yeni", "flex-2026", secret: "123")).Headers.Location!.ToString());

        // The owner's own password still works from a new browser.
        Assert.Equal("/havuz", (await LoginAsync(other, "ALPER", "flex-2026", secret: "alperin-sifresi")).Headers.Location!.ToString());
    }

    [Fact]
    public async Task Guessing_the_code_or_a_password_is_stopped_after_a_few_tries()
    {
        var client = Client("flex-2026");
        for (var i = 0; i < PreBot.Web.Auth.LoginFailures.IpLimit; i++)
        {
            Assert.Equal("/giris?hata=kod&returnUrl=%2Fhavuz", (await LoginAsync(client, "Mert", $"flex-{i:0000}")).Headers.Location!.ToString());
        }

        // Even the right code is refused now, from this address.
        Assert.Equal("/giris?hata=cok&returnUrl=%2Fhavuz", (await LoginAsync(client, "Mert", "flex-2026")).Headers.Location!.ToString());
    }

    [Fact]
    public async Task Banned_player_is_signed_out_and_cannot_sign_in_again()
    {
        var client = Client("flex-2026");
        await LoginAsync(client, "Troll", "flex-2026");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/havuz")).StatusCode);

        var services = _factories[^1].Services;
        var account = await Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
            .GetRequiredService<PreBot.Core.Services.AccountService>(services).GetOrCreateAsync("invite", "troll", "Troll");
        await Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
            .GetRequiredService<PreBot.Core.Services.ModerationService>(services).BanAsync(account.UserId);

        // The open session ends (pages send it to the cookie-clearing logout)...
        Assert.Equal("/", (await client.GetAsync("/auth/logout")).Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/havuz")).StatusCode);
        // ...and signing in again is refused.
        Assert.Equal("/giris?hata=engel", (await LoginAsync(client, "Troll", "flex-2026")).Headers.Location!.ToString());

        // Deleting the data is allowed, but it is not a way back in: the name stays closed.
        await Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
            .GetRequiredService<PreBot.Core.Services.AccountService>(services).DeleteAccountAsync(account.UserId);
        Assert.Equal("/giris?hata=engel", (await LoginAsync(client, "Troll", "flex-2026")).Headers.Location!.ToString());
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/havuz")).StatusCode);
    }

    [Fact]
    public async Task Mute_button_on_a_notification_mutes_only_the_owner_of_the_token()
    {
        var client = Client("flex-2026");
        await LoginAsync(client, "Sessiz", "flex-2026");
        var services = _factories[^1].Services;
        var account = await Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
            .GetRequiredService<PreBot.Core.Services.AccountService>(services).GetOrCreateAsync("invite", "sessiz", "Sessiz");
        var tokens = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
            .GetRequiredService<PreBot.Web.Push.JoinTokens>(services);
        var pre = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions
            .GetRequiredService<PreBot.Core.Services.PreService>(services);

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/api/sustur?t=forged", null)).StatusCode);
        Assert.Null(await pre.GetMutedUntilAsync(account.UserId));

        var ok = await client.PostAsync($"/api/sustur?t={Uri.EscapeDataString(tokens.CreateMute(account.UserId))}", null);
        Assert.Equal(HttpStatusCode.NoContent, ok.StatusCode);
        Assert.NotNull(await pre.GetMutedUntilAsync(account.UserId));
    }

    [Fact]
    public async Task Admin_page_is_only_for_configured_admins()
    {
        var client = Client("flex-2026");
        await LoginAsync(client, "Alper", "flex-2026");

        var page = await client.GetStringAsync("/yonetim");
        Assert.Contains("yalnızca siteyi işleten", page);
        Assert.DoesNotContain("Şikâyetler", page);
    }

    [Fact]
    public async Task Without_an_invite_code_the_form_and_endpoint_do_not_exist()
    {
        var client = Client(null);

        Assert.DoesNotContain("Davet kodu", await client.GetStringAsync("/giris"));
        var response = await client.PostAsync("/auth/invite-login", new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "x", ["code"] = "x" }));
        Assert.NotEqual(HttpStatusCode.Redirect, response.StatusCode);
    }
}
