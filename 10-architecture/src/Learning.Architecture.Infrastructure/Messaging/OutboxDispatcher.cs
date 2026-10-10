using Learning.Architecture.Contracts;
using Learning.Architecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Infrastructure.Messaging;

/// <summary>
/// A bounded, single-dispatcher learning adapter. Deliver THEN acknowledge: a crash in between causes
/// redelivery, not loss. A production multi-worker dispatcher also needs leasing/claiming, poison
/// handling, retry schedules, retention, and telemetry; this class does not silently promise those.
/// </summary>
public sealed class OutboxDispatcher(EnrollmentDbContext database, IEnrollmentEventSink sink)
{
    public async Task<int> DispatchAsync(int limit = 20, CancellationToken cancellationToken = default)
    {
        if (limit is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(limit));
        OutboxRow[] messages = await database.Outbox.AsNoTracking().Where(row => !row.Dispatched)
            .OrderBy(row => row.MessageId).Take(limit).ToArrayAsync(cancellationToken).ConfigureAwait(false);
        int delivered = 0;
        foreach (OutboxRow row in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (row.SchemaVersion != 1)
                throw new InvalidOperationException("Unsupported enrollment event schema; delivery must not be acknowledged.");
            await sink.AcceptAsync(new LearnerEnrolledV1(row.MessageId, row.OfferingId, row.LearnerId),
                cancellationToken).ConfigureAwait(false);
            await database.Outbox.Where(item => item.MessageId == row.MessageId && !item.Dispatched)
                .ExecuteUpdateAsync(update => update.SetProperty(item => item.Dispatched, true), cancellationToken)
                .ConfigureAwait(false);
            delivered++;
        }
        return delivered;
    }
}
