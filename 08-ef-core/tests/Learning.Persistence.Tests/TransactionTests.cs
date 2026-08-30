using Learning.Persistence.Courses;
using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Learning.Persistence.Tests;

public sealed class TransactionTests
{
    private static readonly DateTimeOffset FixedNow =
        DateTimeOffset.Parse("2026-08-30T12:00:00Z");

    [Fact]
    public async Task Publication_CommitsCourseAndOutboxAtomicallyAcrossTwoSaves()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        Guid operationId = Guid.NewGuid();
        await using (LearningDbContext context = database.CreateContext())
        {
            var service = new CoursePublicationService(
                context,
                new FixedTimeProvider(FixedNow),
                new NoOpCoursePublicationHook());
            PublishCourseResult result = await service.PublishAsync(
                new PublishCourseCommand(seeded.FirstCourseId, ExpectedVersion: 1, operationId),
                CancellationToken.None);

            Assert.Equal(PublishCourseStatus.Published, result.Status);
            Assert.Equal(2, result.Version);
        }

        await using LearningDbContext verification = database.CreateContext();
        Course course = await verification.Courses.AsNoTracking()
            .SingleAsync(item => item.Id == seeded.FirstCourseId);
        OutboxMessage message = await verification.OutboxMessages.AsNoTracking().SingleAsync();
        Assert.True(course.IsPublished);
        Assert.Equal(FixedNow, course.PublishedAt);
        Assert.Equal(operationId, message.Id);
        Assert.Equal("learning.course-published.v1", message.Type);
        Assert.Contains(course.Id.ToString(), message.Payload, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FailureBetweenSaves_RollsBackCourseAndOutbox()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using (LearningDbContext context = database.CreateContext())
        {
            var service = new CoursePublicationService(
                context,
                new FixedTimeProvider(FixedNow),
                new ThrowingHook());

            await Assert.ThrowsAsync<SimulatedDownstreamException>(() => service.PublishAsync(
                new PublishCourseCommand(seeded.FirstCourseId, ExpectedVersion: 1, Guid.NewGuid()),
                CancellationToken.None));
        }

        await using LearningDbContext verification = database.CreateContext();
        Course course = await verification.Courses.AsNoTracking()
            .SingleAsync(item => item.Id == seeded.FirstCourseId);
        Assert.False(course.IsPublished);
        Assert.Equal(1, course.Version);
        Assert.Empty(await verification.OutboxMessages.ToArrayAsync());
    }

    [Fact]
    public async Task ReplayedOperationId_ReturnsAlreadyProcessedWithoutDuplicateEffects()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        Guid operationId = Guid.NewGuid();
        await using LearningDbContext firstContext = database.CreateContext();
        var firstService = new CoursePublicationService(
            firstContext, new FixedTimeProvider(FixedNow), new NoOpCoursePublicationHook());
        PublishCourseResult first = await firstService.PublishAsync(
            new PublishCourseCommand(seeded.FirstCourseId, 1, operationId), CancellationToken.None);

        await using LearningDbContext replayContext = database.CreateContext();
        var replayService = new CoursePublicationService(
            replayContext, new FixedTimeProvider(FixedNow), new NoOpCoursePublicationHook());
        PublishCourseResult replay = await replayService.PublishAsync(
            new PublishCourseCommand(seeded.FirstCourseId, 1, operationId), CancellationToken.None);

        Assert.Equal(PublishCourseStatus.Published, first.Status);
        Assert.Equal(PublishCourseStatus.AlreadyProcessed, replay.Status);
        Assert.Equal(1, await replayContext.OutboxMessages.CountAsync());
        Assert.Equal(2, await replayContext.Courses
            .Where(course => course.Id == seeded.FirstCourseId)
            .Select(course => course.Version)
            .SingleAsync());
    }

    [Fact]
    public async Task OneSaveChanges_UsesImplicitTransactionAndRollsBackEarlierInsertOnConstraintFailure()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        await using (LearningDbContext context = database.CreateContext())
        {
            context.Add(new Category("Will Roll Back"));
            context.Add(new Category("Backend Engineering")); // Existing unique name.
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        await using LearningDbContext verification = database.CreateContext();
        Assert.False(await verification.Categories.AnyAsync(category => category.Name == "Will Roll Back"));
    }

    [Fact]
    public async Task Savepoint_AllowsRecoveringOneStageWithoutDiscardingEarlierTransactionWork()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await using LearningDbContext context = database.CreateContext();
        await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
        context.Add(new Category("Before Savepoint"));
        await context.SaveChangesAsync();
        await transaction.CreateSavepointAsync("before_optional_stage");

        var duplicate = new Category("Before Savepoint");
        context.Add(duplicate);
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        await transaction.RollbackToSavepointAsync("before_optional_stage");
        context.Entry(duplicate).State = EntityState.Detached;

        context.Add(new Category("After Recovery"));
        await context.SaveChangesAsync();
        await transaction.CommitAsync();

        await using LearningDbContext verification = database.CreateContext();
        Assert.True(await verification.Categories.AnyAsync(category => category.Name == "Before Savepoint"));
        Assert.True(await verification.Categories.AnyAsync(category => category.Name == "After Recovery"));
    }

    [Fact]
    public async Task CancellationBetweenSaves_RollsBackAcceptedTransactionWork()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        using var cancellation = new CancellationTokenSource();
        var hook = new CancellationHook();
        await using (LearningDbContext context = database.CreateContext())
        {
            var service = new CoursePublicationService(context, new FixedTimeProvider(FixedNow), hook);
            Task<PublishCourseResult> publishing = service.PublishAsync(
                new PublishCourseCommand(seeded.FirstCourseId, 1, Guid.NewGuid()),
                cancellation.Token);
            await hook.Started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            cancellation.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => publishing);
        }

        await using LearningDbContext verification = database.CreateContext();
        Assert.False(await verification.Courses
            .Where(course => course.Id == seeded.FirstCourseId)
            .Select(course => course.IsPublished)
            .SingleAsync());
        Assert.False(await verification.OutboxMessages.AnyAsync());
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class ThrowingHook : ICoursePublicationHook
    {
        public Task AfterCourseSavedAsync(CancellationToken cancellationToken) =>
            throw new SimulatedDownstreamException();
    }

    private sealed class SimulatedDownstreamException : Exception;

    private sealed class CancellationHook : ICoursePublicationHook
    {
        public TaskCompletionSource Started { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public async Task AfterCourseSavedAsync(CancellationToken cancellationToken)
        {
            Started.TrySetResult();
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
        }
    }
}
