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

        public Task PublishAsync(Listing listing, CancellationToken ct = default)
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
    }
}
