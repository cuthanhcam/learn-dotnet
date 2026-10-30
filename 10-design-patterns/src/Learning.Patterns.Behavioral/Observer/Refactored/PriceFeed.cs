namespace Learning.Patterns.Behavioral.Observer.Refactored;

/// <summary>
/// Synchronous, bounded, in-process push. Subscribe/Dispose synchronize membership; callbacks run
/// outside the lock. Overlapping or nested publication is rejected rather than silently queued.
/// This provider adds an explicit best-effort-batch/aggregate-failure policy to IObservable.
/// </summary>
public sealed class PriceFeed : IObservable<PriceUpdate>
{
    private readonly object _gate = new();
    private readonly List<Subscription> _subscriptions = [];
    private readonly int _capacity;
    private FeedState _state = FeedState.Active;
    private Exception? _terminalError;
    private bool _delivering;

    public PriceFeed(int capacity = 32)
    {
        if (capacity is < 1 or > 64)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Subscription capacity must be 1..64.");
        _capacity = capacity;
    }

    public IDisposable Subscribe(IObserver<PriceUpdate> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);
        Exception? error;
        lock (_gate)
        {
            if (_state == FeedState.Active)
            {
                if (_subscriptions.Any(subscription => ReferenceEquals(subscription.Observer, observer)))
                    throw new InvalidOperationException("This observer already has an active subscription.");
                if (_subscriptions.Count == _capacity)
                    throw new InvalidOperationException("The subscription budget is full.");
                var subscription = new Subscription(this, observer);
                _subscriptions.Add(subscription);
                return subscription;
            }
            error = _terminalError;
        }

        // Late subscribers receive the terminal signal inline, outside the lock. They are not
        // retained, even if their callback throws. No old prices are replayed.
        if (error is null)
            observer.OnCompleted();
        else
            observer.OnError(error);
        return EmptySubscription.Instance;
    }

    public void Publish(PriceUpdate update)
    {
        ArgumentNullException.ThrowIfNull(update);
        Deliver(observer => observer.OnNext(update));
    }

    public void Complete() => Deliver(observer => observer.OnCompleted(), FeedState.Completed);

    public void Fail(Exception error)
    {
        ArgumentNullException.ThrowIfNull(error);
        Deliver(observer => observer.OnError(error), FeedState.Failed, error);
    }

    private void Deliver(Action<IObserver<PriceUpdate>> notify, FeedState? terminal = null, Exception? error = null)
    {
        Subscription[] snapshot;
        lock (_gate)
        {
            if (_state != FeedState.Active)
            {
                if (terminal.HasValue)
                    return; // Repeated terminal requests do not change the original outcome.
                throw new InvalidOperationException("The feed has stopped.");
            }
            if (_delivering)
                throw new InvalidOperationException("Nested or overlapping delivery is not supported.");
            _delivering = true;
            snapshot = _subscriptions.ToArray();
            if (terminal.HasValue)
            {
                _state = terminal.Value;
                _terminalError = error;
                _subscriptions.Clear();
            }
        }

        List<Exception> failures = [];
        try
        {
            // This example traverses registration order. A subscription added by a callback joins
            // the NEXT batch; one disposed before selection is skipped. Clients must not assume
            // that a different IObservable implementation has these same extra ordering rules.
            foreach (Subscription subscription in snapshot)
            {
                IObserver<PriceUpdate>? observer = subscription.Observer;
                if (observer is null)
                    continue;
                if (terminal.HasValue)
                    subscription.Dispose();
                try
                {
                    notify(observer);
                }
                catch (Exception failure)
                {
                    // A bad observer does not prevent attempts to notify the rest of this batch.
                    // This is NOT a stream failure: never recursively call OnError on that observer.
                    failures.Add(failure);
                }
            }
        }
        finally
        {
            if (terminal.HasValue)
                foreach (Subscription subscription in snapshot)
                    subscription.Dispose();
            lock (_gate)
                _delivering = false;
        }
        if (failures.Count > 0)
            throw new AggregateException("Observer callbacks failed; the batch was not rolled back.", failures);
    }

    private void Remove(Subscription subscription)
    {
        lock (_gate)
            _subscriptions.Remove(subscription);
    }

    private enum FeedState { Active, Completed, Failed }

    private sealed class Subscription(PriceFeed owner, IObserver<PriceUpdate> observer) : IDisposable
    {
        private PriceFeed? _owner = owner;
        private IObserver<PriceUpdate>? _observer = observer;
        public IObserver<PriceUpdate>? Observer => Volatile.Read(ref _observer);

        public void Dispose()
        {
            PriceFeed? current = Interlocked.Exchange(ref _owner, null);
            Interlocked.Exchange(ref _observer, null);
            current?.Remove(this);
            // Idempotent; no callback runs here. A callback selected by an in-flight batch may
            // still run after disposal. This is not a wait-for-all-callbacks barrier.
        }
    }

    private sealed class EmptySubscription : IDisposable
    {
        public static readonly EmptySubscription Instance = new();
        public void Dispose() { }
    }
}
