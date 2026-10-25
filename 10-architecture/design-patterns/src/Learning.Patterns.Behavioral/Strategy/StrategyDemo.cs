using System.Globalization;
using Learning.Patterns.Behavioral.Strategy.Baseline;
using Learning.Patterns.Behavioral.Strategy.ModernCSharp;
using Learning.Patterns.Behavioral.Strategy.Refactored;

namespace Learning.Patterns.Behavioral.Strategy;

public static class StrategyDemo
{
    public static void Run(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        foreach (CustomerCategory category in Enum.GetValues<CustomerCategory>())
        {
            var request = new PricingRequest(500m, category);
            PricingQuote baseline = new SwitchPricingService().Quote(request);
            PricingQuote strategy = new PricingService(DiscountPolicySelector.Select(category)).Quote(request);
            output.WriteLine(FormattableString.Invariant(
                $"{category}: baseline={baseline.Total:0.00}; strategy={strategy.Total:0.00}; discount={strategy.Discount:0.00}"));
        }
        // A local campaign algorithm extends the context without editing its implementation or the
        // category selector. Its authority/eligibility would be decided by the calling application.
        var campaign = new DelegatePricingService(request => Math.Min(25m, request.Subtotal));
        output.WriteLine($"Delegate campaign total: {campaign.Quote(new PricingRequest(100m, CustomerCategory.Standard)).Total.ToString("0.00", CultureInfo.InvariantCulture)}");
    }
}
