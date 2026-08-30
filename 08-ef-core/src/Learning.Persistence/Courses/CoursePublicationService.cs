using System.Text.Json;
using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Learning.Persistence.Courses;

public sealed record PublishCourseCommand(Guid CourseId, long ExpectedVersion, Guid OperationId);

public enum PublishCourseStatus
{
    Published,
    AlreadyProcessed,
    NotFound,
    Conflict
}

public sealed record PublishCourseResult(PublishCourseStatus Status, long? Version = null);

public interface ICoursePublicationHook
{
    Task AfterCourseSavedAsync(CancellationToken cancellationToken);
}

public sealed class NoOpCoursePublicationHook : ICoursePublicationHook
{
    public Task AfterCourseSavedAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Publishes aggregate state and its integration event in one local database transaction. A durable
/// outbox dispatcher can later publish the message without a distributed transaction with a broker.
/// </summary>
public sealed class CoursePublicationService(
    LearningDbContext dbContext,
    TimeProvider timeProvider,
    ICoursePublicationHook hook)
{
    public async Task<PublishCourseResult> PublishAsync(
        PublishCourseCommand command,
        CancellationToken cancellationToken)
    {
        if (command.OperationId == Guid.Empty)
        {
            throw new ArgumentException("Operation identifier is required.", nameof(command));
        }

        IExecutionStrategy strategy = dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            // A retrying provider may execute this entire delegate again. The transaction must be
            // created inside it, and deterministic OperationId provides idempotent replay detection.
            await using IDbContextTransaction transaction =
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

            if (await dbContext.OutboxMessages.AnyAsync(
                    message => message.Id == command.OperationId,
                    cancellationToken))
            {
                await transaction.CommitAsync(cancellationToken);
                return new PublishCourseResult(PublishCourseStatus.AlreadyProcessed);
            }

            Course? course = await dbContext.Courses
                .Include(item => item.Modules)
                .SingleOrDefaultAsync(item => item.Id == command.CourseId, cancellationToken);
            if (course is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new PublishCourseResult(PublishCourseStatus.NotFound);
            }

            if (course.Version != command.ExpectedVersion)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new PublishCourseResult(PublishCourseStatus.Conflict, course.Version);
            }

            DateTimeOffset now = timeProvider.GetUtcNow();
            course.Publish(now);
            await dbContext.SaveChangesAsync(cancellationToken);

            // This hook represents work between persistence stages and allows deterministic failure
            // and cancellation tests without polluting the public command with test-only flags.
            await hook.AfterCourseSavedAsync(cancellationToken);

            string payload = JsonSerializer.Serialize(new CoursePublishedEvent(
                course.Id,
                course.Title,
                course.Version,
                now));
            dbContext.OutboxMessages.Add(new OutboxMessage(
                command.OperationId,
                now,
                "learning.course-published.v1",
                payload));
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new PublishCourseResult(PublishCourseStatus.Published, course.Version);
        });
    }

    private sealed record CoursePublishedEvent(
        Guid CourseId,
        string Title,
        long Version,
        DateTimeOffset PublishedAt);
}
