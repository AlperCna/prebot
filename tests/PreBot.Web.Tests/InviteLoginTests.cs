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

    private static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string name, string code, bool accept = true)
    {
        var page = await client.GetStringAsync("/giris");
        var form = new Dictionary<string, string>
        {
            ["name"] = name,
            ["code"] = code,
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
    public async Task Without_an_invite_code_the_form_and_endpoint_do_not_exist()
    {
        var client = Client(null);

        Assert.DoesNotContain("Davet kodu", await client.GetStringAsync("/giris"));
        var response = await client.PostAsync("/auth/invite-login", new FormUrlEncodedContent(new Dictionary<string, string> { ["name"] = "x", ["code"] = "x" }));
        Assert.NotEqual(HttpStatusCode.Redirect, response.StatusCode);
    }
}
