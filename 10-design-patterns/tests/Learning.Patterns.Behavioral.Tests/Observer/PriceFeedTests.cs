using Learning.Patterns.Behavioral.Observer;
using Learning.Patterns.Behavioral.Observer.Baseline;
using Learning.Patterns.Behavioral.Observer.ModernCSharp;
using Learning.Patterns.Behavioral.Observer.Observers;
using Learning.Patterns.Behavioral.Observer.Refactored;

namespace Learning.Patterns.Behavioral.Tests.Observer;

public sealed class PriceFeedTests
{
    private static readonly PriceUpdate First = new("BOOK-1", 12m);
    private static readonly PriceUpdate Second = new("BOOK-1", 20m);

    [Fact]
    public void TwoPermanentConsumers_PreserveCorrectBaseline()
    {
        var oldDisplay = new PriceDisplay();
        var oldThreshold = new PriceThresholdObserver(15m);
        var baseline = new FixedPricePublisher(oldDisplay, oldThreshold);
        var display = new PriceDisplay();
        var threshold = new PriceThresholdObserver(15m);
        var feed = new PriceFeed();
        using IDisposable a = feed.Subscribe(display);
        using IDisposable b = feed.Subscribe(threshold);
        foreach (PriceUpdate update in new[] { First, Second })
        {
            baseline.Publish(update);
            feed.Publish(update);
        }
        Assert.Equal(oldDisplay.Latest, display.Latest);
        Assert.Equal(oldThreshold.Matches, threshold.Matches);
        Assert.Equal(1, threshold.Matches);
    }

    [Fact]
    public void DisposingOneToken_DoesNotRemoveOtherObservers()
    {
        var feed = new PriceFeed();
        var first = new Probe();
        var second = new Probe();
        IDisposable token = feed.Subscribe(first);
        using IDisposable other = feed.Subscribe(second);
        feed.Publish(First);
        token.Dispose();
        token.Dispose();
        feed.Publish(Second);
        Assert.Equal(new[] { First }, first.Values);
        Assert.Equal(new[] { First, Second }, second.Values);
    }

    [Fact]
    public void DuplicateActiveObserver_IsRejectedButCanResubscribeAfterDisposal()
    {
        var feed = new PriceFeed();
        var observer = new Probe();
        IDisposable first = feed.Subscribe(observer);
        Assert.Throws<InvalidOperationException>(() => feed.Subscribe(observer));
        first.Dispose();
        using IDisposable replacement = feed.Subscribe(observer);
        feed.Publish(First);
        Assert.Equal(new[] { First }, observer.Values);
    }

    [Fact]
    public void SubscriptionBudget_IsReleasedByDisposal()
    {
        var feed = new PriceFeed(1);
        IDisposable token = feed.Subscribe(new Probe());
        Assert.Throws<InvalidOperationException>(() => feed.Subscribe(new Probe()));
        token.Dispose();
        var replacement = new Probe();
        using IDisposable next = feed.Subscribe(replacement);
        feed.Publish(First);
        Assert.Equal(First, Assert.Single(replacement.Values));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65)]
    [InlineData(int.MaxValue)]
    public void InvalidSubscriptionBudget_IsRejected(int capacity) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new PriceFeed(capacity));

    [Fact]
    public void SelfUnsubscribe_DoesNotCorruptIteration()
    {
        var feed = new PriceFeed();
        IDisposable? self = null;
        var once = new Probe(next: _ => self!.Dispose());
        var always = new Probe();
        self = feed.Subscribe(once);
        using IDisposable other = feed.Subscribe(always);
        feed.Publish(First);
        feed.Publish(Second);
        Assert.Single(once.Values);
        Assert.Equal(2, always.Values.Count);
    }

    [Fact]
    public void DisposedBeforeItsTurn_IsSkippedInTheCurrentSnapshot()
    {
        var feed = new PriceFeed();
        IDisposable? laterToken = null;
        using IDisposable first = feed.Subscribe(new Probe(next: _ => laterToken!.Dispose()));
        var later = new Probe();
        laterToken = feed.Subscribe(later);
        feed.Publish(First);
        Assert.Empty(later.Values);
    }

    [Fact]
    public void SubscriptionAddedByCallback_StartsWithTheNextPublication()
    {
        var feed = new PriceFeed();
        var late = new Probe();
        IDisposable? lateToken = null;
        using IDisposable initial = feed.Subscribe(new Probe(next: _ => lateToken ??= feed.Subscribe(late)));
        try
        {
            feed.Publish(First);
            Assert.Empty(late.Values);
            feed.Publish(Second);
            Assert.Equal(Second, Assert.Single(late.Values));
        }
        finally { lateToken?.Dispose(); }
    }

    [Fact]
    public void ObserverFailure_AttemptsHealthyObserversAndDoesNotBecomeStreamError()
    {
        var feed = new PriceFeed();
        var failure = new InvalidOperationException("observer failed");
        bool fail = true;
        var broken = new Probe(next: _ => { if (fail) throw failure; });
        var healthy = new Probe();
        using IDisposable a = feed.Subscribe(broken);
        using IDisposable b = feed.Subscribe(healthy);
        AggregateException reported = Assert.Throws<AggregateException>(() => feed.Publish(First));
        Assert.Same(failure, Assert.Single(reported.InnerExceptions));
        Assert.Equal(First, Assert.Single(healthy.Values));
        Assert.Empty(broken.Errors);
        Assert.Empty(healthy.Errors);
        fail = false;
        feed.Publish(Second);
        Assert.Equal(2, healthy.Values.Count);
    }

    [Fact]
    public void MultipleObserverFailures_AreReportedTogetherAfterTheBatch()
    {
        var feed = new PriceFeed();
        var first = new InvalidOperationException("first observer");
        var second = new ArgumentException("second observer");
        using IDisposable a = feed.Subscribe(new Probe(next: _ => throw first));
        using IDisposable b = feed.Subscribe(new Probe(next: _ => throw second));
        AggregateException reported = Assert.Throws<AggregateException>(() => feed.Publish(First));
        Assert.Equal(2, reported.InnerExceptions.Count);
        Assert.Contains(first, reported.InnerExceptions);
        Assert.Contains(second, reported.InnerExceptions);
    }

    [Fact]
    public void Completion_IsTerminalAndRepeatedTerminalCallsKeepTheOriginalResult()
    {
        var feed = new PriceFeed();
        var observer = new Probe();
        using IDisposable token = feed.Subscribe(observer);
        feed.Publish(First);
        feed.Complete();
        feed.Complete();
        feed.Fail(new InvalidOperationException("too late"));
        Assert.Equal(1, observer.Completions);
        Assert.Empty(observer.Errors);
        Assert.Throws<InvalidOperationException>(() => feed.Publish(Second));
        Assert.Equal(new[] { First }, observer.Values);
        var late = new Probe();
        using IDisposable ignored = feed.Subscribe(late);
        Assert.Equal(1, late.Completions);
        Assert.Empty(late.Values);
    }

    [Fact]
    public void StreamFailure_NotifiesCurrentAndLateObserversWithoutPriceReplay()
    {
        var feed = new PriceFeed();
        var observer = new Probe();
        using IDisposable token = feed.Subscribe(observer);
        var failure = new InvalidOperationException("provider cannot continue");
        feed.Publish(First);
        feed.Fail(failure);
        feed.Fail(new InvalidOperationException("ignored"));
        feed.Complete();
        Assert.Same(failure, Assert.Single(observer.Errors));
        Assert.Equal(0, observer.Completions);
        Assert.Throws<InvalidOperationException>(() => feed.Publish(Second));
        var late = new Probe();
        using IDisposable ignored = feed.Subscribe(late);
        Assert.Same(failure, Assert.Single(late.Errors));
        Assert.Empty(late.Values);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ThrowingTerminalCallback_DoesNotPreventOtherNotificationsOrReopenTheFeed(bool fault)
    {
        var feed = new PriceFeed();
        var callbackFailure = new InvalidOperationException("bad terminal observer");
        using IDisposable a = feed.Subscribe(new Probe(completed: () => throw callbackFailure, error: _ => throw callbackFailure));
        var healthy = new Probe();
        using IDisposable b = feed.Subscribe(healthy);
        Action stop = fault ? () => feed.Fail(new Exception("stream failed")) : feed.Complete;
        AggregateException reported = Assert.Throws<AggregateException>(stop);
        Assert.Same(callbackFailure, Assert.Single(reported.InnerExceptions));
        Assert.Equal(fault ? 0 : 1, healthy.Completions);
        Assert.Equal(fault ? 1 : 0, healthy.Errors.Count);
        Assert.Throws<InvalidOperationException>(() => feed.Publish(First));
        // Disposal after terminal delivery remains harmless, including after observer failure.
        a.Dispose();
        b.Dispose();
    }

    [Fact]
    public void NestedPublication_IsRejectedAndDoesNotLeaveDeliveryStuck()
    {
        var feed = new PriceFeed();
        bool nested = true;
        using IDisposable a = feed.Subscribe(new Probe(next: _ => { if (nested) feed.Publish(Second); }));
        var healthy = new Probe();
        using IDisposable b = feed.Subscribe(healthy);
        AggregateException failure = Assert.Throws<AggregateException>(() => feed.Publish(First));
        Assert.IsType<InvalidOperationException>(Assert.Single(failure.InnerExceptions));
        Assert.Equal(new[] { First }, healthy.Values);
        nested = false;
        feed.Publish(Second);
        Assert.Equal(new[] { First, Second }, healthy.Values);
    }

    [Fact]
    public void TerminalRequestInsideOnNext_IsRejectedBeforeChangingFeedState()
    {
        var feed = new PriceFeed();
        bool stopInsideCallback = true;
        using IDisposable token = feed.Subscribe(new Probe(next: _ => { if (stopInsideCallback) feed.Complete(); }));
        AggregateException failure = Assert.Throws<AggregateException>(() => feed.Publish(First));
        Assert.IsType<InvalidOperationException>(Assert.Single(failure.InnerExceptions));
        stopInsideCallback = false;
        feed.Publish(Second);
        feed.Complete();
    }

    [Fact]
    public async Task CallbackRunsOutsideMembershipLock_WhileOverlappingPublishIsRejected()
    {
        var feed = new PriceFeed();
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        using IDisposable first = feed.Subscribe(new Probe(next: _ =>
        {
            entered.Set();
            if (!release.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException("test gate was not released");
        }));
        Task publication = Task.Run(() => feed.Publish(First));
        try
        {
            Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));
            await Task.Run(() =>
            {
                using IDisposable temporary = feed.Subscribe(new Probe());
                Assert.Throws<InvalidOperationException>(() => feed.Publish(Second));
            }).WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally { release.Set(); }
        await publication.WaitAsync(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Dispose_DoesNotCancelAlreadySelectedCallbackButRemovesFuturePublications()
    {
        var feed = new PriceFeed();
        using var entered = new ManualResetEventSlim();
        using var release = new ManualResetEventSlim();
        var observer = new Probe(next: _ =>
        {
            entered.Set();
            if (!release.Wait(TimeSpan.FromSeconds(10))) throw new TimeoutException("test gate was not released");
        });
        IDisposable token = feed.Subscribe(observer);
        Task publication = Task.Run(() => feed.Publish(First));
        try
        {
            Assert.True(entered.Wait(TimeSpan.FromSeconds(5)));
            token.Dispose();
        }
        finally { release.Set(); }
        await publication.WaitAsync(TimeSpan.FromSeconds(5));
        feed.Publish(Second);
        Assert.Equal(new[] { First }, observer.Values);
    }

    [Fact]
    public void LateSubscriberFailure_PropagatesAndPreservesTerminalOutcome()
    {
        var feed = new PriceFeed(1);
        feed.Complete();
        var failure = new InvalidOperationException("late callback failed");
        Assert.Same(failure, Assert.Throws<InvalidOperationException>(() =>
            feed.Subscribe(new Probe(completed: () => throw failure))));
        var healthy = new Probe();
        using IDisposable token = feed.Subscribe(healthy);
        Assert.Equal(1, healthy.Completions);
        Assert.Throws<InvalidOperationException>(() => feed.Publish(First));
    }

    [Fact]
    public void InvalidInputs_DoNotConsumeSubscriptionCapacityOrNotifyObservers()
    {
        var feed = new PriceFeed(1);
        Assert.Throws<ArgumentNullException>(() => feed.Subscribe(null!));
        var observer = new Probe();
        using IDisposable token = feed.Subscribe(observer);
        Assert.Throws<ArgumentNullException>(() => feed.Publish(null!));
        Assert.Throws<ArgumentNullException>(() => feed.Fail(null!));
        Assert.Empty(observer.Values);
        Assert.Empty(observer.Errors);
        feed.Publish(First);
        Assert.Single(observer.Values);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1000001)]
    [InlineData(1.001)]
    public void InvalidPrice_IsRejected(double value) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        new PriceUpdate("BOOK-1", (decimal)value));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingProductCode_IsRejected(string? code) => Assert.ThrowsAny<ArgumentException>(() =>
        new PriceUpdate(code!, 1m));

    [Fact]
    public void PriceSnapshot_IsNormalizedAndBounded()
    {
        Assert.Equal("BOOK-1", new PriceUpdate(" BOOK-1 ", 0m).ProductCode);
        Assert.Equal(1_000_000m, new PriceUpdate("BOOK-1", 1_000_000m).PriceUsd);
        Assert.Throws<ArgumentOutOfRangeException>(() => new PriceUpdate(new string('x', 33), 1m));
    }

    [Fact]
    public void EventsAlternative_RequiresExplicitUnsubscription()
    {
        var publisher = new EventPricePublisher();
        var display = new PriceDisplay();
        publisher.PriceChanged += display.OnNext;
        try { publisher.Publish(First); }
        finally { publisher.PriceChanged -= display.OnNext; }
        publisher.Publish(Second);
        Assert.Equal(First, display.Latest);
    }

    [Fact]
    public void EventsAlternative_UsesOrdinaryFailFastMulticastSemantics()
    {
        var publisher = new EventPricePublisher();
        var failure = new InvalidOperationException("handler failed");
        var received = new List<PriceUpdate>();
        Action<PriceUpdate> broken = _ => throw failure;
        publisher.PriceChanged += broken;
        publisher.PriceChanged += received.Add;
        try
        {
            Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => publisher.Publish(First)));
            Assert.Empty(received);
        }
        finally
        {
            publisher.PriceChanged -= broken;
            publisher.PriceChanged -= received.Add;
        }
    }

    [Fact]
    public void Demo_IsDeterministicAndOffline()
    {
        using var output = new StringWriter();
        ObserverDemo.Run(output);
        Assert.Equal(new[] { "Baseline latest USD: 20.00", "Unsubscribed display USD: 12.00",
            "Threshold matches: 1", "Active observer completed: True", "Late observer completed: True" },
            output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }

    // Throwing probes deliberately violate normal observer guidance to verify this provider's
    // documented isolation policy; production observers should avoid throwing from callbacks.
    private sealed class Probe(Action<PriceUpdate>? next = null, Action? completed = null,
        Action<Exception>? error = null) : IObserver<PriceUpdate>
    {
        public List<PriceUpdate> Values { get; } = [];
        public List<Exception> Errors { get; } = [];
        public int Completions { get; private set; }
        public void OnNext(PriceUpdate value) { Values.Add(value); next?.Invoke(value); }
        public void OnCompleted() { Completions++; completed?.Invoke(); }
        public void OnError(Exception failure) { Errors.Add(failure); error?.Invoke(failure); }
    }
}
