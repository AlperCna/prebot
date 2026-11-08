using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace PreBot.Core.Data;

public static class SqliteSetup
{
    /// <summary>
    /// SQLite with a journal mode. WAL is stored in the database file, so setting it once (DatabaseSetup) is enough.
    /// Other modes (MEMORY in tests) are per connection, so they are applied every time a connection opens.
    /// </summary>
    public static DbContextOptionsBuilder UsePreBotSqlite(this DbContextOptionsBuilder options, string connectionString, string journalMode = "WAL")
    {
        options.UseSqlite(connectionString);
        if (!string.Equals(journalMode, "WAL", StringComparison.OrdinalIgnoreCase))
        {
            options.AddInterceptors(new JournalModeInterceptor(journalMode.ToUpperInvariant()));
        }

        return options;
    }

    public static DbContextOptionsBuilder<TContext> UsePreBotSqlite<TContext>(
        this DbContextOptionsBuilder<TContext> options, string connectionString, string journalMode = "WAL")
        where TContext : DbContext
    {
        ((DbContextOptionsBuilder)options).UsePreBotSqlite(connectionString, journalMode);
        return options;
    }

    private sealed class JournalModeInterceptor(string mode) : DbConnectionInterceptor
    {
        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData) => Apply(connection);

        public override Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken ct = default)
        {
            Apply(connection);
            return Task.CompletedTask;
        }

        private void Apply(DbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"PRAGMA journal_mode={mode};";
            command.ExecuteNonQuery();
        }
    }
}
