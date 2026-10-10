namespace Learning.Architecture.Application.Offerings;

public sealed record ListOfferingsQuery(int Offset = 0, int Limit = 20);

/// <summary>
/// The read model may share the write database. CQRS here means distinct responsibilities, not a
/// mandatory second database, eventual consistency, or event sourcing.
/// </summary>
public interface IOfferingQueries
{
    ValueTask<OfferingPage> ListAsync(ListOfferingsQuery query, CancellationToken cancellationToken);
}

public sealed class ListOfferingsHandler(IOfferingQueries queries)
{
    public ValueTask<OfferingPage> HandleAsync(ListOfferingsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        // The application protects all callers, including background jobs and the console. An HTTP
        // adapter can give friendlier field errors, but cannot be the only enforcement point.
        if (query.Offset is < 0 or > 10_000)
            throw new ArgumentOutOfRangeException(nameof(query), "Offset must be between 0 and 10000.");
        if (query.Limit is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(query), "Limit must be between 1 and 100.");
        return queries.ListAsync(query, cancellationToken);
    }
}
