using System.Collections.Concurrent;
using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Application.Offerings;

namespace Learning.Architecture.Infrastructure.Courses;

/// <summary>
/// An executable adapter for learning compare-and-swap semantics. Immutable aggregates let readers
/// keep a safe snapshot; ConcurrentDictionary.TryUpdate atomically checks the original object.
/// State is process-local and intentionally lost at restart. This is not a durable database adapter.
/// </summary>
public sealed class InMemoryCourseOfferingStore : ICourseOfferingStore, IOfferingQueries
{
    private readonly ConcurrentDictionary<Guid, CourseOffering> _offerings = new();

    public ValueTask<OfferingPage> ListAsync(ListOfferingsQuery query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Enumerating immutable values is safe, but a page is not a database-wide snapshot. A stable
        // order avoids arbitrary dictionary ordering; concurrent catalog changes can still shift pages.
        OfferingSummary[] items = _offerings.Values.OrderBy(offering => offering.Id)
            .Skip(query.Offset).Take(query.Limit)
            .Select(offering => new OfferingSummary(offering.Id, offering.Title, offering.Capacity,
                offering.EnrolledCount, offering.Version)).ToArray();
        return ValueTask.FromResult(new OfferingPage(items, query.Offset, query.Limit));
    }

    public bool TryAdd(CourseOffering offering)
    {
        ArgumentNullException.ThrowIfNull(offering);
        return _offerings.TryAdd(offering.Id, offering);
    }

    public ValueTask<CourseOffering?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _offerings.TryGetValue(id, out CourseOffering? offering);
        return ValueTask.FromResult(offering);
    }

    public ValueTask<bool> TrySaveAsync(CourseOffering offering, long expectedVersion,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(offering);
        if (expectedVersion < 0 || offering.Version != checked(expectedVersion + 1))
            throw new ArgumentException("A save must advance exactly one version.", nameof(offering));

        bool saved = _offerings.TryGetValue(offering.Id, out CourseOffering? current)
            && current.Version == expectedVersion
            && _offerings.TryUpdate(offering.Id, offering, current);
        return ValueTask.FromResult(saved);
    }
}
