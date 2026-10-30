namespace Learning.Patterns.Behavioral.Strategy;

public enum CustomerCategory { Standard, Member, Partner }

/// <summary>
/// A deliberately bounded, single-currency quote input. The catalog teaches collaboration, not a
/// complete money model: real systems must make currency, tax, and monetary precision explicit.
/// Immutable input can safely be shared by multiple independent policy evaluations.
/// </summary>
public sealed record PricingRequest
{
    public PricingRequest(decimal subtotal, CustomerCategory customer)
    {
        if (subtotal is < 0 or > 1_000_000 || decimal.Round(subtotal, 2) != subtotal)
            throw new ArgumentOutOfRangeException(nameof(subtotal), "Subtotal must be 0..1000000 with at most two decimal places.");
        if (!Enum.IsDefined(customer))
            throw new ArgumentOutOfRangeException(nameof(customer));
        Subtotal = subtotal;
        Customer = customer;
    }

    public decimal Subtotal { get; }
    public CustomerCategory Customer { get; }
}
