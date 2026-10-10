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
    /// Saves only if the stored version still equals expectedVersion. False means a concurrent change
    /// or deletion; callers must not present it as a successful enrollment. Database implementations
    /// should use a concurrency token or a conditional update in a transaction.
    /// </summary>
    ValueTask<bool> TrySaveAsync(CourseOffering offering, long expectedVersion,
        CancellationToken cancellationToken);
}
