using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PreBot.Core.Data;

/// <summary>
/// Used only by the EF Core tools (dotnet ef migrations add ...). The apps configure the real connection.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PreBotDbContext>
{
    public PreBotDbContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<PreBotDbContext>()
            .UseSqlite("Data Source=design-time.db")
            .Options);
}
