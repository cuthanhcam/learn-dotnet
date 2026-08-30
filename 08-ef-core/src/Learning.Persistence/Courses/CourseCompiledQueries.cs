using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Learning.Persistence.Courses;

/// <summary>
/// Holds explicitly compiled queries for measured, stable hot paths. EF already caches normal query
/// compilation by expression-tree shape, so this technique should be introduced only after profiling.
/// </summary>
public static class CourseCompiledQueries
{
    private static readonly Func<LearningDbContext, string, int, IAsyncEnumerable<CourseListItem>>
        PublishedByCategoryQuery = EF.CompileAsyncQuery(
            (LearningDbContext context, string categoryName, int take) =>
                context.Courses
                    .AsNoTracking()
                    .TagWith("CourseCompiledQueries.PublishedByCategory")
                    .Where(course => course.IsPublished && course.Category.Name == categoryName)
                    .OrderBy(course => course.Slug)
                    .Take(take)
                    .Select(course => new CourseListItem(
                        course.Id,
                        course.Title,
                        course.Slug,
                        course.Price,
                        course.Category.Name,
                        course.Modules.Count)));

    public static async Task<IReadOnlyList<CourseListItem>> ListPublishedByCategoryAsync(
        LearningDbContext context,
        string categoryName,
        int take,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(categoryName);
        ArgumentOutOfRangeException.ThrowIfLessThan(take, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(take, 100);

        var items = new List<CourseListItem>(take);
        // WithCancellation is essential: IAsyncEnumerable does not receive a CancellationToken as a
        // regular delegate argument from EF. Cancellation must flow while the result is enumerated.
        await foreach (CourseListItem item in PublishedByCategoryQuery(context, categoryName.Trim(), take)
                           .WithCancellation(cancellationToken)
                           .ConfigureAwait(false))
        {
            items.Add(item);
        }

        return items;
    }
}
