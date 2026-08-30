using Learning.Persistence.Domain;

namespace Learning.Persistence.Tests.Infrastructure;

public sealed record SeededCourses(Guid CategoryId, Guid FirstCourseId, Guid SecondCourseId);

public static class CourseTestData
{
    public static async Task<SeededCourses> SeedTwoCoursesAsync(SqliteTestDatabase database)
    {
        await using LearningDbContext context = database.CreateContext();
        var category = new Category("Backend Engineering");
        var first = new Course(
            category.Id, "ASP.NET Core", "aspnet-core", 40m, DateTimeOffset.Parse("2026-08-30T00:00:00Z"));
        first.AddModule("Hosting");
        first.AddModule("Middleware");
        var second = new Course(
            category.Id, "Entity Framework Core", "ef-core", 50m, DateTimeOffset.Parse("2026-08-30T00:00:00Z"));
        second.AddModule("DbContext");
        context.AddRange(category, first, second);
        await context.SaveChangesAsync();
        return new SeededCourses(category.Id, first.Id, second.Id);
    }
}
