using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence;

public sealed class LearningDbContext(DbContextOptions<LearningDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseModule> CourseModules => Set<CourseModule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations from this assembly so the context stays a composition point rather
        // than accumulating every table, relationship, index, and provider detail in one method.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearningDbContext).Assembly);
    }
}
