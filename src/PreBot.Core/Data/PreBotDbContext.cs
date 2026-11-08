using Microsoft.EntityFrameworkCore;
using PreBot.Core.Domain;

namespace PreBot.Core.Data;

public sealed class PreBotDbContext(DbContextOptions<PreBotDbContext> options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ListingSlot> ListingSlots => Set<ListingSlot>();
    public DbSet<UserAccount> Accounts => Set<UserAccount>();
    public DbSet<ListingNotification> ListingNotifications => Set<ListingNotification>();
    public DbSet<WebPushSubscription> PushSubscriptions => Set<WebPushSubscription>();
    public DbSet<AlertSubscription> AlertSubscriptions => Set<AlertSubscription>();
    public DbSet<ListingAnnouncement> ListingAnnouncements => Set<ListingAnnouncement>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Discord snowflakes are ulong; SQLite stores 64-bit signed integers. Snowflakes fit below long.MaxValue.
        configurationBuilder.Properties<ulong>().HaveConversion<long>();
        configurationBuilder.Properties<ulong?>().HaveConversion<long?>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(e =>
        {
            e.HasKey(p => p.UserId);
            e.Property(p => p.UserId).ValueGeneratedNever();
            e.Property(p => p.RiotId).HasMaxLength(32);
        });

        modelBuilder.Entity<Availability>(e =>
        {
            e.HasIndex(a => new { a.GuildId, a.UserId }).IsUnique();
            e.HasIndex(a => new { a.GuildId, a.ExpiresAtUtc });
        });

        modelBuilder.Entity<Listing>(e =>
        {
            e.Property(l => l.Note).HasMaxLength(200);
            e.HasIndex(l => new { l.Status, l.NextWaveAtUtc });
            e.HasIndex(l => new { l.GuildId, l.HostUserId, l.Status });
            e.HasMany(l => l.Slots)
                .WithOne()
                .HasForeignKey(s => s.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ListingAnnouncement>(e => e.HasIndex(a => a.ListingId));

        modelBuilder.Entity<WebPushSubscription>(e =>
        {
            e.Property(p => p.Endpoint).HasMaxLength(1024);
            e.Property(p => p.P256dh).HasMaxLength(256);
            e.Property(p => p.Auth).HasMaxLength(128);
            e.Property(p => p.Device).HasMaxLength(64);
            e.HasIndex(p => p.Endpoint).IsUnique();
            e.HasIndex(p => p.UserId);
        });

        modelBuilder.Entity<AlertSubscription>(e =>
        {
            e.HasIndex(a => new { a.CommunityId, a.UserId }).IsUnique();
            e.HasIndex(a => new { a.CommunityId, a.Enabled });
        });

        modelBuilder.Entity<ListingNotification>(e =>
        {
            e.HasIndex(n => new { n.ListingId, n.UserId }).IsUnique();
            e.HasIndex(n => n.UserId);
        });

        modelBuilder.Entity<UserAccount>(e =>
        {
            e.Property(a => a.Provider).HasMaxLength(16);
            e.Property(a => a.ProviderKey).HasMaxLength(128);
            e.Property(a => a.DisplayName).HasMaxLength(32);
            e.Ignore(a => a.UserId);
            e.HasIndex(a => new { a.Provider, a.ProviderKey }).IsUnique();
        });

        modelBuilder.Entity<ListingSlot>(e =>
        {
            // Backstop against double-joins; the claim query also checks this.
            e.HasIndex(s => new { s.ListingId, s.UserId }).IsUnique();
        });
    }
}
