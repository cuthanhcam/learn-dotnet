using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class ModelAndUnitOfWorkTests
{
    [Fact]
    public async Task SaveChanges_PersistsAggregateAcrossShortLivedContexts()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        Guid courseId;

        await using (LearningDbContext writeContext = database.CreateContext())
        {
            var category = new Category("Backend Engineering");
            var course = new Course(
                category.Id,
                "Entity Framework Core",
                "ef-core",
                49.90m,
                DateTimeOffset.Parse("2026-08-30T00:00:00Z"));
            course.AddModule("DbContext and the model");
            course.AddModule("Change tracking");
            writeContext.Add(category);
            writeContext.Add(course);

            Assert.Equal(4, await writeContext.SaveChangesAsync());
            courseId = course.Id;
        }

        await using LearningDbContext readContext = database.CreateContext();
        Course loaded = await readContext.Courses
            .Include(course => course.Category)
            .Include(course => course.Modules.OrderBy(module => module.Order))
            .SingleAsync(course => course.Id == courseId);

        Assert.Equal("Backend Engineering", loaded.Category.Name);
        Assert.Equal([1, 2], loaded.Modules.Select(module => module.Order));
    }

    [Fact]
    public async Task UniqueIndexes_EnforceDatabaseInvariants()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        context.AddRange(new Category("Architecture"), new Category("Architecture"));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task RestrictDelete_PreventsOrphaningCourses()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        Guid categoryId;
        await using (LearningDbContext seedContext = database.CreateContext())
        {
            var category = new Category("Databases");
            seedContext.Add(category);
            seedContext.Add(new Course(
                category.Id, "Relational Modeling", "relational-modeling", 0m, DateTimeOffset.UtcNow));
            await seedContext.SaveChangesAsync();
            categoryId = category.Id;
        }

        // A fresh unit of work loads only the principal, so the relational FK—not an already tracked
        // navigation fixup—proves that Restrict is enforced by the database.
        await using LearningDbContext deleteContext = database.CreateContext();
        Category principal = await deleteContext.Categories.SingleAsync(item => item.Id == categoryId);
        deleteContext.Remove(principal);

        await Assert.ThrowsAsync<DbUpdateException>(() => deleteContext.SaveChangesAsync());
    }
}
