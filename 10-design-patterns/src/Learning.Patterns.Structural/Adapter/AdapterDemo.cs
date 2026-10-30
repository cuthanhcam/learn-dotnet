using System.Globalization;
using Learning.Patterns.Structural.Adapter.Baseline;
using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Adapter.Legacy;
using Learning.Patterns.Structural.Adapter.Refactored;

namespace Learning.Patterns.Structural.Adapter;

public static class AdapterDemo
{
    public static async Task RunAsync(TextWriter output, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);
        var legacy = new SimulatedLegacyRateClient();
        IShippingRates rates = new LegacyShippingRateAdapter(legacy);
        var request = new ShippingRequest(1.25m, "us");
        ShippingQuote baseline = await new LegacyCoupledCheckout(legacy).QuoteAsync(request, cancellationToken);
        ShippingQuote adapted = await rates.QuoteAsync(request, cancellationToken);
        output.WriteLine($"Baseline USD: {baseline.AmountUsd!.Value.ToString("0.00", CultureInfo.InvariantCulture)}");
        output.WriteLine($"Adapter USD: {adapted.AmountUsd!.Value.ToString("0.00", CultureInfo.InvariantCulture)}");
        output.WriteLine($"GB route: {(await rates.QuoteAsync(new ShippingRequest(1m, "GB"), cancellationToken)).Status}");
        output.WriteLine($"DE provider: {(await rates.QuoteAsync(new ShippingRequest(1m, "DE"), cancellationToken)).Status}");
    }
}
