using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class ChangeTrackingTests
{
    [Fact]
    public async Task TrackingQuery_UsesIdentityMapAndPersistsDetectedChanges()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();

        Course firstQuery = await context.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        Course secondQuery = await context.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        firstQuery.UpdateDetails("ASP.NET Core in Production", firstQuery.Slug, firstQuery.Price);

        Assert.Same(firstQuery, secondQuery);
        Assert.Equal(EntityState.Modified, context.Entry(firstQuery).State);
        Assert.True(context.Entry(firstQuery).Property(course => course.Title).IsModified);
        await context.SaveChangesAsync();
        Assert.Equal(EntityState.Unchanged, context.Entry(firstQuery).State);

        await using LearningDbContext verification = database.CreateContext();
        Assert.Equal(
            "ASP.NET Core in Production",
            await verification.Courses
                .Where(course => course.Id == seeded.FirstCourseId)
                .Select(course => course.Title)
                .SingleAsync());
    }

    [Fact]
    public async Task NoTrackingQuery_DoesNotPersistInMemoryMutation()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();
        Course detached = await context.Courses
            .AsNoTracking()
            .SingleAsync(course => course.Id == seeded.FirstCourseId);

        detached.UpdateDetails("Changed only in memory", detached.Slug, detached.Price);
        Assert.Equal(EntityState.Detached, context.Entry(detached).State);
        Assert.Equal(0, await context.SaveChangesAsync());

        Assert.Equal(
            "ASP.NET Core",
            await context.Courses
                .AsNoTracking()
                .Where(course => course.Id == seeded.FirstCourseId)
                .Select(course => course.Title)
                .SingleAsync());
    }

    [Fact]
    public async Task AttachingSecondInstanceWithSameKey_IsRejected()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext firstContext = database.CreateContext();
        await using LearningDbContext secondContext = database.CreateContext();
        Course tracked = await firstContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        Course disconnectedCopy = await secondContext.Courses
            .AsNoTracking()
            .SingleAsync(course => course.Id == seeded.FirstCourseId);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            firstContext.Attach(disconnectedCopy));

        Assert.Contains("cannot be tracked", exception.Message, StringComparison.OrdinalIgnoreCase);
        Course[] entriesAfterFailure = firstContext.ChangeTracker.Entries<Course>()
            .Select(entry => entry.Entity)
            .ToArray();
        Assert.Contains(entriesAfterFailure, entity => ReferenceEquals(entity, tracked));
        Assert.Contains(entriesAfterFailure, entity => ReferenceEquals(entity, disconnectedCopy));
        // The failed Attach has already contaminated tracker state with two instances. Do not catch
        // this programming error and continue the unit of work; dispose the context instead.
    }

    [Fact]
    public async Task IdentityResolution_ReusesSharedNavigationWithoutLongLivedTracking()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();

        Course[] withoutResolution = await context.Courses
            .AsNoTracking()
            .Include(course => course.Category)
            .OrderBy(course => course.Id)
            .ToArrayAsync();
        Course[] withResolution = await context.Courses
            .AsNoTrackingWithIdentityResolution()
            .Include(course => course.Category)
            .OrderBy(course => course.Id)
            .ToArrayAsync();

        Assert.NotSame(withoutResolution[0].Category, withoutResolution[1].Category);
        Assert.Same(withResolution[0].Category, withResolution[1].Category);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task DisabledAutoDetectChanges_RequiresExplicitDetectionAndRestoration()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();
        Course course = await context.Courses.SingleAsync(item => item.Id == seeded.FirstCourseId);

        bool originalSetting = context.ChangeTracker.AutoDetectChangesEnabled;
        try
        {
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            course.UpdateDetails("Manually detected title", course.Slug, course.Price);
            Assert.Equal(0, await context.SaveChangesAsync());

            context.ChangeTracker.DetectChanges();
            Assert.Equal(1, await context.SaveChangesAsync());
        }
        finally
        {
            context.ChangeTracker.AutoDetectChangesEnabled = originalSetting;
        }

        Assert.True(context.ChangeTracker.AutoDetectChangesEnabled);
    }
}
