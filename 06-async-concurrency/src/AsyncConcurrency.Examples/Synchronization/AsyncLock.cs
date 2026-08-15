namespace AsyncConcurrency.Examples.Synchronization;

/// <summary>
/// Provides asynchronous mutual exclusion for a short critical section.
/// Unlike <c>lock</c>, waiting for this lock does not block a worker thread.
/// </summary>
/// <remarks>
/// This type is intentionally non-reentrant. Code that already owns the lock must not try
/// to enter it again, because the second acquisition waits for the first lease to be released.
/// Keep critical sections small and avoid calling unknown application code while holding them.
/// </remarks>
public sealed class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim _gate = new(initialCount: 1, maxCount: 1);
    private int _disposed;

    public async ValueTask<Releaser> EnterAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);

        // Ownership begins only after WaitAsync completes successfully. There is therefore
        // nothing to release when waiting is canceled or faults.
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        return new Releaser(_gate);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            // The owner of this type must ensure no lease or waiter remains during disposal.
            // Disposing a synchronization primitive while it is in use is a lifetime bug.
            _gate.Dispose();
        }
    }

    public sealed class Releaser : IDisposable, IAsyncDisposable
    {
        private SemaphoreSlim? _gate;

        internal Releaser(SemaphoreSlim gate) => _gate = gate;

        public void Dispose()
        {
            // Idempotent release prevents an accidental double Dispose from increasing the
            // semaphore count beyond one and violating mutual exclusion.
            Interlocked.Exchange(ref _gate, null)?.Release();
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
