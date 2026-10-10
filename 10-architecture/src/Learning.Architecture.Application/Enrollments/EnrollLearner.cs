using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Domain.Courses;

namespace Learning.Architecture.Application.Enrollments;

public sealed record EnrollLearnerCommand(Guid OfferingId, Guid LearnerId);
public enum EnrollLearnerStatus { Enrolled, AlreadyEnrolled, Full, NotFound, Conflict }
public sealed record EnrollLearnerResult(EnrollLearnerStatus Status, long? Version = null);

/// <summary>
/// A command handler is an application service organized by feature. It coordinates loading, domain
/// behavior, and persistence, while the aggregate alone decides whether a seat can be consumed.
/// CQRS does not require a mediator library, separate database, or asynchronous message broker.
/// </summary>
public sealed class EnrollLearnerHandler(ICourseOfferingStore offerings)
{
    public async ValueTask<EnrollLearnerResult> HandleAsync(EnrollLearnerCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.OfferingId == Guid.Empty || command.LearnerId == Guid.Empty)
            throw new ArgumentException("Offering and learner identifiers are required.", nameof(command));

        CourseOffering? current = await offerings.FindAsync(command.OfferingId, cancellationToken)
            .ConfigureAwait(false);
        if (current is null)
            return new EnrollLearnerResult(EnrollLearnerStatus.NotFound);

        EnrollmentDecision decision = current.Enroll(command.LearnerId);
        if (decision.Outcome == EnrollmentOutcome.AlreadyEnrolled)
            return new EnrollLearnerResult(EnrollLearnerStatus.AlreadyEnrolled, current.Version);
        if (decision.Outcome == EnrollmentOutcome.Full)
            return new EnrollLearnerResult(EnrollLearnerStatus.Full, current.Version);

        bool saved = await offerings.TrySaveAsync(decision.Offering, current.Version, cancellationToken)
            .ConfigureAwait(false);
        // A conflict is explicit. The caller may reload and retry a bounded number of times; the
        // handler does not blindly repeat side effects or turn a conflict into a generic server error.
        return saved
            ? new EnrollLearnerResult(EnrollLearnerStatus.Enrolled, decision.Offering.Version)
            : new EnrollLearnerResult(EnrollLearnerStatus.Conflict);
    }
}
