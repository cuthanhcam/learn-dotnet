using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learning.Persistence.Configurations;

public sealed class CourseModuleConfiguration : IEntityTypeConfiguration<CourseModule>
{
    public void Configure(EntityTypeBuilder<CourseModule> builder)
    {
        builder.ToTable("course_modules");
        builder.HasKey(module => module.Id);
        builder.Property(module => module.Title).HasMaxLength(160).IsRequired();
        builder.HasIndex(module => new { module.CourseId, module.Order }).IsUnique();

        builder.HasOne(module => module.Course)
            .WithMany(course => course.Modules)
            .HasForeignKey(module => module.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
