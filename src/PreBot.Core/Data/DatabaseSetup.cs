using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace PreBot.Core.Data;

public static class DatabaseSetup
{
    private const int SqliteIoError = 10;
    private static readonly string[] AllowedJournalModes = ["WAL", "MEMORY", "DELETE", "TRUNCATE"];

    /// <summary>
    /// Brings the database schema up to date (creates it on first run). Safe to call on every start.
    /// <paramref name="journalMode"/>: WAL (default) lets readers keep reading while the single writer writes.
    /// Tests use MEMORY: no -wal/-shm/-journal files at all, so antivirus scanning on Windows (which briefly locks
    /// fresh files and makes SQLite report "disk I/O error") has nothing to trip over.
    /// </summary>
    public static async Task MigrateAsync(
        IDbContextFactory<PreBotDbContext> factory, string journalMode = "WAL", CancellationToken ct = default)
    {
        journalMode = journalMode.ToUpperInvariant();
        if (!AllowedJournalModes.Contains(journalMode))
        {
            throw new ArgumentException($"Unsupported journal mode '{journalMode}'.", nameof(journalMode));
        }

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await using var db = await factory.CreateDbContextAsync(ct);

                // Set the journal mode first, so the migrations themselves already run in it.
                await db.Database.OpenConnectionAsync(ct);
                // journalMode is checked against a fixed allow-list above, so building the PRAGMA is safe.
                var pragma = "PRAGMA journal_mode=" + journalMode + ";";
                await db.Database.ExecuteSqlRawAsync(pragma, ct);
                await db.Database.MigrateAsync(ct);
                return;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == SqliteIoError && attempt < 5)
            {
                // A brand-new file can be briefly locked by antivirus or file sync.
                SqliteConnection.ClearAllPools();
                await Task.Delay(TimeSpan.FromMilliseconds(250 * attempt), ct);
            }
        }
    }
}
