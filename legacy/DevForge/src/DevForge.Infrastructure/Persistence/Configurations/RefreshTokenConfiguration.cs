using DevForge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevForge.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            // Properties
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(rt => rt.UserId)
                .IsRequired();

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedByIp)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(50);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(500);

            builder.Property(rt => rt.DeviceInfo)
                .HasMaxLength(500);

            builder.Property(rt => rt.UserAgent)
                .HasMaxLength(500);

            builder.Property(rt => rt.ReasonRevoked)
                .HasMaxLength(200);

            // Indexes for Performance
            builder.HasIndex(rt => rt.Token)
                .HasDatabaseName("IX_RefreshTokens_Token")
                .IsUnique();

            builder.HasIndex(rt => rt.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            builder.HasIndex(rt => rt.ExpiresAt)
                .HasDatabaseName("IX_RefreshTokens_ExpiresAt");

            builder.HasIndex(rt => rt.RevokedAt)
                .HasDatabaseName("IX_RefreshTokens_RevokedAt")
                .HasFilter("[RevokedAt] IS NOT NULL");

            builder.HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
                .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

            // Ignore computed properties and domain events
            builder.Ignore(rt => rt.DomainEvents);
            builder.Ignore(rt => rt.IsExpired);
            builder.Ignore(rt => rt.IsRevoked);
            builder.Ignore(rt => rt.IsActive);
        }
    }
}
