using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Courses;

/// <summary>
/// Read-only use cases project directly to transport-neutral read models. They do not materialize
/// full entity graphs merely to discard most columns, and they do not leave entities tracked.
/// </summary>
public sealed class CourseQueries(LearningDbContext dbContext)
{
    public async Task<CoursePage> ListAsync(
        int page,
        int pageSize,
        string? titlePrefix,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(page);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, 100);

        IQueryable<Domain.Course> query = dbContext.Courses.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(titlePrefix))
        {
            string normalizedPrefix = titlePrefix.Trim();
            // StartsWith remains part of the expression tree and is translated by the provider.
            // Calling AsEnumerable before this point would silently move filtering to the client.
            query = query.Where(course => course.Title.StartsWith(normalizedPrefix));
        }

        int totalCount = await query.CountAsync(cancellationToken);
        int skip = checked((page - 1) * pageSize);
        CourseListItem[] items = await query
            .OrderBy(course => course.Title)
            .ThenBy(course => course.Id)
            .Skip(skip)
            .Take(pageSize)
            .Select(course => new CourseListItem(
                course.Id,
                course.Title,
                course.Slug,
                course.Price,
                course.Category.Name,
                course.Modules.Count))
            .ToArrayAsync(cancellationToken);

        return new CoursePage(items, page, pageSize, totalCount);
    }

    public Task<CourseDetails?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Courses
            .AsNoTracking()
            .Where(course => course.Id == id)
            .Select(course => new CourseDetails(
                course.Id,
                course.Title,
                course.Slug,
                course.Price,
                course.Version,
                course.Category.Name,
                course.Modules
                    .OrderBy(module => module.Order)
                    .Select(module => new CourseModuleDetails(module.Id, module.Order, module.Title))
                    .ToArray()))
            .SingleOrDefaultAsync(cancellationToken);
}
