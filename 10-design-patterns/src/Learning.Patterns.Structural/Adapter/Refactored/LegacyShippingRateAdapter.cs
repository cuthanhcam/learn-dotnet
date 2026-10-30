using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Adapter.Legacy;

namespace Learning.Patterns.Structural.Adapter.Refactored;

/// <summary>
/// Object Adapter: implements the target contract and composes the incompatible SDK. The caller owns
/// the SDK client's lifetime; quoting does not dispose it. This boundary translates meaning, not just
/// method names. It does not perform currency conversion or silently retry provider calls.
/// </summary>
public sealed class LegacyShippingRateAdapter : IShippingRates
{
    private readonly ILegacyRateClient _legacy;

    public LegacyShippingRateAdapter(ILegacyRateClient legacy) =>
        _legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));

    public async ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        var parcel = new LegacyParcel(checked((int)(request.WeightKilograms * 1000m)), request.DestinationCountry);
        LegacyRateReply reply;
        try
        {
            reply = await _legacy.FetchRateAsync(parcel, cancellationToken).ConfigureAwait(false);
        }
        catch (LegacyServiceUnavailableException)
        {
            return ShippingQuote.TemporarilyUnavailable();
        }
        if (reply is null)
            throw new InvalidDataException("The shipping provider returned no reply.");
        return reply.Code switch
        {
            "NO_ROUTE" => ShippingQuote.NoRoute(),
            "BUSY" => ShippingQuote.TemporarilyUnavailable(),
            "OK" => TranslatePrice(reply),
            _ => throw new InvalidDataException("The shipping provider returned an unknown status.")
        };
    }

    private static ShippingQuote TranslatePrice(LegacyRateReply reply)
    {
        if (reply.PriceCents is null or < 0 or > 100_000_000 || reply.Currency != "USD")
            throw new InvalidDataException("The shipping provider returned invalid price or currency data.");
        // Decimal division preserves cents. Integer division would truncate 1299 to 12, while double
        // would introduce a binary floating-point representation into a monetary contract.
        return ShippingQuote.Available(reply.PriceCents.Value / 100m);
    }
}
