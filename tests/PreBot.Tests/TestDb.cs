using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using PreBot.Core.Data;
using PreBot.Core.Services;

namespace PreBot.Tests;

/// <summary>
/// A real SQLite database in a temp file (not the EF InMemory provider, which does not enforce
/// transactions or constraints), so race and constraint behaviour is tested for real.
/// </summary>
public sealed class TestDb : IDisposable
{
    public const ulong Guild = 1000;
    public const ulong Channel = 2000;

    private readonly string _path = Path.Combine(Path.GetTempPath(), $"prebot-test-{Guid.NewGuid():N}.db");

    public TestDb()
    {
        Options = new DbContextOptionsBuilder<PreBotDbContext>()
            .UseSqlite($"Data Source={_path};Pooling=False;Default Timeout=30")
            .Options;
        using (var db = new PreBotDbContext(Options))
        {
            db.Database.Migrate(); // the real migrations, so tests also prove they work
        }

        Factory = new ContextFactory(Options);
        Time = new FakeTimeProvider(new DateTimeOffset(2026, 11, 13, 21, 0, 0, TimeSpan.Zero));
        Settings = new PreBotOptions();
        Service = new PreService(Factory, Time, Settings);
    }

    public DbContextOptions<PreBotDbContext> Options { get; }
    public IDbContextFactory<PreBotDbContext> Factory { get; }
    public FakeTimeProvider Time { get; }
    public PreBotOptions Settings { get; }
    public PreService Service { get; }

    public PreBotDbContext NewContext() => new(Options);

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        try
        {
            File.Delete(_path);
        }
        catch (IOException)
        {
            // Temp file; ignore if still locked.
        }
    }

    private sealed class ContextFactory(DbContextOptions<PreBotDbContext> options) : IDbContextFactory<PreBotDbContext>
    {
        public PreBotDbContext CreateDbContext() => new(options);
    }
}
