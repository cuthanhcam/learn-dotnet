namespace Learning.Architecture.Application.Offerings;

/// <summary>
/// A read contract, not a domain aggregate or an EF entity. Learner identities deliberately stay out
/// of the public catalog. Returning a materialized value also keeps IQueryable out of application ports.
/// </summary>
public sealed record OfferingSummary(Guid Id, string Title, int Capacity, int EnrolledCount, long Version)
{
    public int AvailableSeats => Capacity - EnrolledCount;
}

public sealed record OfferingPage(IReadOnlyList<OfferingSummary> Items, int Offset, int Limit);
