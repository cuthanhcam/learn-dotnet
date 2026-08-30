using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class RelationshipLoadingTests
{
    [Fact]
    public void ExplicitJoinEntity_RejectsDuplicateAssociationBeforeDatabaseWork()
    {
        var course = new Course(
            Guid.NewGuid(), "Course", "course", 10m, DateTimeOffset.UtcNow);
        Guid tagId = Guid.NewGuid();
        course.AddTag(tagId);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            course.AddTag(tagId));

        Assert.Contains("already contains", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SingleQuery_WithTwoSiblingCollectionsUsesOneCommandButCrossJoinsRows()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);

        Course course = await context.Courses
            .AsSingleQuery()
            .Include(item => item.Modules)
            .Include(item => item.CourseTags)
                .ThenInclude(link => link.Tag)
            .SingleAsync(item => item.Id == seeded.FirstCourseId);

        Assert.Equal(2, course.Modules.Count);
        Assert.Equal(2, course.CourseTags.Count);
        string sql = Assert.Single(capture.Commands);
        Assert.True(CountOccurrences(sql, "LEFT JOIN") >= 2);
        // Two modules × two tags produce four relational rows before identity resolution rebuilds
        // one course object. Larger sibling collections multiply far more dramatically.
    }

    [Fact]
    public async Task SplitQuery_AvoidsSiblingCollectionCrossProductUsingThreeCommands()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);

        Course course = await context.Courses
            .AsSplitQuery()
            .Include(item => item.Modules)
            .Include(item => item.CourseTags)
                .ThenInclude(link => link.Tag)
            .SingleAsync(item => item.Id == seeded.FirstCourseId);

        Assert.Equal(2, course.Modules.Count);
        Assert.Equal(2, course.CourseTags.Count);
        Assert.Equal(3, capture.Commands.Count);
    }

    [Fact]
    public async Task FilteredInclude_TrackingFixupCanReintroducePreviouslyLoadedChildren()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();

        Course fullyLoaded = await context.Courses
            .Include(course => course.Modules)
            .SingleAsync(course => course.Id == seeded.FirstCourseId);
        Assert.Equal(2, fullyLoaded.Modules.Count);

        Course filteredInSameContext = await context.Courses
            .Include(course => course.Modules.Where(module => module.Order > 1))
            .SingleAsync(course => course.Id == seeded.FirstCourseId);

        Assert.Same(fullyLoaded, filteredInSameContext);
        Assert.Equal(2, filteredInSameContext.Modules.Count);

        await using LearningDbContext freshContext = database.CreateContext();
        Course filteredWithoutPriorTracking = await freshContext.Courses
            .AsNoTracking()
            .Include(course => course.Modules.Where(module => module.Order > 1))
            .SingleAsync(course => course.Id == seeded.FirstCourseId);
        Assert.Single(filteredWithoutPriorTracking.Modules);
        Assert.Equal(2, filteredWithoutPriorTracking.Modules.Single().Order);
    }

    [Fact]
    public async Task DeletingCourse_CascadesOwnedLinksButPreservesSharedTags()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using (LearningDbContext deleteContext = database.CreateContext())
        {
            Course course = await deleteContext.Courses
                .SingleAsync(item => item.Id == seeded.FirstCourseId);
            deleteContext.Remove(course);
            await deleteContext.SaveChangesAsync();
        }

        await using LearningDbContext verification = database.CreateContext();
        Assert.False(await verification.CourseModules.AnyAsync(module => module.CourseId == seeded.FirstCourseId));
        Assert.False(await verification.CourseTags.AnyAsync(link => link.CourseId == seeded.FirstCourseId));
        Assert.Equal(2, await verification.Tags.CountAsync());
        Assert.True(await verification.Courses.AnyAsync(course => course.Id == seeded.SecondCourseId));
    }

    private static int CountOccurrences(string value, string token)
    {
        int count = 0;
        int index = 0;
        while ((index = value.IndexOf(token, index, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            count++;
            index += token.Length;
        }

        return count;
    }
}
