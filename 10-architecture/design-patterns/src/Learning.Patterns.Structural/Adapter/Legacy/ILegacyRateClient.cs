namespace Learning.Patterns.Structural.Adapter.Legacy;

// Simulated third-party SDK types. Keep their awkward units/status vocabulary so the adapter's
// purpose remains visible; these are not the contract the application should adopt as its own.
public sealed record LegacyParcel(int Grams, string Country);
public sealed record LegacyRateReply(string Code, long? PriceCents, string? Currency);

public interface ILegacyRateClient
{
    Task<LegacyRateReply> FetchRateAsync(LegacyParcel parcel, CancellationToken cancellationToken);
}

/// <summary>A specifically documented transient SDK failure, not a catch-all exception category.</summary>
public sealed class LegacyServiceUnavailableException : Exception
{
    public LegacyServiceUnavailableException() : base("Legacy shipping service is unavailable.") { }
}
