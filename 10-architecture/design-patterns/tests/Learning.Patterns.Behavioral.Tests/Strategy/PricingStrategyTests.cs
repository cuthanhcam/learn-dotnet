using Learning.Patterns.Behavioral.Strategy;
using Learning.Patterns.Behavioral.Strategy.Baseline;
using Learning.Patterns.Behavioral.Strategy.ModernCSharp;
using Learning.Patterns.Behavioral.Strategy.Refactored;

namespace Learning.Patterns.Behavioral.Tests.Strategy;

public sealed class PricingStrategyTests
{
    [Theory]
    [InlineData(CustomerCategory.Standard, "500", "0")]
    [InlineData(CustomerCategory.Member, "100", "10")]
    [InlineData(CustomerCategory.Member, "500", "50")]
    [InlineData(CustomerCategory.Member, "1000", "50")]
    [InlineData(CustomerCategory.Partner, "499", "24.95")]
    [InlineData(CustomerCategory.Partner, "500", "75")]
    [InlineData(CustomerCategory.Partner, "1000", "150")]
    [InlineData(CustomerCategory.Member, "0.05", "0.01")]
    [InlineData(CustomerCategory.Partner, "0", "0")]
    public void Refactor_PreservesBaselineAndExpectedBusinessOutcome(CustomerCategory category,
        string subtotalText, string expectedText)
    {
        decimal subtotal = decimal.Parse(subtotalText, System.Globalization.CultureInfo.InvariantCulture);
        decimal expected = decimal.Parse(expectedText, System.Globalization.CultureInfo.InvariantCulture);
        var request = new PricingRequest(subtotal, category);
        IDiscountPolicy policy = DiscountPolicySelector.Select(category);
        PricingQuote result = new PricingService(policy).Quote(request);
        Assert.Equal(expected, result.Discount);
        Assert.Equal(subtotal - expected, result.Total);
        Assert.Equal(new SwitchPricingService().Quote(request), result);
        Assert.Equal(new DelegatePricingService(policy.CalculateDiscount).Quote(request), result);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000001)]
    public void InvalidSubtotal_IsRejected(int amount) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new PricingRequest(amount, CustomerCategory.Standard));

    [Fact]
    public void FractionBeyondMinorUnit_IsRejected() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new PricingRequest(1.001m, CustomerCategory.Standard));

    [Fact]
    public void UnknownCategory_IsRejected() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new PricingRequest(10m, (CustomerCategory)999));

    [Fact]
    public void Selector_DoesNotSilentlyFallBackForUnknownCategory() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DiscountPolicySelector.Select((CustomerCategory)999));

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void BrokenPolicy_IsRejectedRatherThanClamped(int discount)
    {
        var request = new PricingRequest(100m, CustomerCategory.Standard);
        Assert.Throws<InvalidOperationException>(() => new PricingService(new FixedDiscountPolicy(discount)).Quote(request));
        Assert.Throws<InvalidOperationException>(() => new DelegatePricingService(_ => discount).Quote(request));
    }

    [Fact]
    public void ConsumerDefinedPolicy_ExtendsBehaviorWithoutChangingContext()
    {
        var service = new PricingService(new FixedDiscountPolicy(7m));
        Assert.Equal(93m, service.Quote(new PricingRequest(100m, CustomerCategory.Standard)).Total);
    }

    [Fact]
    public void ZeroTotal_IsValidWhenDiscountEqualsSubtotal() => Assert.Equal(0m,
        new PricingService(new FixedDiscountPolicy(10m)).Quote(new PricingRequest(10m, CustomerCategory.Standard)).Total);

    [Fact]
    public void MissingCollaborator_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new PricingService(null!));
        Assert.Throws<ArgumentNullException>(() => new DelegatePricingService(null!));
    }

    [Fact]
    public void MissingInput_IsRejectedAtBothContextBoundaries()
    {
        Assert.Throws<ArgumentNullException>(() => new PricingService(new StandardDiscountPolicy()).Quote(null!));
        Assert.Throws<ArgumentNullException>(() => new DelegatePricingService(_ => 0).Quote(null!));
        Assert.Throws<ArgumentNullException>(() => new SwitchPricingService().Quote(null!));
    }

    [Fact]
    public void Demo_ProducesDeterministicReadableOutput()
    {
        using var output = new StringWriter();
        StrategyDemo.Run(output);
        Assert.Equal(new[]
        {
            "Standard: baseline=500.00; strategy=500.00; discount=0.00",
            "Member: baseline=450.00; strategy=450.00; discount=50.00",
            "Partner: baseline=425.00; strategy=425.00; discount=75.00",
            "Delegate campaign total: 75.00"
        }, output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }

    private sealed class FixedDiscountPolicy(decimal discount) : IDiscountPolicy
    {
        public decimal CalculateDiscount(PricingRequest request) => discount;
    }
}
