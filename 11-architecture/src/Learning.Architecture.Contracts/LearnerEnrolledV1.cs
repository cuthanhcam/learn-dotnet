namespace Learning.Architecture.Contracts;

/// <summary>
/// Integration data owned by the enrollment publisher. Only identifiers needed by the subscriber
/// cross the boundary. No aggregate, database entity, token, or learner email address is shared.
/// A breaking change needs a new contract version and an explicit compatibility plan.
/// </summary>
public sealed record LearnerEnrolledV1(Guid MessageId, Guid OfferingId, Guid LearnerId);

public interface IEnrollmentEventSink
{
    ValueTask AcceptAsync(LearnerEnrolledV1 message, CancellationToken cancellationToken);
}
