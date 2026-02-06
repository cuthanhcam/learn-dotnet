using DevForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevForge.Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            // Indexes for Performance
            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Permissions_Name")
                .IsUnique();

            builder.HasIndex(p => p.Category)
                .HasDatabaseName("IX_Permissions_Category");

            builder.HasIndex(p => new { p.Category, p.Name })
                .HasDatabaseName("IX_Permissions_Category_Name");

            // Ignore Domain Events
            builder.Ignore(p => p.DomainEvents);

            // Relationships
            builder.HasMany(p => p.RolePermissions)
                .WithOne()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
