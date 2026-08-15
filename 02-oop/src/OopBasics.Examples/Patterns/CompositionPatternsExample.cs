namespace OopBasics.Examples.Patterns;

public static class CompositionPatternsExample
{
    public static void Run()
    {
        var order = new Shipment(weightKilograms: 3m, destination: Destination.Domestic);
        var checkout = new ShippingService(new StandardShippingPolicy());

        Console.WriteLine("Composition and Strategy");
        Console.WriteLine($"Standard shipping: {checkout.Calculate(order):C}");

        checkout.ChangePolicy(new ExpressShippingPolicy());
        Console.WriteLine($"Express shipping: {checkout.Calculate(order):C}");
    }
}

public enum Destination
{
    Domestic,
    International
}

public sealed record Shipment
{
    public Shipment(decimal weightKilograms, Destination destination)
    {
        if (weightKilograms <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(weightKilograms),
                "Shipment weight must be positive.");
        }

        WeightKilograms = weightKilograms;
        Destination = destination;
    }

    public decimal WeightKilograms { get; }
    public Destination Destination { get; }
}

public interface IShippingPolicy
{
    decimal Calculate(Shipment shipment);
}

public sealed class StandardShippingPolicy : IShippingPolicy
{
    public decimal Calculate(Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        decimal baseRate = shipment.Destination == Destination.Domestic ? 5m : 15m;
        return baseRate + (shipment.WeightKilograms * 1.25m);
    }
}

public sealed class ExpressShippingPolicy : IShippingPolicy
{
    public decimal Calculate(Shipment shipment)
    {
        ArgumentNullException.ThrowIfNull(shipment);

        decimal baseRate = shipment.Destination == Destination.Domestic ? 12m : 30m;
        return baseRate + (shipment.WeightKilograms * 2.50m);
    }
}

public sealed class ShippingService
{
    private IShippingPolicy _policy;

    public ShippingService(IShippingPolicy policy)
    {
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }

    public decimal Calculate(Shipment shipment) => _policy.Calculate(shipment);

    public void ChangePolicy(IShippingPolicy policy)
    {
        // Composition allows behavior to change without changing Shipment or
        // creating an inheritance hierarchy of shipment subtypes.
        _policy = policy ?? throw new ArgumentNullException(nameof(policy));
    }
}
