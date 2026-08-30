using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learning.Persistence.Configurations;

public sealed class CourseTagConfiguration : IEntityTypeConfiguration<CourseTag>
{
    public void Configure(EntityTypeBuilder<CourseTag> builder)
    {
        builder.ToTable("course_tags");
        builder.HasKey(link => new { link.CourseId, link.TagId });

        builder.HasOne(link => link.Course)
            .WithMany(course => course.CourseTags)
            .HasForeignKey(link => link.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(link => link.Tag)
            .WithMany(tag => tag.CourseTags)
            .HasForeignKey(link => link.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
