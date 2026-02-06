using DevForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevForge.Infrastructure.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(ur => ur.Id);

            // Properties
            builder.Property(ur => ur.UserId)
                .IsRequired();

            builder.Property(ur => ur.RoleId)
                .IsRequired();

            builder.Property(ur => ur.AssignedAt)
                .IsRequired();

            // Composite Index for uniqueness
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // Ignore Domain Events
            builder.Ignore(ur => ur.DomainEvents);
        }
    }
}
