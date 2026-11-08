using Microsoft.EntityFrameworkCore;

namespace PreBot.Core.Data;

public static class DatabaseSetup
{
    /// <summary>
    /// Brings the database schema up to date (creates it on first run) and enables WAL,
    /// so readers keep reading while the single writer writes. Safe to call on every start.
    /// </summary>
    public static async Task MigrateAsync(IDbContextFactory<PreBotDbContext> factory, CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await db.Database.MigrateAsync(ct);
        await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;", ct);
    }
}
