namespace Learning.Patterns.Structural.Adapter.Contracts;

public enum ShippingQuoteStatus { Available, NoRoute, TemporarilyUnavailable }

/// <summary>The client sees typed outcomes and a USD amount, not legacy status strings or cents.</summary>
public sealed record ShippingQuote
{
    private ShippingQuote(ShippingQuoteStatus status, decimal? amountUsd)
    {
        Status = status;
        AmountUsd = amountUsd;
    }

    public ShippingQuoteStatus Status { get; }
    public decimal? AmountUsd { get; }

    public static ShippingQuote Available(decimal amountUsd)
    {
        if (amountUsd is < 0 or > 1_000_000 || decimal.Round(amountUsd, 2) != amountUsd)
            throw new ArgumentOutOfRangeException(nameof(amountUsd));
        return new ShippingQuote(ShippingQuoteStatus.Available, amountUsd);
    }

    public static ShippingQuote NoRoute() => new(ShippingQuoteStatus.NoRoute, null);
    public static ShippingQuote TemporarilyUnavailable() => new(ShippingQuoteStatus.TemporarilyUnavailable, null);
}
