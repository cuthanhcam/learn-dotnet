using Learning.Persistence.Courses;
using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class QueryPerformanceTests
{
    [Fact]
    public async Task ExplicitLoadingInsideLoop_DemonstratesNPlusOneRoundTrips()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);

        Course[] courses = await context.Courses.OrderBy(course => course.Title).ToArrayAsync();
        foreach (Course course in courses)
        {
            // This innocent-looking loop produces one module query per course. With 1,000 courses it
            // becomes 1,001 round trips, even though every individual statement is fast.
            await context.Entry(course).Collection(item => item.Modules).LoadAsync();
        }

        Assert.Equal(3, capture.Commands.Count);
        Assert.Equal([2, 1], courses.Select(course => course.Modules.Count));
    }

    [Fact]
    public async Task EagerLoading_KnownGraphUsesOneRoundTrip()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);

        Course[] courses = await context.Courses
            .Include(course => course.Modules)
            .OrderBy(course => course.Title)
            .ToArrayAsync();

        Assert.Equal(2, courses.Length);
        Assert.Single(capture.Commands);
        Assert.Contains("LEFT JOIN", capture.Commands[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ProjectedPage_UsesCountAndNarrowDataQueryWithoutTracking()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);
        var queries = new CourseQueries(context);

        CoursePage page = await queries.ListAsync(1, 20, null, CancellationToken.None);

        Assert.Equal(2, page.TotalCount);
        Assert.Equal(2, capture.Commands.Count);
        Assert.Contains("COUNT(*)", capture.Commands[0], StringComparison.OrdinalIgnoreCase);
        // The data projection also contains a correlated COUNT(*) for ModuleCount, so command order
        // is a clearer distinction than searching for the aggregate token.
        string dataSql = capture.Commands[1];
        Assert.DoesNotContain("CreatedAt", dataSql, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Version", dataSql, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task KeysetPagination_ContinuesAfterStableUniqueSlugCursor()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);
        var queries = new CourseQueries(context);

        IReadOnlyList<CourseListItem> firstPage =
            await queries.ListAfterSlugAsync(null, 1, CancellationToken.None);
        IReadOnlyList<CourseListItem> secondPage =
            await queries.ListAfterSlugAsync(firstPage[0].Slug, 1, CancellationToken.None);

        Assert.Equal("aspnet-core", Assert.Single(firstPage).Slug);
        Assert.Equal("ef-core", Assert.Single(secondPage).Slug);
        Assert.Contains("CourseQueries.ListAfterSlug", capture.Commands[0], StringComparison.Ordinal);
        Assert.Contains("WHERE", capture.Commands[1], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("OFFSET", capture.Commands[1], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UnsupportedClientMethod_ThrowsInsteadOfSilentlyLoadingEveryRow()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();

        IQueryable<Course> query = context.Courses.Where(course => HasEvenWordCount(course.Title));

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            query.ToArrayAsync());
        Assert.Contains("could not be translated", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasEvenWordCount(string value) =>
        value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length % 2 == 0;
}
