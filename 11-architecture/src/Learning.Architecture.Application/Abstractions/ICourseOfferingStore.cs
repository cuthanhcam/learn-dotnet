using Learning.Architecture.Domain.Courses;

namespace Learning.Architecture.Application.Abstractions;

/// <summary>
/// A use-case-specific persistence port. Compare-and-save is part of its behavioral contract;
/// replacing it with a generic Update method would hide the concurrency requirement.
/// </summary>
public interface ICourseOfferingStore
{
    ValueTask<CourseOffering?> FindAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Saves the next enrollment snapshot only if the stored version equals expectedVersion and its
    /// immutable title/capacity match the supplied snapshot. The snapshot must advance exactly one
    /// version. False means a missing or incompatible stored state; it must not be reported as success.
    /// Callers derive the snapshot with Enroll on the loaded aggregate, not arbitrary reconstruction.
    /// Database implementations must perform comparison and mutation atomically.
    /// </summary>
    ValueTask<bool> TrySaveAsync(CourseOffering offering, long expectedVersion,
        CancellationToken cancellationToken);
}
