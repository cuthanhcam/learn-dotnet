using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Notifications;

/// <summary>
/// The notifications module owns its schema and has no reference to enrollment infrastructure.
/// The welcome entry is a durable local work item, NOT proof that an external email was delivered.
/// </summary>
public sealed class WelcomeDbContext(DbContextOptions<WelcomeDbContext> options) : DbContext(options)
{
    public DbSet<WelcomeWorkItem> WorkItems => Set<WelcomeWorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<WelcomeWorkItem>().HasKey(item => item.MessageId);
}

public sealed class WelcomeWorkItem
{
    // The primary key serves as an inbox deduplication key. For this one-effect consumer, the work
    // item and inbox record are the SAME row, so they cannot commit independently.
    public Guid MessageId { get; set; }
    public Guid OfferingId { get; set; }
    public Guid LearnerId { get; set; }
}
