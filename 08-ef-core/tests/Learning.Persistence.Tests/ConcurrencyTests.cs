using Learning.Persistence.Courses;
using Learning.Persistence.Domain;
using Learning.Persistence.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Tests;

public sealed class ConcurrencyTests
{
    [Fact]
    public async Task ConcurrentUpdates_SecondWriterReceivesDatabaseValues()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext firstContext = database.CreateContext();
        await using LearningDbContext secondContext = database.CreateContext();
        Course first = await firstContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        Course stale = await secondContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);

        first.UpdateDetails("First writer", first.Slug, 70m);
        first.IncrementVersion();
        await firstContext.SaveChangesAsync();
        stale.UpdateDetails("Stale writer", stale.Slug, 1m);
        stale.IncrementVersion();

        DbUpdateConcurrencyException exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => secondContext.SaveChangesAsync());
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry = Assert.Single(exception.Entries);
        Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues databaseValues =
            (await entry.GetDatabaseValuesAsync())!;

        Assert.Equal(1L, entry.OriginalValues.GetValue<long>(nameof(Course.Version)));
        Assert.Equal(2L, entry.CurrentValues.GetValue<long>(nameof(Course.Version)));
        Assert.Equal(2L, databaseValues.GetValue<long>(nameof(Course.Version)));
        Assert.Equal("First writer", databaseValues.GetValue<string>(nameof(Course.Title)));
    }

    [Fact]
    public async Task StaleDisconnectedCommand_ReturnsConflictWithoutAttemptingUpdate()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using (LearningDbContext concurrentContext = database.CreateContext())
        {
            Course concurrent = await concurrentContext.Courses
                .SingleAsync(course => course.Id == seeded.FirstCourseId);
            concurrent.UpdateDetails("Already changed", concurrent.Slug, 80m);
            concurrent.IncrementVersion();
            await concurrentContext.SaveChangesAsync();
        }

        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);
        var editor = new CourseEditor(context);
        UpdateCourseResult result = await editor.UpdateAsync(
            new UpdateCourseCommand(
                seeded.FirstCourseId, "Client edit", "client-edit", 1m, ExpectedVersion: 1),
            CancellationToken.None);

        Assert.Equal(UpdateCourseStatus.Conflict, result.Status);
        Assert.Equal(2, result.Current!.Version);
        Assert.Equal("Already changed", result.Current.Title);
        Assert.Single(capture.Commands); // SELECT only; no UPDATE was attempted.
    }

    [Fact]
    public async Task ClientWinsRetry_MustAdoptDatabaseOriginalsAndAdvanceVersionAgain()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext winnerContext = database.CreateContext();
        await using LearningDbContext retryContext = database.CreateContext();
        Course winner = await winnerContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        Course retry = await retryContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        winner.UpdateDetails("Store value", winner.Slug, 100m);
        winner.IncrementVersion();
        await winnerContext.SaveChangesAsync();
        retry.UpdateDetails("Explicit client wins", retry.Slug, 5m);
        retry.IncrementVersion();

        DbUpdateConcurrencyException exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => retryContext.SaveChangesAsync());
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry = Assert.Single(exception.Entries);
        Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues databaseValues =
            (await entry.GetDatabaseValuesAsync())!;

        entry.OriginalValues.SetValues(databaseValues);
        retry.AdvanceVersionFrom(databaseValues.GetValue<long>(nameof(Course.Version)));
        await retryContext.SaveChangesAsync();

        await using LearningDbContext verification = database.CreateContext();
        Course persisted = await verification.Courses
            .AsNoTracking()
            .SingleAsync(course => course.Id == seeded.FirstCourseId);
        Assert.Equal("Explicit client wins", persisted.Title);
        Assert.Equal(3, persisted.Version);
    }

    [Fact]
    public async Task StoreWinsResolution_ReplacesCurrentAndOriginalValuesWithoutSecondWrite()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext storeContext = database.CreateContext();
        await using LearningDbContext staleContext = database.CreateContext();
        Course store = await storeContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        Course stale = await staleContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        store.UpdateDetails("Store remains authoritative", store.Slug, 90m);
        store.IncrementVersion();
        await storeContext.SaveChangesAsync();
        stale.UpdateDetails("Discard this edit", stale.Slug, 2m);
        stale.IncrementVersion();

        DbUpdateConcurrencyException exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => staleContext.SaveChangesAsync());
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry = Assert.Single(exception.Entries);
        Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues databaseValues =
            (await entry.GetDatabaseValuesAsync())!;
        entry.CurrentValues.SetValues(databaseValues);
        entry.OriginalValues.SetValues(databaseValues);
        entry.State = EntityState.Unchanged;

        Assert.Equal("Store remains authoritative", stale.Title);
        Assert.Equal(0, await staleContext.SaveChangesAsync());
    }

    [Fact]
    public async Task StaleDelete_ThrowsWhenAnotherWriterUpdatedVersion()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        await using LearningDbContext updateContext = database.CreateContext();
        await using LearningDbContext deleteContext = database.CreateContext();
        Course updated = await updateContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        Course staleDelete = await deleteContext.Courses.SingleAsync(course => course.Id == seeded.FirstCourseId);
        updated.UpdateDetails("Updated before delete", updated.Slug, updated.Price);
        updated.IncrementVersion();
        await updateContext.SaveChangesAsync();
        deleteContext.Remove(staleDelete);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => deleteContext.SaveChangesAsync());
    }

    [Fact]
    public async Task UpdateSql_IncludesPrimaryKeyAndOriginalVersionPredicate()
    {
        await using var database = new SqliteTestDatabase();
        await database.InitializeAsync();
        SeededCourses seeded = await CourseTestData.SeedTwoCoursesAsync(database);
        var capture = new CommandCaptureInterceptor();
        await using LearningDbContext context = database.CreateContext(capture);
        Course course = await context.Courses.SingleAsync(item => item.Id == seeded.FirstCourseId);
        course.UpdateDetails("Inspect SQL", course.Slug, course.Price);
        course.IncrementVersion();
        await context.SaveChangesAsync();

        string updateSql = capture.Commands.Single(command =>
            command.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("WHERE \"Id\" = @p", updateSql, StringComparison.Ordinal);
        Assert.Contains("AND \"Version\" = @p", updateSql, StringComparison.Ordinal);
    }
}
