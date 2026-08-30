using Learning.Persistence.Courses;
using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class CourseWorkflowTests
{
    [Fact]
    public async Task ReadQueries_ProjectRequiredDataAndLeaveTrackerEmpty()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();
        var queries = new CourseQueries(context);

        CoursePage page = await queries.ListAsync(1, 10, "Entity", CancellationToken.None);
        CourseDetails? details = await queries.FindAsync(seeded.FirstCourseId, CancellationToken.None);

        CourseListItem item = Assert.Single(page.Items);
        Assert.Equal("Entity Framework Core", item.Title);
        Assert.Equal(1, item.ModuleCount);
        Assert.Equal([1, 2], details!.Modules.Select(module => module.Order));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task DisconnectedUpdate_ReloadsAggregateAndChangesOnlyAllowedFields()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using (LearningDbContext writeContext = database.CreateContext())
        {
            var editor = new CourseEditor(writeContext);
            UpdateCourseResult result = await editor.UpdateAsync(
                new UpdateCourseCommand(
                    seeded.FirstCourseId,
                    "ASP.NET Core Architecture",
                    "aspnet-core-architecture",
                    60m),
                CancellationToken.None);

            Assert.Equal(UpdateCourseStatus.Updated, result.Status);
            Assert.Equal(2, result.Version);
        }

        await using LearningDbContext verification = database.CreateContext();
        Course course = await verification.Courses
            .Include(item => item.Modules)
            .SingleAsync(item => item.Id == seeded.FirstCourseId);
        Assert.Equal("ASP.NET Core Architecture", course.Title);
        Assert.Equal(seeded.CategoryId, course.CategoryId);
        Assert.Equal(2, course.Modules.Count);
    }

    [Fact]
    public async Task DisconnectedUpdate_MissingRowReturnsExpectedOutcome()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        var editor = new CourseEditor(context);

        UpdateCourseResult result = await editor.UpdateAsync(
            new UpdateCourseCommand(Guid.NewGuid(), "Missing", "missing", 1m),
            CancellationToken.None);

        Assert.Equal(UpdateCourseStatus.NotFound, result.Status);
        Assert.Null(result.Version);
        Assert.Empty(context.ChangeTracker.Entries());
    }
}
