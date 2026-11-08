using Microsoft.EntityFrameworkCore;

namespace PreBot.Tests;

public sealed class MigrationTests : IDisposable
{
    private readonly TestDb _t = new();

    public void Dispose() => _t.Dispose();

    [Fact]
    public void Model_has_no_changes_missing_a_migration()
    {
        using var db = _t.NewContext();

        // Fails if someone changed an entity without running: dotnet ef migrations add <Name> --project src/PreBot.Core
        Assert.False(db.Database.HasPendingModelChanges());
    }

    [Fact]
    public void All_migrations_are_applied_on_a_fresh_database()
    {
        using var db = _t.NewContext();

        Assert.Empty(db.Database.GetPendingMigrations());
        Assert.NotEmpty(db.Database.GetAppliedMigrations());
    }
}
