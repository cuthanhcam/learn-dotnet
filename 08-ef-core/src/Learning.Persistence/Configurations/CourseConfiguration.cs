using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learning.Persistence.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("courses");
        builder.HasKey(course => course.Id);
        builder.Property(course => course.Title).HasMaxLength(160).IsRequired();
        builder.Property(course => course.Slug).HasMaxLength(180).IsRequired();
        builder.Property(course => course.Price).HasPrecision(18, 2);
        builder.Property(course => course.Version).IsConcurrencyToken();
        builder.HasIndex(course => course.Slug).IsUnique();

        builder.HasOne(course => course.Category)
            .WithMany(category => category.Courses)
            .HasForeignKey(course => course.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(course => course.Modules)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Navigation(course => course.CourseTags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
