using Learning.Patterns.Structural.Adapter.Contracts;
using Learning.Patterns.Structural.Decorator;
using Learning.Patterns.Structural.Decorator.Baseline;
using Learning.Patterns.Structural.Decorator.Contracts;
using Learning.Patterns.Structural.Decorator.Refactored;

namespace Learning.Patterns.Structural.Tests.Decorator;

public sealed class ShippingDecoratorTests
{
    private static readonly ShippingRequest Request = new(1m, "US");

    [Theory]
    [InlineData(ShippingQuoteStatus.Available)]
    [InlineData(ShippingQuoteStatus.NoRoute)]
    [InlineData(ShippingQuoteStatus.TemporarilyUnavailable)]
    public async Task FinalAuditComposition_PreservesCorrectBaseline(ShippingQuoteStatus terminal)
    {
        ShippingQuote[] sequence = [ShippingQuote.TemporarilyUnavailable(), Quote(terminal), Quote(terminal)];
        var baselineAudit = new Audit();
        var decoratedAudit = new Audit();
        var baselineDelay = new Delay();
        var decoratedDelay = new Delay();
        ShippingQuote original = await new RetryAndAuditQuoteWorkflow(new Rates(sequence), baselineDelay, baselineAudit)
            .QuoteAsync(Request);
        ShippingQuote decorated = await new AuditedShippingRates(
            new RetryingShippingRates(new Rates(sequence), decoratedDelay), decoratedAudit).QuoteAsync(Request);

        Assert.Equal(original, decorated);
        Assert.Equal(baselineAudit.Records, decoratedAudit.Records);
        Assert.Equal(baselineDelay.Calls, decoratedDelay.Calls);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WrapperOrder_SelectsFinalResultOrPerAttemptAudit(bool finalOnly)
    {
        var rates = new Rates(ShippingQuote.TemporarilyUnavailable(), ShippingQuote.Available(12.99m));
        var audit = new Audit();
        var delay = new Delay();
        IShippingRates composed = finalOnly
            ? new AuditedShippingRates(new RetryingShippingRates(rates, delay), audit)
            : new RetryingShippingRates(new AuditedShippingRates(rates, audit), delay);

        Assert.Equal(ShippingQuote.Available(12.99m), await composed.QuoteAsync(Request));
        Assert.Equal(finalOnly
            ? new[] { ShippingQuoteStatus.Available }
            : [ShippingQuoteStatus.TemporarilyUnavailable, ShippingQuoteStatus.Available],
            audit.Records.Select(record => record.Quote.Status));
        Assert.All(audit.Records, record => Assert.Equal(Request, record.Request));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task UnavailableProvider_RespectsTotalAttemptBudget(int maxAttempts)
    {
        var rates = new Rates(Enumerable.Repeat(ShippingQuote.TemporarilyUnavailable(), maxAttempts).ToArray());
        var delay = new Delay();
        ShippingQuote result = await new RetryingShippingRates(rates, delay, maxAttempts).QuoteAsync(Request);
        Assert.Equal(ShippingQuote.TemporarilyUnavailable(), result);
        Assert.Equal(maxAttempts, rates.Tokens.Count);
        Assert.Equal(maxAttempts - 1, delay.Calls);
    }

    [Theory]
    [InlineData(ShippingQuoteStatus.Available)]
    [InlineData(ShippingQuoteStatus.NoRoute)]
    public async Task TerminalOutcome_DoesNotRequestAnotherAttempt(ShippingQuoteStatus status)
    {
        var rates = new Rates(Quote(status));
        var delay = new Delay();
        Assert.Equal(Quote(status), await new RetryingShippingRates(rates, delay).QuoteAsync(Request));
        Assert.Single(rates.Tokens);
        Assert.Equal(0, delay.Calls);
    }

    [Fact]
    public async Task ProviderException_PropagatesWithoutRetryOrCompletionRecord()
    {
        var failure = new InvalidOperationException("provider failure");
        var rates = new CallbackRates(_ => ValueTask.FromException<ShippingQuote>(failure));
        var delay = new Delay();
        var audit = new Audit();
        IShippingRates composed = new AuditedShippingRates(new RetryingShippingRates(rates, delay), audit);
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => composed.QuoteAsync(Request).AsTask()));
        Assert.Equal(1, rates.Calls);
        Assert.Equal(0, delay.Calls);
        Assert.Empty(audit.Records);
    }

    [Fact]
    public async Task RequiredAudit_IsAwaitedBeforeQuoteCompletes()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var audit = new Audit(async _ =>
        {
            started.SetResult();
            await release.Task;
        });
        Task<ShippingQuote> operation = new AuditedShippingRates(new Rates(ShippingQuote.Available(1m)), audit)
            .QuoteAsync(Request).AsTask();
        try
        {
            await started.Task.WaitAsync(TimeSpan.FromSeconds(5));
            Assert.False(operation.IsCompleted);
        }
        finally
        {
            release.TrySetResult();
        }
        Assert.Equal(ShippingQuote.Available(1m), await operation);
    }

    [Fact]
    public async Task AuditFailureInsideRetry_IsNotMistakenForProviderUnavailability()
    {
        var failure = new InvalidOperationException("required audit failed");
        var rates = new Rates(ShippingQuote.TemporarilyUnavailable());
        var delay = new Delay();
        var audit = new Audit(_ => ValueTask.FromException(failure));
        var composed = new RetryingShippingRates(new AuditedShippingRates(rates, audit), delay);
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => composed.QuoteAsync(Request).AsTask()));
        Assert.Single(rates.Tokens);
        Assert.Equal(0, delay.Calls);
    }

    [Fact]
    public async Task DelayFailure_DoesNotStartAnotherAttempt()
    {
        var failure = new InvalidOperationException("delay failed");
        var rates = new Rates(ShippingQuote.TemporarilyUnavailable());
        var composed = new RetryingShippingRates(rates, new Delay(_ => throw failure));
        Assert.Same(failure, await Assert.ThrowsAsync<InvalidOperationException>(() => composed.QuoteAsync(Request).AsTask()));
        Assert.Single(rates.Tokens);
    }

    [Fact]
    public async Task Cancellation_PropagatesSameTokenAcrossRetriesAndAudit()
    {
        using var cancellation = new CancellationTokenSource();
        var rates = new Rates(ShippingQuote.TemporarilyUnavailable(), ShippingQuote.Available(2m));
        var audit = new Audit(token =>
        {
            Assert.Equal(cancellation.Token, token);
            return ValueTask.CompletedTask;
        });
        var delay = new Delay(token => Assert.Equal(cancellation.Token, token));
        await new AuditedShippingRates(new RetryingShippingRates(rates, delay), audit)
            .QuoteAsync(Request, cancellation.Token);
        Assert.Equal(2, rates.Tokens.Count);
        Assert.All(rates.Tokens, token => Assert.Equal(cancellation.Token, token));
        Assert.Single(audit.Records);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task PreCanceledCall_DoesNotReachInnerComponent(bool auditWrapper)
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var rates = new Rates();
        IShippingRates composed = auditWrapper
            ? new AuditedShippingRates(rates, new Audit()) : new RetryingShippingRates(rates, new Delay());
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => composed.QuoteAsync(Request, cancellation.Token).AsTask());
        Assert.Empty(rates.Tokens);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task InnerIgnoresCancellation_NoAuditOrRetryStartsAfterward(bool auditWrapper)
    {
        using var cancellation = new CancellationTokenSource();
        var rates = new CallbackRates(_ =>
        {
            cancellation.Cancel();
            return ValueTask.FromResult(ShippingQuote.TemporarilyUnavailable());
        });
        var audit = new Audit();
        var delay = new Delay();
        IShippingRates composed = auditWrapper
            ? new AuditedShippingRates(rates, audit) : new RetryingShippingRates(rates, delay);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => composed.QuoteAsync(Request, cancellation.Token).AsTask());
        Assert.Equal(1, rates.Calls);
        Assert.Empty(audit.Records);
        Assert.Equal(0, delay.Calls);
    }

    [Fact]
    public async Task DelayIgnoresCancellation_NoSecondProviderAttemptStarts()
    {
        using var cancellation = new CancellationTokenSource();
        var rates = new Rates(ShippingQuote.TemporarilyUnavailable());
        var composed = new RetryingShippingRates(rates, new Delay(_ => cancellation.Cancel()));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => composed.QuoteAsync(Request, cancellation.Token).AsTask());
        Assert.Single(rates.Tokens);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(4)]
    [InlineData(int.MaxValue)]
    public void InvalidAttemptBudget_IsRejected(int attempts) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new RetryingShippingRates(new Rates(), new Delay(), attempts));

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(31000)]
    public void InvalidFixedDelay_IsRejected(int milliseconds) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new FixedQuoteRetryDelay(TimeSpan.FromMilliseconds(milliseconds)));

    [Fact]
    public async Task FixedDelay_ObservesCancellationWithoutSleeping()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new FixedQuoteRetryDelay(TimeSpan.FromSeconds(1)).WaitAsync(cancellation.Token).AsTask());
    }

    [Fact]
    public async Task Demo_HasDeterministicOfflineOutput()
    {
        using var output = new StringWriter();
        await DecoratorDemo.RunAsync(output);
        Assert.Equal(new[] { "Baseline USD: 12.99", "Decorated USD: 12.99",
            "DE bounded result: TemporarilyUnavailable", "Audit records: 2", "Retry waits: 2" },
            output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }

    private static ShippingQuote Quote(ShippingQuoteStatus status) => status switch
    {
        ShippingQuoteStatus.Available => ShippingQuote.Available(12.99m),
        ShippingQuoteStatus.NoRoute => ShippingQuote.NoRoute(),
        _ => ShippingQuote.TemporarilyUnavailable()
    };

    private sealed class Rates(params ShippingQuote[] sequence) : IShippingRates
    {
        private readonly Queue<ShippingQuote> _quotes = new(sequence);
        public List<CancellationToken> Tokens { get; } = [];
        public ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
        {
            Tokens.Add(cancellationToken);
            return ValueTask.FromResult(_quotes.Dequeue());
        }
    }

    private sealed class CallbackRates(Func<CancellationToken, ValueTask<ShippingQuote>> callback) : IShippingRates
    {
        public int Calls { get; private set; }
        public ValueTask<ShippingQuote> QuoteAsync(ShippingRequest request, CancellationToken cancellationToken = default)
        {
            Calls++;
            return callback(cancellationToken);
        }
    }

    private sealed class Audit(Func<CancellationToken, ValueTask>? record = null) : IQuoteAudit
    {
        public List<(ShippingRequest Request, ShippingQuote Quote)> Records { get; } = [];
        public ValueTask RecordAsync(ShippingRequest request, ShippingQuote quote, CancellationToken cancellationToken)
        {
            Records.Add((request, quote));
            return record?.Invoke(cancellationToken) ?? ValueTask.CompletedTask;
        }
    }

    private sealed class Delay(Action<CancellationToken>? wait = null) : IQuoteRetryDelay
    {
        public int Calls { get; private set; }
        public ValueTask WaitAsync(CancellationToken cancellationToken)
        {
            Calls++;
            wait?.Invoke(cancellationToken);
            return ValueTask.CompletedTask;
        }
    }
}
