using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using PreBot.Core.Domain;
using PreBot.Core.Services;
using PreBot.Web.Live;
using PreBot.Web.Push;

namespace PreBot.Web.Tests;

/// <summary>
/// Runs the real app with a fake push service in place of Google/Mozilla/Apple, and checks what
/// the server actually sends when a listing opens.
/// </summary>
public sealed class PushTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"prebot-push-test-{Guid.NewGuid():N}.db");
    private readonly FakePushService _push = new();
    private readonly WebApplicationFactory<Program> _factory;

    public PushTests()
    {
        var (publicKey, privateKey) = VapidKeys.Generate();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
            b.UseSetting("Push:PublicKey", publicKey);
            b.UseSetting("Push:PrivateKey", privateKey);
            b.UseSetting("Push:Subject", "mailto:test@example.com");
            b.ConfigureTestServices(s => s.AddHttpClient("webpush").ConfigurePrimaryHttpMessageHandler(() => _push));
        });
        _factory.CreateClient(); // start the host (creates the database)
    }

    public void Dispose()
    {
        _factory.Dispose();
        SqliteConnection.ClearAllPools();
        foreach (var file in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
        {
            try { File.Delete(file); } catch (IOException) { }
        }
    }

    private T Get<T>() where T : notnull => _factory.Services.GetRequiredService<T>();

    /// <summary>A browser-like subscription: a real P-256 public key and 16-byte auth secret.</summary>
    private static (string P256dh, string Auth) BrowserKeys()
    {
        using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        var q = ecdh.ExportParameters(false).Q;
        var point = new byte[65];
        point[0] = 0x04;
        q.X!.CopyTo(point, 1);
        q.Y!.CopyTo(point, 33);
        return (WebEncoders.Base64UrlEncode(point), WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(16)));
    }

    private async Task<ulong> PlayerWithPhoneAsync(string name, string endpoint)
    {
        var account = await Get<AccountService>().GetOrCreateAsync("dev", name.ToLowerInvariant(), name);
        var (p256dh, auth) = BrowserKeys();
        await Get<SubscriptionService>().SavePushSubscriptionAsync(account.UserId, endpoint, p256dh, auth, "Android · Chrome");
        return account.UserId;
    }

    [Fact]
    public async Task Opening_a_listing_pushes_an_encrypted_urgent_short_lived_message_to_matching_players()
    {
        var host = (await Get<AccountService>().GetOrCreateAsync("dev", "mert", "Mert")).UserId;
        var player = await PlayerWithPhoneAsync("Alper", "https://push.example/alper-phone");
        await Get<PoolActions>().SetAvailableAsync(player, Role.Support, Rank.Gold, TimeSpan.FromHours(1));

        var result = await Get<PoolActions>().CreateListingAsync(host, Role.Support, Rank.Gold, Rank.Platinum, 1, null);

        var request = Assert.Single(_push.Requests);
        Assert.Equal("https://push.example/alper-phone", request.Url);
        Assert.Equal("120", request.Headers["TTL"]);
        Assert.Equal("high", request.Headers["Urgency"]);
        Assert.Equal($"listing{result.Listing!.Id}", request.Headers["Topic"]);
        Assert.StartsWith("vapid t=", request.Headers["Authorization"]);
        Assert.Equal("aes128gcm", request.Headers["Content-Encoding"]);
        Assert.True(request.BodyLength > 100); // encrypted payload, not plain text
    }

    [Fact]
    public async Task Players_who_do_not_fit_get_nothing()
    {
        var host = (await Get<AccountService>().GetOrCreateAsync("dev", "mert", "Mert")).UserId;
        var top = await PlayerWithPhoneAsync("Can", "https://push.example/can");
        await Get<PoolActions>().SetAvailableAsync(top, Role.Top, Rank.Gold, TimeSpan.FromHours(1));

        await Get<PoolActions>().CreateListingAsync(host, Role.Support, Rank.Gold, Rank.Gold, 1, null);

        Assert.Empty(_push.Requests);
    }

    [Fact]
    public async Task Dead_subscriptions_are_removed_when_the_push_service_says_gone()
    {
        var player = await PlayerWithPhoneAsync("Alper", "https://push.example/gone");
        _push.StatusFor["https://push.example/gone"] = HttpStatusCode.Gone;

        var sent = await Get<PushSender>().SendTestAsync(player);

        Assert.Equal(0, sent);
        Assert.Equal(0, await Get<SubscriptionService>().CountPushDevicesAsync(player));
    }

    [Fact]
    public async Task Test_notification_reaches_every_device_of_the_user()
    {
        var player = await PlayerWithPhoneAsync("Alper", "https://push.example/phone");
        var (p, a) = BrowserKeys();
        await Get<SubscriptionService>().SavePushSubscriptionAsync(player, "https://push.example/pc", p, a, "Windows · Edge");

        var sent = await Get<PushSender>().SendTestAsync(player);

        Assert.Equal(2, sent);
        Assert.Equal(
            new[] { "https://push.example/pc", "https://push.example/phone" },
            _push.Requests.Select(r => r.Url).Order());
    }

    private sealed class FakePushService : HttpMessageHandler
    {
        public List<(string Url, Dictionary<string, string> Headers, int BodyLength)> Requests { get; } = [];
        public Dictionary<string, HttpStatusCode> StatusFor { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var headers = request.Headers.Concat(request.Content?.Headers ?? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>())
                .ToDictionary(h => h.Key, h => string.Join(",", h.Value), StringComparer.OrdinalIgnoreCase);
            var body = request.Content is null ? [] : await request.Content.ReadAsByteArrayAsync(ct);
            var url = request.RequestUri!.ToString();
            lock (Requests)
            {
                Requests.Add((url, headers, body.Length));
            }

            return new HttpResponseMessage(StatusFor.GetValueOrDefault(url, HttpStatusCode.Created));
        }
    }
}
