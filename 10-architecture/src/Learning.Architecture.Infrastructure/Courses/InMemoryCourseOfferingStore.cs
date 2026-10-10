using System.Collections.Concurrent;
using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Domain.Courses;

namespace Learning.Architecture.Infrastructure.Courses;

/// <summary>
/// An executable adapter for learning compare-and-swap semantics. Immutable aggregates let readers
/// keep a safe snapshot; ConcurrentDictionary.TryUpdate atomically checks the original object.
/// State is process-local and intentionally lost at restart. This is not a durable database adapter.
/// </summary>
public sealed class InMemoryCourseOfferingStore : ICourseOfferingStore
{
    private readonly ConcurrentDictionary<Guid, CourseOffering> _offerings = new();

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
        if (offering.Version != expectedVersion + 1)
            throw new ArgumentException("A save must advance exactly one version.", nameof(offering));

        bool saved = _offerings.TryGetValue(offering.Id, out CourseOffering? current)
            && current.Version == expectedVersion
            && _offerings.TryUpdate(offering.Id, offering, current);
        return ValueTask.FromResult(saved);
    }
}
