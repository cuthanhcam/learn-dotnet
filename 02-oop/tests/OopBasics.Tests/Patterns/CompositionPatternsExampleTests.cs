using OopBasics.Examples.Patterns;

namespace OopBasics.Tests.Patterns;

public sealed class CompositionPatternsExampleTests
{
    [Fact]
    public void ShippingService_DelegatesToSelectedPolicy()
    {
        var shipment = new Shipment(2m, Destination.Domestic);
        var service = new ShippingService(new StandardShippingPolicy());

        Assert.Equal(7.50m, service.Calculate(shipment));

        service.ChangePolicy(new ExpressShippingPolicy());
        Assert.Equal(17m, service.Calculate(shipment));
    }

    [Fact]
    public void Shipment_RejectsStateThatViolatesItsInvariant()
    {
        ArgumentOutOfRangeException error = Assert.Throws<ArgumentOutOfRangeException>(
            () => new Shipment(0m, Destination.International));

        Assert.Equal("weightKilograms", error.ParamName);
    }

    [Fact]
    public void ShippingService_AcceptsConsumerDefinedPolicy()
    {
        var service = new ShippingService(new FreeShippingPolicy());

        decimal price = service.Calculate(new Shipment(100m, Destination.International));

        Assert.Equal(0m, price);
    }

    private sealed class FreeShippingPolicy : IShippingPolicy
    {
        public decimal Calculate(Shipment shipment)
        {
            ArgumentNullException.ThrowIfNull(shipment);
            return 0m;
        }
    }
}
