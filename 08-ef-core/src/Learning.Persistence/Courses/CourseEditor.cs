using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Courses;

public sealed record UpdateCourseCommand(Guid Id, string Title, string Slug, decimal Price);

public enum UpdateCourseStatus
{
    Updated,
    NotFound
}

public sealed record UpdateCourseResult(UpdateCourseStatus Status, long? Version = null);

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

        course.UpdateDetails(command.Title, command.Slug, command.Price);
        course.IncrementVersion();
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateCourseResult(UpdateCourseStatus.Updated, course.Version);
    }
}
