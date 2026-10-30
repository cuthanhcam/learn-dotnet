using Learning.Patterns.Structural.Adapter;
using Learning.Patterns.Structural.Adapter.Baseline;
using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Adapter.Legacy;
using Learning.Patterns.Structural.Adapter.Refactored;

namespace Learning.Patterns.Structural.Tests.Adapter;

public sealed class ShippingAdapterTests
{
    [Fact]
    public async Task MissingDependenciesAndInput_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new LegacyShippingRateAdapter(null!));
        Assert.Throws<ArgumentNullException>(() => new LegacyCoupledCheckout(null!));
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await new LegacyShippingRateAdapter(new RecordingClient()).QuoteAsync(null!));
    }

    [Theory]
    [InlineData("US", ShippingQuoteStatus.Available)]
    [InlineData("GB", ShippingQuoteStatus.NoRoute)]
    [InlineData("DE", ShippingQuoteStatus.TemporarilyUnavailable)]
    public async Task Refactor_PreservesBaselineAndExpectedOutcome(string country, ShippingQuoteStatus expected)
    {
        var legacy = new SimulatedLegacyRateClient();
        var request = new ShippingRequest(1.25m, country);
        ShippingQuote quote = await new LegacyShippingRateAdapter(legacy).QuoteAsync(request);
        Assert.Equal(expected, quote.Status);
        Assert.Equal(await new LegacyCoupledCheckout(legacy).QuoteAsync(request), quote);
        if (expected == ShippingQuoteStatus.Available)
            Assert.Equal(12.99m, quote.AmountUsd);
        else
            Assert.Null(quote.AmountUsd);
    }

    [Theory]
    [InlineData("0.001", 1)]
    [InlineData("1.250", 1250)]
    [InlineData("1000", 1000000)]
    public async Task UnitsAndCountry_AreTranslatedExactly(string kilograms, int grams)
    {
        var legacy = new RecordingClient();
        await new LegacyShippingRateAdapter(legacy).QuoteAsync(new ShippingRequest(
            decimal.Parse(kilograms, System.Globalization.CultureInfo.InvariantCulture), " us "));
        Assert.Equal(new LegacyParcel(grams, "US"), legacy.LastParcel);
        Assert.Equal(1, legacy.Calls);
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("usd")]
    [InlineData("")]
    [InlineData(null)]
    public async Task UnknownCurrency_IsNotSilentlyTreatedAsUsd(string? currency) =>
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await new LegacyShippingRateAdapter(new RecordingClient(new LegacyRateReply("OK", 1299, currency)))
                .QuoteAsync(new ShippingRequest(1m, "US")));

    [Theory]
    [InlineData(-1L)]
    [InlineData(100000001L)]
    [InlineData(null)]
    public async Task InvalidProviderPrice_IsAProtocolFailure(long? cents) =>
        await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await new LegacyShippingRateAdapter(new RecordingClient(new LegacyRateReply("OK", cents, "USD")))
                .QuoteAsync(new ShippingRequest(1m, "US")));

    [Fact]
    public async Task ZeroPrice_IsValidFreeShipping() => Assert.Equal(0m,
        (await new LegacyShippingRateAdapter(new RecordingClient(new LegacyRateReply("OK", 0, "USD")))
            .QuoteAsync(new ShippingRequest(1m, "US"))).AmountUsd);

    [Fact]
    public async Task UnknownStatus_IsNotRelabeledAsNoRoute() => await Assert.ThrowsAsync<InvalidDataException>(async () =>
        await new LegacyShippingRateAdapter(new RecordingClient(new LegacyRateReply("NEW_CODE", null, null)))
            .QuoteAsync(new ShippingRequest(1m, "US")));

    [Fact]
    public async Task MissingReply_IsNotAnAvailableZeroPrice() => await Assert.ThrowsAsync<InvalidDataException>(async () =>
        await new LegacyShippingRateAdapter(new FailingClient((_, _) => Task.FromResult<LegacyRateReply>(null!)))
            .QuoteAsync(new ShippingRequest(1m, "US")));

    [Fact]
    public async Task KnownTransientFailure_MapsWithoutHiddenRetry()
    {
        int calls = 0;
        var legacy = new FailingClient((_, _) =>
        {
            calls++;
            throw new LegacyServiceUnavailableException();
        });
        ShippingQuote quote = await new LegacyShippingRateAdapter(legacy).QuoteAsync(new ShippingRequest(1m, "US"));
        Assert.Equal(ShippingQuoteStatus.TemporarilyUnavailable, quote.Status);
        Assert.Equal(1, calls);
    }

    [Fact]
    public async Task UnexpectedFailure_PropagatesInsteadOfPretendingToBeTemporary() =>
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await new LegacyShippingRateAdapter(new FailingClient((_, _) => throw new InvalidOperationException("Test fault")))
                .QuoteAsync(new ShippingRequest(1m, "US")));

    [Fact]
    public async Task PreCanceledRequest_DoesNotCallTheProvider()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var legacy = new RecordingClient();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await new LegacyShippingRateAdapter(legacy).QuoteAsync(new ShippingRequest(1m, "US"), cancellation.Token));
        Assert.Equal(0, legacy.Calls);
    }

    [Fact]
    public async Task CancellationDuringProviderCall_PropagatesWithTheOriginalToken()
    {
        using var cancellation = new CancellationTokenSource();
        var legacy = new FailingClient((_, token) =>
        {
            Assert.Equal(cancellation.Token, token);
            cancellation.Cancel();
            return Task.FromCanceled<LegacyRateReply>(token);
        });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await new LegacyShippingRateAdapter(legacy).QuoteAsync(new ShippingRequest(1m, "US"), cancellation.Token));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1001)]
    public void InvalidWeight_IsRejected(int kilograms) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new ShippingRequest(kilograms, "US"));

    [Fact]
    public void SubGramPrecision_IsNotSilentlyRounded() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new ShippingRequest(1.0001m, "US"));

    [Theory]
    [InlineData("USA")]
    [InlineData("1A")]
    [InlineData("éa")]
    public void MalformedCountry_IsRejected(string country) => Assert.Throws<ArgumentException>(() =>
        new ShippingRequest(1m, country));

    [Fact]
    public async Task Adapter_DoesNotDisposeCallerOwnedClient()
    {
        using var legacy = new RecordingClient();
        await new LegacyShippingRateAdapter(legacy).QuoteAsync(new ShippingRequest(1m, "US"));
        Assert.False(legacy.Disposed);
    }

    [Fact]
    public async Task Demo_IsDeterministic()
    {
        using var output = new StringWriter();
        await AdapterDemo.RunAsync(output);
        Assert.Equal(new[]
        {
            "Baseline USD: 12.99", "Adapter USD: 12.99", "GB route: NoRoute", "DE provider: TemporarilyUnavailable"
        }, output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }

    private sealed class RecordingClient(LegacyRateReply? reply = null) : ILegacyRateClient, IDisposable
    {
        public int Calls { get; private set; }
        public LegacyParcel? LastParcel { get; private set; }
        public bool Disposed { get; private set; }
        public Task<LegacyRateReply> FetchRateAsync(LegacyParcel parcel, CancellationToken cancellationToken)
        {
            Calls++;
            LastParcel = parcel;
            return Task.FromResult(reply ?? new LegacyRateReply("OK", 1299, "USD"));
        }
        public void Dispose() => Disposed = true;
    }

    private sealed class FailingClient(Func<LegacyParcel, CancellationToken, Task<LegacyRateReply>> call) : ILegacyRateClient
    {
        public Task<LegacyRateReply> FetchRateAsync(LegacyParcel parcel, CancellationToken cancellationToken) => call(parcel, cancellationToken);
    }
}
