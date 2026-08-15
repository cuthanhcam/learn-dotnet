using AsyncConcurrency.Examples.Channels;

namespace AsyncConcurrency.Tests;

public sealed class ChannelWorkPoolTests
{
    [Fact]
    public async Task SelectAsync_BoundsWorkersAndPreservesInputOrder()
    {
        int active = 0;
        int peak = 0;

        int[] results = await ChannelWorkPool.SelectAsync(
            Enumerable.Range(1, 20),
            workerCount: 3,
            capacity: 2,
            async (value, token) =>
            {
                int current = Interlocked.Increment(ref active);
                UpdateMaximum(ref peak, current);
                try
                {
                    // Reverse delays deliberately make completion order differ from input order.
                    await Task.Delay(21 - value, token);
                    return value * value;
                }
                finally
                {
                    Interlocked.Decrement(ref active);
                }
            });

        Assert.Equal(Enumerable.Range(1, 20).Select(value => value * value), results);
        Assert.InRange(peak, 1, 3);
    }

    [Fact]
    public async Task SelectAsync_PropagatesWorkerFailureWithoutHangingProducer()
    {
        Task<int[]> operation = ChannelWorkPool.SelectAsync(
            Enumerable.Range(1, 1_000),
            workerCount: 2,
            capacity: 1,
            (value, _) => value == 3
                ? Task.FromException<int>(new InvalidOperationException("invalid item"))
                : Task.FromResult(value));

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            operation.WaitAsync(TimeSpan.FromSeconds(5)));

        Assert.Equal("invalid item", exception.Message);
    }

    [Fact]
    public async Task SelectAsync_ObservesCallerCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => ChannelWorkPool.SelectAsync(
            [1, 2, 3],
            workerCount: 1,
            capacity: 1,
            static (value, _) => Task.FromResult(value),
            cancellation.Token));
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
