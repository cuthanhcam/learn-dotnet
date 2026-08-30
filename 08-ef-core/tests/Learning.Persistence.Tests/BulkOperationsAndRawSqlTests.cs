using Learning.Persistence.Courses;
using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class BulkOperationsAndRawSqlTests
{
    [Fact]
    public async Task ExecuteUpdate_ChangesEveryReadyRowWithOneCommand()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        var interceptor = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(interceptor);
        var operations = new CourseBulkOperations(context);
        DateTimeOffset publishedAt = DateTimeOffset.Parse("2026-08-30T10:00:00Z");

        int affected = await operations.PublishReadyCoursesAsync(
            seeded.CategoryId, publishedAt, CancellationToken.None);

        Assert.Equal(2, affected);
        string command = Assert.Single(
            interceptor.Commands,
            sql => sql.StartsWith("UPDATE", StringComparison.Ordinal));
        Assert.Contains("EXISTS", command, StringComparison.OrdinalIgnoreCase);

        await using LearningDbContext verification = database.CreateContext();
        Course[] courses = await verification.Courses.OrderBy(course => course.Slug).ToArrayAsync();
        Assert.All(courses, course =>
        {
            Assert.True(course.IsPublished);
            Assert.Equal(publishedAt, course.PublishedAt);
            Assert.Equal(2, course.Version);
        });
    }

    [Fact]
    public async Task ExecuteUpdate_DoesNotRefreshAnEntityAlreadyInTheIdentityMap()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext context = database.CreateContext();
        Course tracked = await context.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        var operations = new CourseBulkOperations(context);

        await operations.PublishReadyCoursesAsync(
            seeded.CategoryId, DateTimeOffset.Parse("2026-08-30T10:00:00Z"), CancellationToken.None);

        // The in-memory object remains stale because ExecuteUpdate bypasses tracked state. Reload is
        // an explicit boundary; alternatively, perform bulk work in a dedicated short-lived context.
        Assert.False(tracked.IsPublished);
        Assert.Equal(1, tracked.Version);
        await context.Entry(tracked).ReloadAsync();
        Assert.True(tracked.IsPublished);
        Assert.Equal(2, tracked.Version);
    }

    [Fact]
    public async Task ExecuteDelete_RemovesOnlyOneBoundedCleanupBatch()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using (LearningDbContext arrange = database.CreateContext())
        {
            for (int index = 0; index < 4; index++)
            {
                var message = new OutboxMessage(
                    Guid.NewGuid(),
                    DateTimeOffset.Parse("2026-08-01T00:00:00Z").AddMinutes(index),
                    "TestEvent",
                    "{}");
                message.MarkProcessed(DateTimeOffset.Parse("2026-08-02T00:00:00Z").AddMinutes(index));
                arrange.Add(message);
            }

            arrange.Add(new OutboxMessage(
                Guid.NewGuid(), DateTimeOffset.Parse("2026-08-30T00:00:00Z"), "PendingEvent", "{}"));
            await arrange.SaveChangesAsync();
        }

        await using LearningDbContext context = database.CreateContext();
        var operations = new CourseBulkOperations(context);
        int deleted = await operations.DeleteProcessedOutboxMessagesAsync(
            batchSize: 2, CancellationToken.None);

        Assert.Equal(2, deleted);
        Assert.Equal(3, await context.OutboxMessages.CountAsync());
        Assert.Equal(1, await context.OutboxMessages.CountAsync(message => message.ProcessedAt == null));
    }

    [Fact]
    public async Task RawSqlInterpolation_BindsHostileInputAsAParameter()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        var interceptor = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(interceptor);
        var operations = new CourseBulkOperations(context);
        const string hostileInput = "aspnet-core' OR 1=1 --";

        Course? result = await operations.FindBySlugWithSqlAsync(hostileInput, CancellationToken.None);

        Assert.Null(result);
        CommandSnapshot snapshot = Assert.Single(interceptor.Snapshots);
        Assert.DoesNotContain(hostileInput, snapshot.CommandText, StringComparison.Ordinal);
        Assert.Contains(
            snapshot.Parameters,
            parameter => parameter.Value?.ToString() == hostileInput.ToLowerInvariant());
        Assert.Empty(context.ChangeTracker.Entries());
    }
}
