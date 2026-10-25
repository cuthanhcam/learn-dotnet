using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Infrastructure.Persistence;

/// <summary>
/// A unit-of-work scoped to one operation, never a singleton. Persistence records live here rather
/// than making the domain aggregate satisfy ORM setters, constructors, and navigation conventions.
/// </summary>
public sealed class EnrollmentDbContext(DbContextOptions<EnrollmentDbContext> options) : DbContext(options)
{
    public DbSet<OfferingRow> Offerings => Set<OfferingRow>();
    public DbSet<EnrollmentReceiptRow> Receipts => Set<EnrollmentReceiptRow>();
    public DbSet<OutboxRow> Outbox => Set<OutboxRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OfferingRow>(entity =>
        {
            entity.HasKey(row => row.Id);
            entity.Property(row => row.Title).HasMaxLength(200).IsRequired();
            entity.Property(row => row.LearnersJson).IsRequired();
            // ExecuteUpdate below includes an explicit version predicate. IsConcurrencyToken also
            // protects future tracked updates, but does NOT add predicates to ExecuteUpdate itself.
            entity.Property(row => row.Version).IsConcurrencyToken();
            entity.ToTable("Offerings", table =>
            {
                table.HasCheckConstraint("CK_Offerings_Capacity", "Capacity BETWEEN 1 AND 1000");
                table.HasCheckConstraint("CK_Offerings_Count", "EnrolledCount BETWEEN 0 AND Capacity");
                table.HasCheckConstraint("CK_Offerings_Version", "Version >= EnrolledCount");
            });
        });
        modelBuilder.Entity<EnrollmentReceiptRow>().HasKey(row => row.RequestId);
        modelBuilder.Entity<OutboxRow>().HasKey(row => row.MessageId);
        modelBuilder.Entity<OutboxRow>().HasIndex(row => new { row.Dispatched, row.MessageId });
    }
}

public sealed class OfferingRow
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public int Capacity { get; set; }
    public int EnrolledCount { get; set; }
    public long Version { get; set; }
    public string LearnersJson { get; set; } = "[]";
}

public sealed class EnrollmentReceiptRow
{
    public Guid RequestId { get; set; }
    public Guid OfferingId { get; set; }
    public Guid LearnerId { get; set; }
    public int Status { get; set; }
    public long? Version { get; set; }
}

/// <summary>A versioned integration contract is stored as data, not an assembly-qualified CLR name.</summary>
public sealed class OutboxRow
{
    public Guid MessageId { get; set; }
    public Guid OfferingId { get; set; }
    public Guid LearnerId { get; set; }
    public int SchemaVersion { get; set; } = 1;
    public bool Dispatched { get; set; }
}
