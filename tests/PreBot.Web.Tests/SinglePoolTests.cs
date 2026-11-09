using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using PreBot.Bot;
using PreBot.Core.Domain;
using PreBot.Core.Notifications;
using PreBot.Core.Services;
using PreBot.Web.Live;
using PreBot.Web.Push;

namespace PreBot.Web.Tests;

/// <summary>
/// One pool for the website and Discord: a group that opens a listing with /pre-lazim reaches players on
/// the site, and players who only use Discord are reached by DM (no push device).
/// </summary>
public sealed class SinglePoolTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"prebot-pool-test-{Guid.NewGuid():N}.db");
    private readonly RecordingMessenger _dm = new();
    private readonly WebApplicationFactory<Program> _factory;

    public SinglePoolTests()
    {
        var (pub, priv) = VapidKeys.Generate();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("Discord:Token", "");
            b.UseSetting("Discord:Enabled", "false");
            b.UseSetting("Database:JournalMode", "MEMORY");
            b.UseSetting("Authentication:Google:ClientId", "");
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
            b.UseSetting("Push:PublicKey", pub);
            b.UseSetting("Push:PrivateKey", priv);
            b.UseSetting("PublicUrl", "https://prebot.example");
            // What Program.cs registers when the bot runs inside the site, with Discord itself faked.
            b.ConfigureTestServices(s =>
            {
                s.AddSingleton<IDirectMessenger>(_dm);
                s.AddSingleton<IWebPool, WebPoolForDiscord>();
                s.AddSingleton<INotificationChannel, DiscordDmChannel>();
            });
        });
        _factory.CreateClient();

        // Who is reachable by DM follows the real rule (Discord account, no push device).
        var real = new DiscordDirectMessenger(
            new Discord.WebSocket.DiscordSocketClient(), Get<AccountService>(), Get<SubscriptionService>(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<DiscordDirectMessenger>.Instance);
        _dm.Reach = ids => real.ReachableWithoutPushAsync(ids);
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

    [Fact]
    public async Task Discord_user_gets_the_same_account_as_discord_login_on_the_site()
    {
        var pool = Get<IWebPool>();
        var fromBot = await pool.AccountForDiscordUserAsync(1234567890123, "Mert");
        var fromLogin = await Get<AccountService>().GetOrCreateAsync("discord", "1234567890123", "Mert");

        Assert.Equal(fromLogin.UserId, fromBot);
        Assert.Equal(fromBot, await pool.AccountForDiscordUserAsync(1234567890123, "Başka İsim"));
    }

    [Fact]
    public async Task Listing_opened_on_discord_reaches_site_players_and_discord_players_by_dm()
    {
        var pool = Get<IWebPool>();
        var actions = Get<PoolActions>();
        var siteUser = (await Get<AccountService>().GetOrCreateAsync("google", "g-1", "Site Oyuncusu")).UserId;
        var discordUser = await pool.AccountForDiscordUserAsync(555, "Discordcu");
        var host = await pool.AccountForDiscordUserAsync(777, "İlan Sahibi");

        await actions.SetAvailableAsync(siteUser, Role.Support, Rank.Gold, TimeSpan.FromHours(1));
        await pool.SetAvailableAsync(discordUser, Role.Fill, Rank.Silver, TimeSpan.FromHours(1));

        var result = await pool.CreateListingAsync(host, Role.Support, Rank.Iron, Rank.Diamond, 1, null, announceChannelId: 42);

        Assert.NotNull(result.Listing);
        Assert.Equal(Communities.Web, result.Listing!.GuildId); // a website listing, visible on the site
        Assert.Equal(2, result.PotentialMatches);                 // the site player and the Discord player
        var dm = await _dm.WaitForListingAsync(555);
        Assert.Equal(result.Listing.Id, dm.ListingId);
        Assert.DoesNotContain(_dm.Listings, d => d.DiscordId == 777); // the host is not notified about their own listing

        // Without a Riot ID the seat can't be taken (the host could not invite them); Discord asks for it first.
        Assert.Equal(ClaimResult.NeedsRiotId, (await pool.ClaimAsync(result.Listing.Id, discordUser)).Result);
        Assert.True(await pool.SetRiotIdAsync(discordUser, "Discordcu#TR1"));

        // The Discord player takes the seat from the DM button; the Discord-only host hears about it by DM.
        var claim = await pool.ClaimAsync(result.Listing.Id, discordUser);
        Assert.Equal(ClaimResult.ClaimedAndFilled, claim.Result);
        var hostDm = await _dm.WaitForTextAsync(777);
        Assert.Contains("Discordcu", hostDm.Title);
        Assert.Equal($"https://prebot.example/ilan/{result.Listing.Id}", hostDm.Url);
    }

    [Fact]
    public async Task Player_who_becomes_available_late_hears_about_a_waiting_listing_at_once()
    {
        var pool = Get<IWebPool>();
        var host = await pool.AccountForDiscordUserAsync(1001, "Host");
        var late = await pool.AccountForDiscordUserAsync(1002, "Geç Gelen");
        var listing = (await pool.CreateListingAsync(host, Role.Mid, Rank.Iron, Rank.Diamond, 1, null, 42)).Listing!;
        Assert.Empty(_dm.Listings); // nobody in the pool yet

        await pool.SetAvailableAsync(late, Role.Mid, Rank.Gold, TimeSpan.FromHours(1));

        var dm = await _dm.WaitForListingAsync(1002);
        Assert.Equal(listing.Id, dm.ListingId);
    }

    [Fact]
    public async Task A_player_holds_one_open_seat_at_a_time()
    {
        var pool = Get<IWebPool>();
        var a = await pool.AccountForDiscordUserAsync(2001, "A");
        var b = await pool.AccountForDiscordUserAsync(2002, "B");
        var player = await pool.AccountForDiscordUserAsync(2003, "Oyuncu");
        await pool.SetRiotIdAsync(player, "Oyuncu#TR1");
        var first = (await pool.CreateListingAsync(a, Role.Mid, Rank.Iron, Rank.Diamond, 2, null, 42)).Listing!;
        var second = (await pool.CreateListingAsync(b, Role.Top, Rank.Iron, Rank.Diamond, 1, null, 42)).Listing!;

        Assert.Equal(ClaimResult.Claimed, (await pool.ClaimAsync(first.Id, player)).Result);
        Assert.Equal(ClaimResult.HoldsOtherSeat, (await pool.ClaimAsync(second.Id, player)).Result);
    }

    [Fact]
    public async Task Players_with_a_push_device_are_not_also_messaged_on_discord()
    {
        var accounts = Get<AccountService>();
        var withPush = await Get<IWebPool>().AccountForDiscordUserAsync(901, "Push'lu");
        var withoutPush = await Get<IWebPool>().AccountForDiscordUserAsync(902, "Push'suz");
        var google = (await accounts.GetOrCreateAsync("google", "g-9", "Google")).UserId;
        await Get<SubscriptionService>().SavePushSubscriptionAsync(withPush, "https://push.example/abc", "p256", "auth", "PC");

        var messenger = new DiscordDirectMessenger(
            new Discord.WebSocket.DiscordSocketClient(), accounts, Get<SubscriptionService>(),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<DiscordDirectMessenger>.Instance);
        var reachable = await messenger.ReachableWithoutPushAsync([withPush, withoutPush, google]);

        Assert.Equal(new Dictionary<ulong, ulong> { [withoutPush] = 902 }, reachable);
    }

    private sealed class RecordingMessenger : IDirectMessenger
    {
        private readonly IDirectMessenger _real;
        public ConcurrentQueue<(ulong DiscordId, int ListingId)> Listings { get; } = new();
        public ConcurrentQueue<(ulong DiscordId, string Title, string? Url)> Texts { get; } = new();

        public RecordingMessenger() => _real = new NoDirectMessenger();

        // Reachability is the real rule (Discord account, no push device), resolved lazily from the app's services.
        public Func<IEnumerable<ulong>, Task<IReadOnlyDictionary<ulong, ulong>>>? Reach { get; set; }

        public async Task<IReadOnlyDictionary<ulong, ulong>> ReachableWithoutPushAsync(IEnumerable<ulong> userIds, CancellationToken ct = default) =>
            Reach is null ? await _real.ReachableWithoutPushAsync(userIds, ct) : await Reach(userIds);

        public Task SendListingAsync(ulong discordUserId, WaveAlert alert, string listingUrl, CancellationToken ct = default)
        {
            Listings.Enqueue((discordUserId, alert.ListingId));
            return Task.CompletedTask;
        }

        public Task SendTextAsync(ulong discordUserId, string title, string body, string? url, CancellationToken ct = default)
        {
            Texts.Enqueue((discordUserId, title, url));
            return Task.CompletedTask;
        }

        public async Task<(ulong DiscordId, int ListingId)> WaitForListingAsync(ulong discordId)
        {
            for (var i = 0; i < 100; i++)
            {
                if (Listings.FirstOrDefault(l => l.DiscordId == discordId) is { ListingId: > 0 } found)
                {
                    return found;
                }

                await Task.Delay(50);
            }

            throw new TimeoutException($"No listing DM to {discordId}");
        }

        public async Task<(ulong DiscordId, string Title, string? Url)> WaitForTextAsync(ulong discordId)
        {
            for (var i = 0; i < 100; i++)
            {
                if (Texts.FirstOrDefault(t => t.DiscordId == discordId) is { Title: not null } found)
                {
                    return found;
                }

                await Task.Delay(50);
            }

            throw new TimeoutException($"No DM to {discordId}");
        }
    }
}
