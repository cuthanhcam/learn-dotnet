namespace AsyncConcurrency.Examples.Synchronization;

public sealed class AsyncConcurrencyLimiter : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public AsyncConcurrencyLimiter(int maximumConcurrency)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumConcurrency);
        _semaphore = new SemaphoreSlim(maximumConcurrency, maximumConcurrency);
    }

    public async ValueTask<Lease> EnterAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // WaitAsync suspends the caller without occupying a thread while no
        // permit is available. Ownership starts only after this await succeeds.
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new Lease(_semaphore);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _semaphore.Dispose();
    }

    public sealed class Lease : IDisposable, IAsyncDisposable
    {
        private SemaphoreSlim? _owner;

        internal Lease(SemaphoreSlim owner) => _owner = owner;

        public void Dispose()
        {
            // Exchange makes repeated Dispose/DisposeAsync calls harmless and
            // prevents an accidental double release from inflating the count.
            SemaphoreSlim? owner = Interlocked.Exchange(ref _owner, null);
            owner?.Release();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
