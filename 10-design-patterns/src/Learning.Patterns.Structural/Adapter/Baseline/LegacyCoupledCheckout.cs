using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Adapter.Legacy;

namespace Learning.Patterns.Structural.Adapter.Baseline;

/// <summary>
/// Correct original integration, but application code knows the SDK, grams, status strings, cents,
/// and provider failure types. Repeated checkout callers would repeat these integration decisions.
/// The adapter refactor relocates this coupling behind a client-owned interface.
/// </summary>
public sealed class LegacyCoupledCheckout
{
    private readonly ILegacyRateClient _legacy;

    public LegacyCoupledCheckout(ILegacyRateClient legacy) =>
        _legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));

    public async Task<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        LegacyRateReply reply;
        try
        {
            reply = await _legacy.FetchRateAsync(new LegacyParcel(checked((int)(request.WeightKilograms * 1000m)),
                request.DestinationCountry), cancellationToken).ConfigureAwait(false);
        }
        catch (LegacyServiceUnavailableException)
        {
            return ShippingQuote.TemporarilyUnavailable();
        }
        if (reply is null)
            throw new InvalidDataException("The shipping provider returned no reply.");
        if (reply.Code == "NO_ROUTE")
            return ShippingQuote.NoRoute();
        if (reply.Code == "BUSY")
            return ShippingQuote.TemporarilyUnavailable();
        if (reply.Code != "OK" || reply.PriceCents is null or < 0 or > 100_000_000 || reply.Currency != "USD")
            throw new InvalidDataException("The shipping provider returned invalid quote data.");
        return ShippingQuote.Available(reply.PriceCents.Value / 100m);
    }
}
