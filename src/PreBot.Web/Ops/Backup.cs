using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PreBot.Core.Data;
using PreBot.Core.Domain;

namespace PreBot.Web.Ops;

/// <summary>
/// Nightly backup of everything that cannot be recreated: the database, the cookie keys (or everyone is
/// logged out) and the Web Push key (or every push subscription dies). Files go to {DataDirectory}/backups;
/// copying them off the server is the job of a host cron (see README, "Yedekleme").
/// </summary>
public sealed class BackupRunner(
    IDbContextFactory<PreBotDbContext> dbFactory,
    DataDirectory data,
    IHttpClientFactory httpClients,
    IConfiguration config,
    ILogger<BackupRunner> logger)
{
    private const string DateFormat = "yyyy-MM-dd";

    public string BackupRoot => Path.Combine(data.Path, "backups");

    /// <summary>Writes the backup for <paramref name="date"/> and prunes old ones. Returns the backup folder.</summary>
    public async Task<string> RunAsync(DateOnly date, CancellationToken ct = default)
    {
        var folder = Path.Combine(BackupRoot, date.ToString(DateFormat, CultureInfo.InvariantCulture));
        try
        {
            Directory.CreateDirectory(folder);

            // VACUUM INTO writes a consistent copy even while the app is writing (WAL). Copying the .db file
            // itself is not safe: recent commits may still be only in the -wal file.
            var dbFile = Path.Combine(folder, "prebot.db");
            var temp = dbFile + ".tmp";
            File.Delete(temp);
            await using (var db = await dbFactory.CreateDbContextAsync(ct))
            {
                await db.Database.ExecuteSqlRawAsync("VACUUM INTO {0}", [temp], ct);
            }

            File.Move(temp, dbFile, overwrite: true);

            var keys = Path.Combine(data.Path, "keys");
            if (Directory.Exists(keys))
            {
                var keysCopy = Directory.CreateDirectory(Path.Combine(folder, "keys"));
                foreach (var file in Directory.GetFiles(keys, "*.xml"))
                {
                    File.Copy(file, Path.Combine(keysCopy.FullName, Path.GetFileName(file)), overwrite: true);
                }
            }

            var vapid = Path.Combine(data.Path, "vapid-keys.json");
            if (File.Exists(vapid))
            {
                File.Copy(vapid, Path.Combine(folder, "vapid-keys.json"), overwrite: true);
            }

            var removed = Prune(date);
            logger.LogInformation("Backup written to {Folder}; removed {Removed} old backup(s)", folder, removed);
            await HeartbeatAsync(success: true, ct);
            return folder;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Backup failed");
            await HeartbeatAsync(success: false, ct);
            throw;
        }
    }

    private int Prune(DateOnly today)
    {
        var existing = Directory.GetDirectories(BackupRoot)
            .Select(d => DateOnly.TryParseExact(Path.GetFileName(d), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var day)
                ? (Path: d, Day: (DateOnly?)day)
                : (Path: d, Day: null))
            .Where(x => x.Day is not null)
            .ToList();

        var keep = BackupRetention.Keep(existing.Select(x => x.Day!.Value), today);
        var removed = 0;
        foreach (var (path, day) in existing)
        {
            if (!keep.Contains(day!.Value))
            {
                Directory.Delete(path, recursive: true);
                removed++;
            }
        }

        return removed;
    }

    /// <summary>Optional dead-man's switch (e.g. Healthchecks.io): silence for a day means backups stopped.</summary>
    private async Task HeartbeatAsync(bool success, CancellationToken ct)
    {
        var url = config["Backup:HeartbeatUrl"];
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        try
        {
            using var client = httpClients.CreateClient("heartbeat");
            await client.GetAsync(success ? url : url.TrimEnd('/') + "/fail", ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Backup heartbeat failed");
        }
    }
}

/// <summary>Which daily backups to keep: the last 7 days, Sundays of the last 4 weeks, 1st of the month for 6 months.</summary>
public static class BackupRetention
{
    public static IReadOnlySet<DateOnly> Keep(IEnumerable<DateOnly> existing, DateOnly today) =>
        existing.Where(day =>
        {
            var age = today.DayNumber - day.DayNumber;
            return age < 7
                || (age < 28 && day.DayOfWeek == DayOfWeek.Sunday)
                || (age < 183 && day.Day == 1);
        }).ToHashSet();
}

/// <summary>Runs <see cref="BackupRunner"/> every night at 05:00 Turkey time, when nobody is playing.</summary>
public sealed class BackupScheduler(BackupRunner runner, TimeProvider time, ILogger<BackupScheduler> logger) : BackgroundService
{
    public static readonly TimeOnly RunAt = new(5, 0);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var nowUtc = time.GetUtcNow().UtcDateTime;
            var delay = DelayUntilNextRun(nowUtc);
            logger.LogInformation("Next backup in {Delay}", delay);
            await Task.Delay(delay, time, stoppingToken);

            try
            {
                await runner.RunAsync(DateOnly.FromDateTime(TurkeyTime.ToLocal(time.GetUtcNow().UtcDateTime)), stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Already logged; try again tomorrow rather than crash the site.
            }
        }
    }

    public static TimeSpan DelayUntilNextRun(DateTime nowUtc)
    {
        var local = TurkeyTime.ToLocal(nowUtc);
        var next = local.Date + RunAt.ToTimeSpan();
        if (next <= local)
        {
            next = next.AddDays(1);
        }

        return next - local;
    }
}
