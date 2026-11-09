using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using PreBot.Web.Ops;

namespace PreBot.Web.Tests;

public sealed class BackupTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), $"prebot-backup-test-{Guid.NewGuid():N}");
    private static readonly (string PublicKey, string PrivateKey) TestKeys = PreBot.Web.Push.VapidKeys.Generate();
    private readonly WebApplicationFactory<Program> _factory;

    public BackupTests()
    {
        Directory.CreateDirectory(_dir);
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Development");
            b.UseSetting("Discord:Token", "");
            b.UseSetting("Discord:Enabled", "false");
            b.UseSetting("Database:JournalMode", "MEMORY");
            b.UseSetting("Authentication:Google:ClientId", "");
            b.UseSetting("DataDirectory", _dir);
            b.UseSetting("ConnectionStrings:PreBot", $"Data Source={Path.Combine(_dir, "prebot-web.db")}");
            b.UseSetting("Push:PublicKey", TestKeys.PublicKey);
            b.UseSetting("Push:PrivateKey", TestKeys.PrivateKey);
        });
    }

    public void Dispose()
    {
        _factory.Dispose();
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(_dir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public async Task Backup_contains_a_readable_database_and_the_keys()
    {
        _ = _factory.CreateClient(); // starts the app: migrates the DB, creates the key ring
        var runner = _factory.Services.GetRequiredService<BackupRunner>();
        Directory.CreateDirectory(Path.Combine(_dir, "keys"));
        await File.WriteAllTextAsync(Path.Combine(_dir, "keys", "key-test.xml"), "<key />");

        var folder = await runner.RunAsync(new DateOnly(2026, 11, 10));

        var dbFile = Path.Combine(folder, "prebot.db");
        Assert.True(File.Exists(dbFile));
        Assert.True(File.Exists(Path.Combine(folder, "keys", "key-test.xml")));
        await using var connection = new SqliteConnection($"Data Source={dbFile};Mode=ReadOnly;Pooling=False");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA integrity_check";
        Assert.Equal("ok", await command.ExecuteScalarAsync());
        command.CommandText = "SELECT count(*) FROM sqlite_master WHERE name = 'Listings'";
        Assert.Equal(1L, await command.ExecuteScalarAsync());
    }

    [Fact]
    public void Retention_keeps_a_week_of_dailies_four_sundays_and_six_monthlies()
    {
        var today = new DateOnly(2026, 11, 10);
        var all = Enumerable.Range(0, 200).Select(i => today.AddDays(-i)).ToList();

        var keep = BackupRetention.Keep(all, today);

        Assert.All(Enumerable.Range(0, 7), i => Assert.Contains(today.AddDays(-i), keep));
        Assert.Contains(new DateOnly(2026, 10, 25), keep);       // a Sunday 16 days ago
        Assert.DoesNotContain(new DateOnly(2026, 10, 27), keep); // a Tuesday 14 days ago
        Assert.Contains(new DateOnly(2026, 6, 1), keep);         // monthly, ~5 months ago
        Assert.DoesNotContain(new DateOnly(2026, 5, 1), keep);   // older than 6 months
    }

    [Theory]
    [InlineData("2026-11-09T21:00:00Z", 5)] // 00:00 TRT -> 5 hours
    [InlineData("2026-11-10T02:00:00Z", 24)] // exactly 05:00 TRT -> tomorrow
    [InlineData("2026-11-10T01:30:00Z", 0.5)] // 04:30 TRT -> 30 minutes
    public void Next_backup_is_at_five_in_the_morning_turkey_time(string nowUtc, double hours) =>
        Assert.Equal(TimeSpan.FromHours(hours), BackupScheduler.DelayUntilNextRun(DateTime.Parse(nowUtc).ToUniversalTime()));
}
