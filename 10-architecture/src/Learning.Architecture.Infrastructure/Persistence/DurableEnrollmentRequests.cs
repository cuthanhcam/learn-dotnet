using Learning.Architecture.Application.Enrollments;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Infrastructure.Persistence;

/// <summary>
/// Decorates the same application handler with relational coordination. The domain still decides
/// enrollment rules; this adapter owns database transaction and durable request-replay mechanics.
/// A fresh scoped DbContext is required per invocation, including retries after a failed invocation.
/// </summary>
public sealed class DurableEnrollmentRequests(EnrollmentDbContext database) : IEnrollmentRequests
{
    public async ValueTask<EnrollmentRequestResult> ExecuteAsync(EnrollmentRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.RequestId == Guid.Empty || request.OfferingId == Guid.Empty || request.LearnerId == Guid.Empty)
            throw new ArgumentException("Request, offering, and learner identifiers are required.", nameof(request));

        await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken)
            .ConfigureAwait(false);
        EnrollmentReceiptRow? receipt = await database.Receipts.AsNoTracking()
            .SingleOrDefaultAsync(row => row.RequestId == request.RequestId, cancellationToken)
            .ConfigureAwait(false);
        if (receipt is not null)
        {
            // The key is bound to semantic input. A key replay with different input is a client error,
            // not permission to return another operation's success or to mutate a different offering.
            if (receipt.OfferingId != request.OfferingId || receipt.LearnerId != request.LearnerId)
                return new EnrollmentRequestResult(EnrollmentRequestStatus.KeyReused);
            return new EnrollmentRequestResult(EnrollmentRequestStatus.Completed,
                new EnrollLearnerResult((EnrollLearnerStatus)receipt.Status, receipt.Version), Replayed: true);
        }

        var handler = new EnrollLearnerHandler(new EfCourseOfferingStore(database));
        EnrollLearnerResult result = await handler.HandleAsync(
            new EnrollLearnerCommand(request.OfferingId, request.LearnerId), cancellationToken).ConfigureAwait(false);
        if (result.Status == EnrollLearnerStatus.Conflict)
            return new EnrollmentRequestResult(EnrollmentRequestStatus.Conflict);

        database.Receipts.Add(new EnrollmentReceiptRow
        {
            RequestId = request.RequestId,
            OfferingId = request.OfferingId,
            LearnerId = request.LearnerId,
            Status = (int)result.Status,
            Version = result.Version
        });
        if (result.Status == EnrollLearnerStatus.Enrolled)
        {
            // No email or broker call runs inside the transaction. The outbox records delivery intent;
            // another process/iteration may deliver it at least once after the commit succeeds.
            database.Outbox.Add(new OutboxRow
            {
                MessageId = request.RequestId,
                OfferingId = request.OfferingId,
                LearnerId = request.LearnerId
            });
        }

        await database.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        return new EnrollmentRequestResult(EnrollmentRequestStatus.Completed, result);
        // Disposal rolls back on exceptions, including cancellation. A disconnect during commit can
        // leave the caller uncertain; retry the SAME key with a NEW scope to discover the durable result.
        // Provider lock/busy errors deliberately propagate: they are not semantic capacity conflicts.
    }
}
