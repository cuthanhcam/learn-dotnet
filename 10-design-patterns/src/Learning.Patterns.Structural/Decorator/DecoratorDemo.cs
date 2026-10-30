using System.Globalization;
using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Adapter.Legacy;
using Learning.Patterns.Structural.Adapter.Refactored;
using Learning.Patterns.Structural.Decorator.Baseline;
using Learning.Patterns.Structural.Decorator.Contracts;
using Learning.Patterns.Structural.Decorator.Refactored;

namespace Learning.Patterns.Structural.Decorator;

public static class DecoratorDemo
{
    public static async Task RunAsync(TextWriter output, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);
        IShippingRates component = new LegacyShippingRateAdapter(new SimulatedLegacyRateClient());
        var baseline = new RetryAndAuditQuoteWorkflow(component, new SimulatedDelay(), new RecordingAudit());
        var audit = new RecordingAudit();
        var delay = new SimulatedDelay();
        IShippingRates decorated = new AuditedShippingRates(new RetryingShippingRates(component, delay), audit);
        var request = new ShippingRequest(1.25m, "US");
        ShippingQuote original = await baseline.QuoteAsync(request, cancellationToken);
        ShippingQuote result = await decorated.QuoteAsync(request, cancellationToken);
        output.WriteLine($"Baseline USD: {original.AmountUsd!.Value.ToString("0.00", CultureInfo.InvariantCulture)}");
        output.WriteLine($"Decorated USD: {result.AmountUsd!.Value.ToString("0.00", CultureInfo.InvariantCulture)}");
        output.WriteLine($"DE bounded result: {(await decorated.QuoteAsync(new ShippingRequest(1m, "DE"), cancellationToken)).Status}");
        output.WriteLine($"Audit records: {audit.Count}");
        output.WriteLine($"Retry waits: {delay.Count}");
    }

    private sealed class RecordingAudit : IQuoteAudit
    {
        public int Count { get; private set; }
        public ValueTask RecordAsync(ShippingRequest request, ShippingQuote quote, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Count++;
            return ValueTask.CompletedTask;
        }
    }

    // Offline demonstration only. A real provider needs a deliberate delay/backoff policy;
    // FixedQuoteRetryDelay supplies a cancellable fixed delay without changing either decorator.
    private sealed class SimulatedDelay : IQuoteRetryDelay
    {
        public int Count { get; private set; }
        public ValueTask WaitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Count++;
            return ValueTask.CompletedTask;
        }
    }
}
