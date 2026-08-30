using System.Collections.Concurrent;
using Learning.Persistence.Courses;
using Learning.Persistence.Diagnostics;
using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class CompiledQueriesAndDiagnosticsTests
{
    [Fact]
    public async Task CompiledQuery_ProjectsPublishedRowsWithoutTrackingEntities()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using (LearningDbContext publisher = database.CreateContext())
        {
            await new CourseBulkOperations(publisher).PublishReadyCoursesAsync(
                seeded.CategoryId, DateTimeOffset.Parse("2026-08-30T12:00:00Z"), CancellationToken.None);
        }

        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);

        IReadOnlyList<CourseListItem> items = await CourseCompiledQueries.ListPublishedByCategoryAsync(
            context, "Backend Engineering", take: 10, CancellationToken.None);

        Assert.Equal(["aspnet-core", "ef-core"], items.Select(item => item.Slug));
        Assert.Single(capture.Commands);
        Assert.Contains("CourseCompiledQueries.PublishedByCategory", capture.Commands[0], StringComparison.Ordinal);
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task CompiledQuery_ValidatesInputsBeforeDatabaseExecution()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            CourseCompiledQueries.ListPublishedByCategoryAsync(
                context, "Backend Engineering", take: 101, CancellationToken.None));

        Assert.Empty(capture.Commands);
    }

    [Fact]
    public async Task MetricsInterceptor_RecordsSuccessfulCommandWithoutSqlOrParameters()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        await CourseTestData.SeedTwoCoursesAsync(database);
        var sink = new RecordingCommandObservationSink();
        var metrics = new CommandMetricsInterceptor(sink);
        await using LearningDbContext context = database.CreateContext(metrics);

        _ = await context.Courses.AsNoTracking().CountAsync();

        CommandObservation observation = Assert.Single(sink.Observations);
        Assert.True(observation.Succeeded);
        Assert.Null(observation.ErrorType);
        Assert.True(observation.Duration >= TimeSpan.Zero);
        // EF relational queries commonly execute through a data reader even when LINQ ultimately
        // produces one scalar value; telemetry records the actual ADO.NET execution method.
        Assert.Equal("ExecuteReader", observation.Operation);
        // CommandObservation structurally cannot carry command text or parameter values. This makes
        // safe telemetry the default rather than relying on every caller to remember redaction.
    }

    [Fact]
    public async Task MetricsInterceptor_RecordsProviderFailureAndPreservesTheException()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        var sink = new RecordingCommandObservationSink();
        var metrics = new CommandMetricsInterceptor(sink);
        await using LearningDbContext context = database.CreateContext(metrics);
        context.AddRange(new Category("Duplicate"), new Category("Duplicate"));

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        CommandObservation failure = Assert.Single(sink.Observations, item => !item.Succeeded);
        Assert.NotNull(failure.ErrorType);
    }

    private sealed class RecordingCommandObservationSink : ICommandObservationSink
    {
        private readonly ConcurrentQueue<CommandObservation> _observations = new();
        public IReadOnlyList<CommandObservation> Observations => _observations.ToArray();
        public void Record(CommandObservation observation) => _observations.Enqueue(observation);
    }
}
