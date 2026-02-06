using DevForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevForge.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            // Value Objects as Owned Entities
            builder.OwnsOne(u => u.Username, username =>
            {
                username.Property(un => un.Value)
                    .HasColumnName("Username")
                    .IsRequired()
                    .HasMaxLength(50);
            });

            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(254);
            });

            builder.OwnsOne(u => u.PasswordHash, passwordHash =>
            {
                passwordHash.Property(ph => ph.Value)
                    .HasColumnName("PasswordHash")
                    .IsRequired();
            });

            builder.OwnsOne(u => u.PhoneNumber, phoneNumber =>
            {
                phoneNumber.Property(pn => pn.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(15);
            });

            // Properties
            builder.Property(u => u.IsActive).IsRequired();
            builder.Property(u => u.EmailConfirmed).IsRequired();
            builder.Property(u => u.PhoneNumberConfirmed).IsRequired();
            builder.Property(u => u.TwoFactorEnabled).IsRequired();
            builder.Property(u => u.LockoutEnabled).IsRequired();
            builder.Property(u => u.AccessFailedCount).IsRequired();
            builder.Property(u => u.CreatedAt).IsRequired();

            builder.Property(u => u.EmailConfirmationToken).HasMaxLength(500);
            builder.Property(u => u.PasswordResetToken).HasMaxLength(500);
            builder.Property(u => u.TwoFactorSecretKey).HasMaxLength(100);

            // Indexes for Performance (on regular properties only, not on owned entities)
            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            builder.HasIndex(u => u.EmailConfirmed)
                .HasDatabaseName("IX_Users_EmailConfirmed");

            builder.HasIndex(u => u.CreatedAt)
                .HasDatabaseName("IX_Users_CreatedAt");

            builder.HasIndex(u => u.LastLoginAt)
                .HasDatabaseName("IX_Users_LastLoginAt");

            builder.HasIndex(u => u.EmailConfirmationToken)
                .HasDatabaseName("IX_Users_EmailConfirmationToken")
                .HasFilter("[EmailConfirmationToken] IS NOT NULL");

            builder.HasIndex(u => u.PasswordResetToken)
                .HasDatabaseName("IX_Users_PasswordResetToken")
                .HasFilter("[PasswordResetToken] IS NOT NULL");

            // Composite indexes for common queries
            builder.HasIndex(u => new { u.IsActive, u.EmailConfirmed })
                .HasDatabaseName("IX_Users_IsActive_EmailConfirmed");

            // Ignore Domain Events
            builder.Ignore(u => u.DomainEvents);

            // Relationships
            builder.HasMany(u => u.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
