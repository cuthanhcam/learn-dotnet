using AsyncConcurrency.Examples.AsyncBasics;
using AsyncConcurrency.Examples.Synchronization;

namespace AsyncConcurrency.Tests;

public sealed class AdvancedCoordinationTests
{
    [Fact]
    public async Task ConcurrencyLimiter_BoundsEntryAndReleasesWithAwaitUsing()
    {
        using var limiter = new AsyncConcurrencyLimiter(2);
        int active = 0;
        int peak = 0;
        var releaseWorkers = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task[] workers = Enumerable.Range(0, 6).Select(async _ =>
        {
            await using AsyncConcurrencyLimiter.Lease lease = await limiter.EnterAsync();
            int current = Interlocked.Increment(ref active);
            UpdateMaximum(ref peak, current);
            try
            {
                await releaseWorkers.Task;
            }
            finally
            {
                Interlocked.Decrement(ref active);
            }
        }).ToArray();

        await WaitUntilAsync(() => Volatile.Read(ref active) == 2);
        Assert.Equal(2, peak);

        releaseWorkers.SetResult();
        await Task.WhenAll(workers);
        Assert.Equal(0, active);
    }

    [Fact]
    public async Task ConcurrencyLimiter_LeaseCannotReleaseTwice()
    {
        using var limiter = new AsyncConcurrencyLimiter(1);
        AsyncConcurrencyLimiter.Lease lease = await limiter.EnterAsync();

        lease.Dispose();
        lease.Dispose();

        await using AsyncConcurrencyLimiter.Lease next = await limiter.EnterAsync();
    }

    [Fact]
    public async Task ManualResetEvent_ReleasesAllWaitersAndCanReset()
    {
        var signal = new AsyncManualResetEvent();
        Task first = signal.WaitAsync();
        Task second = signal.WaitAsync();

        signal.Set();
        await Task.WhenAll(first, second);
        Assert.True(signal.IsSet);

        signal.Reset();
        Task afterReset = signal.WaitAsync();
        Assert.False(afterReset.IsCompleted);

        signal.Set();
        await afterReset;
    }

    [Fact]
    public async Task ManualResetEvent_CancelsOneWaitWithoutCancelingSignal()
    {
        var signal = new AsyncManualResetEvent();
        using var cancellation = new CancellationTokenSource();
        Task canceledWait = signal.WaitAsync(cancellation.Token);
        Task survivingWait = signal.WaitAsync();

        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => canceledWait);

        signal.Set();
        await survivingWait;
    }

    [Fact]
    public async Task InCompletionOrder_YieldsAsTasksFinish()
    {
        var first = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var second = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        IAsyncEnumerator<int> enumerator = TaskCompositionExample
            .InCompletionOrder([first.Task, second.Task])
            .GetAsyncEnumerator();

        Task<bool> move = enumerator.MoveNextAsync().AsTask();
        second.SetResult(2);

        Assert.True(await move);
        Assert.Equal(2, enumerator.Current);

        first.SetResult(1);
        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal(1, enumerator.Current);
        Assert.False(await enumerator.MoveNextAsync());
        await enumerator.DisposeAsync();
    }

    [Fact]
    public async Task ObserveAllFailuresAsync_ReturnsEveryFailure()
    {
        Exception[] failures = await TaskCompositionExample.ObserveAllFailuresAsync(
        [
            Task.FromException(new IOException("first")),
            Task.FromException(new InvalidOperationException("second"))
        ]);

        Assert.Collection(
            failures.OrderBy(exception => exception.Message),
            exception => Assert.IsType<IOException>(exception),
            exception => Assert.IsType<InvalidOperationException>(exception));
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition())
        {
            await Task.Delay(1, timeout.Token);
        }
    }

    private static void UpdateMaximum(ref int maximum, int candidate)
    {
        int snapshot;
        do
        {
            snapshot = Volatile.Read(ref maximum);
            if (candidate <= snapshot)
            {
                return;
            }
        }
        while (Interlocked.CompareExchange(ref maximum, candidate, snapshot) != snapshot);
    }
}
