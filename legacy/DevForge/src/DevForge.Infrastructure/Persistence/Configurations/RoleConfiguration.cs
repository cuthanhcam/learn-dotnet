using DevForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevForge.Infrastructure.Persistence.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.IsSystemRole)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            // Indexes for Performance
            builder.HasIndex(r => r.Name)
                .HasDatabaseName("IX_Roles_Name")
                .IsUnique();

            builder.HasIndex(r => r.IsSystemRole)
                .HasDatabaseName("IX_Roles_IsSystemRole");

            builder.HasIndex(r => r.CreatedAt)
                .HasDatabaseName("IX_Roles_CreatedAt");

            // Ignore Domain Events
            builder.Ignore(r => r.DomainEvents);

            // Relationships
            builder.HasMany(r => r.RolePermissions)
                .WithOne()
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
