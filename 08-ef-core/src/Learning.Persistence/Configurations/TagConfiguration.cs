using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learning.Persistence.Configurations;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(tag => tag.Id);
        builder.Property(tag => tag.Name).HasMaxLength(50).IsRequired();
        builder.HasIndex(tag => tag.Name).IsUnique();
    }
}
