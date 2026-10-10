using System.Text.Json;
using Learning.Architecture.Application.Abstractions;
using Learning.Architecture.Application.Offerings;
using Learning.Architecture.Domain.Courses;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Infrastructure.Persistence;

public sealed class EfCourseOfferingStore(EnrollmentDbContext database) : ICourseOfferingStore, IOfferingQueries
{
    public async ValueTask<CourseOffering?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        OfferingRow? row = await database.Offerings.AsNoTracking()
            .SingleOrDefaultAsync(offering => offering.Id == id, cancellationToken).ConfigureAwait(false);
        if (row is null)
            return null;
        Guid[] learners = JsonSerializer.Deserialize<Guid[]>(row.LearnersJson)
            ?? throw new InvalidOperationException("Stored learner collection cannot be null.");
        CourseOffering offering = CourseOffering.Restore(row.Id, row.Title, row.Capacity, row.Version, learners);
        if (offering.EnrolledCount != row.EnrolledCount)
            throw new InvalidOperationException("Stored learner count does not match its snapshot.");
        return offering;
    }

    public async ValueTask<bool> TrySaveAsync(CourseOffering offering, long expectedVersion,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(offering);
        if (expectedVersion < 0 || offering.Version != checked(expectedVersion + 1))
            throw new ArgumentException("A save must advance exactly one version.", nameof(offering));
        string learners = JsonSerializer.Serialize(offering.LearnerIds);
        // One SQL UPDATE performs comparison and mutation atomically. Checking Version in C# and
        // then issuing an unconditional UPDATE would reintroduce the last-seat lost-update bug.
        // ExecuteUpdate executes immediately, bypasses the tracker, and joins any current transaction.
        int affected = await database.Offerings
            .Where(row => row.Id == offering.Id && row.Version == expectedVersion)
            .ExecuteUpdateAsync(update => update
                .SetProperty(row => row.EnrolledCount, offering.EnrolledCount)
                .SetProperty(row => row.LearnersJson, learners)
                .SetProperty(row => row.Version, offering.Version), cancellationToken).ConfigureAwait(false);
        return affected == 1;
    }

    public async ValueTask<OfferingPage> ListAsync(ListOfferingsQuery query, CancellationToken cancellationToken)
    {
        // Projection happens in SQL: catalog queries do not load or deserialize learner identities.
        // Stable ordering is essential for offset pagination, although inserts can still shift pages.
        OfferingSummary[] items = await database.Offerings.AsNoTracking().OrderBy(row => row.Id)
            .Skip(query.Offset).Take(query.Limit)
            .Select(row => new OfferingSummary(row.Id, row.Title, row.Capacity, row.EnrolledCount, row.Version))
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);
        return new OfferingPage(items, query.Offset, query.Limit);
    }

    public static OfferingRow ToRow(CourseOffering offering) => new()
    {
        Id = offering.Id,
        Title = offering.Title,
        Capacity = offering.Capacity,
        EnrolledCount = offering.EnrolledCount,
        Version = offering.Version,
        LearnersJson = JsonSerializer.Serialize(offering.LearnerIds)
    };
}
