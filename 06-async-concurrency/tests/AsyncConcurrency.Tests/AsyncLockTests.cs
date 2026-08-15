using AsyncConcurrency.Examples.Synchronization;

namespace AsyncConcurrency.Tests;

public sealed class AsyncLockTests
{
    [Fact]
    public async Task EnterAsync_SerializesCompoundUpdates()
    {
        using var mutex = new AsyncLock();
        int balance = 1_000;

        Task[] withdrawals = Enumerable.Range(0, 100).Select(async _ =>
        {
            await using AsyncLock.Releaser lease = await mutex.EnterAsync();

            // The invariant spans a check and an update. Interlocked on the subtraction alone
            // would not make this entire decision atomic.
            if (balance >= 10)
            {
                await Task.Yield();
                balance -= 10;
            }
        }).ToArray();

        await Task.WhenAll(withdrawals);

        Assert.Equal(0, balance);
    }

    [Fact]
    public async Task EnterAsync_CancelsWaiterWithoutReleasingOwnersLease()
    {
        using var mutex = new AsyncLock();
        await using AsyncLock.Releaser owner = await mutex.EnterAsync();
        using var cancellation = new CancellationTokenSource();

        ValueTask<AsyncLock.Releaser> waiting = mutex.EnterAsync(cancellation.Token);
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => waiting.AsTask());
    }

    [Fact]
    public async Task Releaser_MultipleDisposalsReleaseOnlyOnce()
    {
        using var mutex = new AsyncLock();
        AsyncLock.Releaser lease = await mutex.EnterAsync();

        lease.Dispose();
        await lease.DisposeAsync();

        await using AsyncLock.Releaser next = await mutex.EnterAsync();
    }
}
