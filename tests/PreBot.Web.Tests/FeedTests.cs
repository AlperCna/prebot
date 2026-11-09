using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using PreBot.Core.Domain;
using PreBot.Core.Services;
using PreBot.Web.Live;
using PreBot.Web.Push;

namespace PreBot.Web.Tests;

/// <summary>A listing opened on the website is announced through the feed (Discord), without blocking.</summary>
public sealed class FeedTests : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"prebot-feed-test-{Guid.NewGuid():N}.db");
    private readonly RecordingFeed _feed = new();
    private readonly WebApplicationFactory<Program> _factory;

    public FeedTests()
    {
        var (pub, priv) = VapidKeys.Generate();
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            // Tests never use local user-secrets: no real Discord bot, no real Google keys.
            b.UseSetting("Discord:Token", "");
            b.UseSetting("Discord:Enabled", "false");
            b.UseSetting("Database:JournalMode", "MEMORY");
            b.UseSetting("Authentication:Google:ClientId", "");
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={_dbPath}");
            b.UseSetting("Push:PublicKey", pub);
            b.UseSetting("Push:PrivateKey", priv);
            b.ConfigureTestServices(s => s.AddSingleton<IListingFeed>(_feed));
        });
        _factory.CreateClient();
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

    [Fact]
    public async Task Opening_a_listing_announces_it_once()
    {
        var host = (await _factory.Services.GetRequiredService<AccountService>().GetOrCreateAsync("dev", "mert", "Mert")).UserId;
        var actions = _factory.Services.GetRequiredService<PoolActions>();

        var result = await actions.CreateListingAsync(host, Role.Jungle, Rank.Silver, Rank.Gold, 1, "eğlencesine");
        var blocked = await actions.CreateListingAsync(host, Role.Top, Rank.Silver, Rank.Gold, 1, null); // already has one

        var announced = await _feed.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal(result.Listing!.Id, announced.Id);
        Assert.Equal(Role.Jungle, announced.Role);
        Assert.Null(blocked.Listing);
        await Task.Delay(300);
        Assert.Single(_feed.All);
    }

    private sealed class RecordingFeed : IListingFeed
    {
        private readonly TaskCompletionSource<Listing> _first = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public List<Listing> All { get; } = [];

        public Task PublishAsync(Listing listing, ulong? alsoChannelId = null, CancellationToken ct = default)
        {
            lock (All)
            {
                All.Add(listing);
            }

            _first.TrySetResult(listing);
            return Task.CompletedTask;
        }

        public async Task<Listing> WaitAsync(TimeSpan timeout) =>
            await _first.Task.WaitAsync(timeout);

        public List<int> Updates { get; } = [];

        public Task UpdateAsync(int listingId, CancellationToken ct = default)
        {
            lock (Updates)
            {
                Updates.Add(listingId);
            }

            return Task.CompletedTask;
        }

        public async Task WaitForUpdatesAsync(int listingId, int count, TimeSpan timeout)
        {
            var until = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < until)
            {
                lock (Updates)
                {
                    if (Updates.Count(id => id == listingId) >= count)
                    {
                        return;
                    }
                }

                await Task.Delay(50);
            }

            throw new TimeoutException($"Expected {count} update(s) of listing {listingId}, got {Updates.Count(id => id == listingId)}");
        }
    }

    [Fact]
    public async Task Joining_and_closing_keep_the_announcement_up_to_date()
    {
        var accounts = _factory.Services.GetRequiredService<AccountService>();
        var host = (await accounts.GetOrCreateAsync("dev", "host2", "Host")).UserId;
        var player = (await accounts.GetOrCreateAsync("dev", "player2", "Player")).UserId;
        var actions = _factory.Services.GetRequiredService<PoolActions>();
        var id = (await actions.CreateListingAsync(host, Role.Mid, Rank.Gold, Rank.Gold, 1, null)).Listing!.Id;
        await _factory.Services.GetRequiredService<PreService>().SetRiotIdAsync(player, "Player#TR1");

        await actions.ClaimAsync(id, player);
        await _feed.WaitForUpdatesAsync(id, 1, TimeSpan.FromSeconds(5));

        await actions.CancelAsync(id, host);
        await _feed.WaitForUpdatesAsync(id, 2, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Expired_listing_can_be_reopened_with_the_same_settings_by_its_host_only()
    {
        var accounts = _factory.Services.GetRequiredService<AccountService>();
        var host = (await accounts.GetOrCreateAsync("dev", "host3", "Host")).UserId;
        var other = (await accounts.GetOrCreateAsync("dev", "other3", "Other")).UserId;
        var actions = _factory.Services.GetRequiredService<PoolActions>();
        var first = (await actions.CreateListingAsync(host, Role.Adc, Rank.Silver, Rank.Platinum, 2, "akşam oyunu")).Listing!;
        await actions.CancelAsync(first.Id, host);

        Assert.Null((await actions.ReopenAsync(first.Id, other)).Listing);

        var again = (await actions.ReopenAsync(first.Id, host)).Listing;
        Assert.NotNull(again);
        Assert.NotEqual(first.Id, again!.Id);
        Assert.Equal((Role.Adc, Rank.Silver, Rank.Platinum, 2, "akşam oyunu"),
            (again.Role, again.MinRank, again.MaxRank, again.SlotsNeeded, again.Note));
    }

    [Fact]
    public async Task Reopening_keeps_the_lobby_already_open_in_the_client()
    {
        var accounts = _factory.Services.GetRequiredService<AccountService>();
        var host = (await accounts.GetOrCreateAsync("dev", "host4", "Host")).UserId;
        var actions = _factory.Services.GetRequiredService<PoolActions>();
        var first = (await actions.CreateListingAsync(host, Role.Top, Rank.Iron, Rank.Diamond, 1, null, withLobby: true, lobbyLocked: true)).Listing!;
        Assert.NotNull(first.LobbyPassword);
        Assert.DoesNotContain(first.LobbyPassword!, Lobby.Name(first.Role, first.LobbyCode!));

        await actions.CancelAsync(first.Id, host);
        var again = (await actions.ReopenAsync(first.Id, host)).Listing!;
        Assert.Equal((first.LobbyCode, first.LobbyPassword), (again.LobbyCode, again.LobbyPassword));
    }
}
