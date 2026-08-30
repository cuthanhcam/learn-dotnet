using Learning.Persistence.Domain;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Courses;

/// <summary>
/// Contains set-based maintenance operations that intentionally bypass the change tracker.
/// Use this style when every matching row receives the same database-expressible change and
/// aggregate behavior does not need to run once per entity.
/// </summary>
public sealed class CourseBulkOperations(LearningDbContext dbContext)
{
    public async Task<int> PublishReadyCoursesAsync(
        Guid categoryId,
        DateTimeOffset publishedAt,
        CancellationToken cancellationToken)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("Category identifier is required.", nameof(categoryId));
        }

        // ExecuteUpdate sends one UPDATE directly to the database. It does not load Course objects,
        // invoke Course.Publish, run DetectChanges, or synchronize already-tracked instances.
        // Therefore every invariant that matters to this maintenance operation must appear in the
        // predicate or be protected by a database constraint.
        return await dbContext.Courses
            .Where(course => course.CategoryId == categoryId)
            .Where(course => !course.IsPublished)
            .Where(course => course.Modules.Any())
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(course => course.IsPublished, true)
                    .SetProperty(course => course.PublishedAt, publishedAt)
                    .SetProperty(course => course.Version, course => course.Version + 1),
                cancellationToken);
    }

    public async Task<int> DeleteProcessedOutboxMessagesAsync(
        int batchSize,
        CancellationToken cancellationToken)
    {
        if (batchSize is < 1 or > 1_000)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be between 1 and 1,000.");
        }

        // ExecuteDelete itself has no Take overload with portable DELETE semantics. Selecting a
        // bounded key set first keeps lock duration and transaction-log growth predictable. The
        // second command still performs one set-based DELETE rather than deleting tracked entities.
        Guid[] ids = await dbContext.OutboxMessages
            .Where(message => message.ProcessedAt != null)
            .OrderBy(message => message.Id)
            .Select(message => message.Id)
            .Take(batchSize)
            .ToArrayAsync(cancellationToken);

        if (ids.Length == 0)
        {
            return 0;
        }

        return await dbContext.OutboxMessages
            .Where(message => ids.Contains(message.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<Course?> FindBySlugWithSqlAsync(string slug, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(slug);
        string normalizedSlug = slug.Trim().ToLowerInvariant();

        // The interpolated value becomes a DbParameter; it is not concatenated into SQL. Raw SQL
        // should be reserved for queries EF cannot express well, and the returned entity still uses
        // normal EF materialization. AsNoTracking makes the read-only intent explicit.
        return dbContext.Courses
            .FromSqlInterpolated($"SELECT * FROM courses WHERE Slug = {normalizedSlug}")
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }
}
