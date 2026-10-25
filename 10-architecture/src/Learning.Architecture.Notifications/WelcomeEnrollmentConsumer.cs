using Learning.Architecture.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Notifications;

public sealed class WelcomeEnrollmentConsumer(IDbContextFactory<WelcomeDbContext> contexts) : IEnrollmentEventSink
{
    public async ValueTask AcceptAsync(LearnerEnrolledV1 message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (message.MessageId == Guid.Empty || message.OfferingId == Guid.Empty || message.LearnerId == Guid.Empty)
            throw new ArgumentException("Integration identifiers cannot be empty.", nameof(message));
        await using WelcomeDbContext database = await contexts.CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);
        WelcomeWorkItem? existing = await database.WorkItems.AsNoTracking()
            .SingleOrDefaultAsync(item => item.MessageId == message.MessageId, cancellationToken)
            .ConfigureAwait(false);
        if (existing is not null)
        {
            if (existing.OfferingId != message.OfferingId || existing.LearnerId != message.LearnerId)
                throw new InvalidOperationException("A message identifier was reused with different integration data.");
            return;
        }
        database.WorkItems.Add(new WelcomeWorkItem
        {
            MessageId = message.MessageId,
            OfferingId = message.OfferingId,
            LearnerId = message.LearnerId
        });
        // The unique key is the final defense if two deliveries both miss the initial lookup. A
        // competing insert may throw; the transport retries with a fresh context and then deduplicates.
        // Never catch every DbUpdateException and pretend it means a successful duplicate delivery.
        await database.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
