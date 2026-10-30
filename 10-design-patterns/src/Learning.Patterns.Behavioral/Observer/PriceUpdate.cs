namespace Learning.Patterns.Behavioral.Observer;

/// <summary>An immutable snapshot shared with subscribers; no mutable publisher state is exposed.</summary>
public sealed record PriceUpdate
{
    public PriceUpdate(string productCode, decimal priceUsd)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);
        string code = productCode.Trim();
        if (code.Length > 32)
            throw new ArgumentOutOfRangeException(nameof(productCode));
        if (priceUsd is < 0 or > 1_000_000 || decimal.Round(priceUsd, 2) != priceUsd)
            throw new ArgumentOutOfRangeException(nameof(priceUsd));
        ProductCode = code;
        PriceUsd = priceUsd;
    }

    public string ProductCode { get; }
    public decimal PriceUsd { get; }
}
