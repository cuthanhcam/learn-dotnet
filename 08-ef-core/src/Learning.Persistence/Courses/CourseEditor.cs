using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Courses;

public sealed record UpdateCourseCommand(
    Guid Id,
    string Title,
    string Slug,
    decimal Price,
    long ExpectedVersion);

public enum UpdateCourseStatus
{
    Updated,
    NotFound,
    Conflict
}

public sealed record CourseConflict(
    string Title,
    string Slug,
    decimal Price,
    long Version);

public sealed record UpdateCourseResult(
    UpdateCourseStatus Status,
    long? Version = null,
    CourseConflict? Current = null);

/// <summary>
/// A disconnected caller supplies values, never an entity instance to attach blindly. The use case
/// reloads the authoritative aggregate, applies allowed behavior, and persists tracked differences.
/// </summary>
public sealed class CourseEditor(LearningDbContext dbContext)
{
    public async Task<UpdateCourseResult> UpdateAsync(
        UpdateCourseCommand command,
        CancellationToken cancellationToken)
    {
        Domain.Course? course = await dbContext.Courses
            .SingleOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
        if (course is null)
        {
            return new UpdateCourseResult(UpdateCourseStatus.NotFound);
        }

        if (course.Version != command.ExpectedVersion)
        {
            return Conflict(course);
        }

        course.UpdateDetails(command.Title, command.Slug, command.Price);
        course.IncrementVersion();
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return new UpdateCourseResult(UpdateCourseStatus.Updated, course.Version);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            // A pre-check improves the common stale-client response, but another transaction can
            // commit after that check. The database UPDATE predicate remains the authoritative guard.
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry = exception.Entries.Single();
            Microsoft.EntityFrameworkCore.ChangeTracking.PropertyValues? databaseValues =
                await entry.GetDatabaseValuesAsync(cancellationToken);
            if (databaseValues is null)
            {
                return new UpdateCourseResult(UpdateCourseStatus.NotFound);
            }

            return new UpdateCourseResult(
                UpdateCourseStatus.Conflict,
                Current: new CourseConflict(
                    databaseValues.GetValue<string>(nameof(Domain.Course.Title))!,
                    databaseValues.GetValue<string>(nameof(Domain.Course.Slug))!,
                    databaseValues.GetValue<decimal>(nameof(Domain.Course.Price)),
                    databaseValues.GetValue<long>(nameof(Domain.Course.Version))));
        }
    }

    private static UpdateCourseResult Conflict(Domain.Course course) => new(
        UpdateCourseStatus.Conflict,
        Current: new CourseConflict(course.Title, course.Slug, course.Price, course.Version));
}
