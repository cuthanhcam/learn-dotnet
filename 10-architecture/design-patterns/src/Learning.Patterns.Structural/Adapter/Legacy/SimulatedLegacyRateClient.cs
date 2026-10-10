namespace Learning.Patterns.Structural.Adapter.Legacy;

/// <summary>
/// Deterministic offline SDK simulation, not an HTTP implementation. The asynchronous signature models
/// an I/O SDK boundary; no Task.Run, delay, network dependency, or hidden background work is introduced.
/// </summary>
public sealed class SimulatedLegacyRateClient : ILegacyRateClient
{
    public Task<LegacyRateReply> FetchRateAsync(LegacyParcel parcel, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(parcel);
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(parcel.Country switch
        {
            "US" => new LegacyRateReply("OK", 1299, "USD"),
            "DE" => new LegacyRateReply("BUSY", null, null),
            _ => new LegacyRateReply("NO_ROUTE", null, null)
        });
    }
}
