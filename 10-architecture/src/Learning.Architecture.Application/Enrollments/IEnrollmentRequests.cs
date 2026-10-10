namespace Learning.Architecture.Application.Enrollments;

public sealed record EnrollmentRequest(Guid RequestId, Guid OfferingId, Guid LearnerId);
public enum EnrollmentRequestStatus { Completed, KeyReused, Conflict }
public sealed record EnrollmentRequestResult(EnrollmentRequestStatus Status,
    EnrollLearnerResult? Enrollment = null, bool Replayed = false);

/// <summary>
/// An atomic workflow boundary: mutation, receipt, and integration intent commit together. This is
/// deliberately stronger than the aggregate persistence port used by the introductory handler.
/// </summary>
public interface IEnrollmentRequests
{
    ValueTask<EnrollmentRequestResult> ExecuteAsync(EnrollmentRequest request,
        CancellationToken cancellationToken = default);
}
