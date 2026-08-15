using AsyncConcurrency.Examples.AsyncBasics;
using AsyncConcurrency.Examples.Cancellation;
using AsyncConcurrency.Examples.Channels;
using AsyncConcurrency.Examples.Synchronization;
using AsyncConcurrency.Exercises;

namespace AsyncConcurrency.Tests;

public sealed class AsyncExamplesTests
{
    [Fact]
    public async Task RunConcurrentlyAsync_PreservesInputOrder()
    {
        string[] result = await AsyncBasicsExample.RunConcurrentlyAsync(["one", "two", "three"]);

        Assert.Equal(["ONE", "TWO", "THREE"], result);
    }

    [Fact]
    public async Task CountUntilCancelledAsync_ObservesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            CancellationExample.CountUntilCancelledAsync(10, TimeSpan.FromMilliseconds(1), cancellation.Token));
    }

    [Fact]
    public async Task WithTimeoutAsync_TranslatesItsOwnTimeout()
    {
        await Assert.ThrowsAsync<TimeoutException>(() =>
            CancellationExample.WithTimeoutAsync(
                async token =>
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, token);
                    return 42;
                },
                TimeSpan.FromMilliseconds(10)));
    }

    [Fact]
    public async Task ThreadSafeCounter_DoesNotLoseConcurrentUpdates()
    {
        var counter = new ThreadSafeCounter();

        await Task.WhenAll(Enumerable.Range(0, 1_000).Select(_ => Task.Run(counter.Increment)));

        Assert.Equal(1_000, counter.Value);
    }

    [Fact]
    public async Task BoundedExecutor_PreservesOrderAndConcurrencyLimit()
    {
        int running = 0;
        int peak = 0;

        int[] result = await BoundedExecutor.SelectAsync(
            Enumerable.Range(1, 12),
            maxConcurrency: 3,
            async (value, token) =>
            {
                int current = Interlocked.Increment(ref running);
                UpdateMaximum(ref peak, current);
                try
                {
                    await Task.Delay(2, token);
                    return value * 2;
                }
                finally
                {
                    Interlocked.Decrement(ref running);
                }
            });

        Assert.Equal(Enumerable.Range(1, 12).Select(value => value * 2), result);
        Assert.InRange(peak, 1, 3);
    }

    [Fact]
    public async Task ChannelPipeline_AppliesBackpressureWithoutChangingOrder()
    {
        int[] result = await ChannelPipelineExample.SquareAsync([1, 2, 3, 4], capacity: 1);

        Assert.Equal([1, 4, 9, 16], result);
    }

    [Fact]
    public async Task AsyncMap_PropagatesCancellationToSelector()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => AsyncMap.MapAsync(
            new[] { 1, 2 },
            maxConcurrency: 1,
            static async (_, token) =>
            {
                await Task.Delay(1, token);
                return 1;
            },
            cancellation.Token));
    }

    [Fact]
    public async Task AsyncRetry_RetriesOnlyTransientFailures()
    {
        int attempts = 0;

        int result = await AsyncRetry.ExecuteAsync(
            _ => ++attempts < 3
                ? Task.FromException<int>(new IOException("Temporary failure."))
                : Task.FromResult(42),
            maxAttempts: 3,
            delay: TimeSpan.Zero,
            isTransient: exception => exception is IOException);

        Assert.Equal(42, result);
        Assert.Equal(3, attempts);
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
