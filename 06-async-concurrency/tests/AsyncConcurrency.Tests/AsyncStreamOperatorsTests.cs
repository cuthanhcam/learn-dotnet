using System.Runtime.CompilerServices;
using AsyncConcurrency.Examples.AsyncBasics;

namespace AsyncConcurrency.Tests;

public sealed class AsyncStreamOperatorsTests
{
    [Fact]
    public async Task Buffer_EmitsFullBatchesAndPartialFinalBatch()
    {
        List<int[]> batches = [];

        await foreach (int[] batch in AsyncStreamOperators.Buffer(GenerateAsync(1, 2, 3, 4, 5), 2))
        {
            batches.Add(batch);
        }

        Assert.Collection(
            batches,
            batch => Assert.Equal([1, 2], batch),
            batch => Assert.Equal([3, 4], batch),
            batch => Assert.Equal([5], batch));
    }

    [Fact]
    public async Task SelectAwait_IsSequentialAndPreservesOrder()
    {
        int active = 0;
        int peak = 0;

        IReadOnlyList<int> values = await AsyncStreamOperators.ToBoundedListAsync(
            AsyncStreamOperators.SelectAwait(
                GenerateAsync(1, 2, 3),
                async (value, token) =>
                {
                    int current = Interlocked.Increment(ref active);
                    peak = Math.Max(peak, current);
                    try
                    {
                        await Task.Yield();
                        token.ThrowIfCancellationRequested();
                        return value * 10;
                    }
                    finally
                    {
                        Interlocked.Decrement(ref active);
                    }
                }),
            maximumCount: 3);

        Assert.Equal([10, 20, 30], values);
        Assert.Equal(1, peak);
    }

    [Fact]
    public async Task ToBoundedListAsync_RejectsStreamBeyondLimit()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            AsyncStreamOperators.ToBoundedListAsync(GenerateAsync(1, 2, 3), maximumCount: 2));
    }

    [Fact]
    public async Task Buffer_PropagatesEnumerationCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        IAsyncEnumerator<int[]> enumerator = AsyncStreamOperators
            .Buffer(CancellableSource(cancellation.Token), batchSize: 1, cancellation.Token)
            .GetAsyncEnumerator();

        Assert.True(await enumerator.MoveNextAsync());
        Assert.Equal([1], enumerator.Current);

        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            enumerator.MoveNextAsync().AsTask());
        await enumerator.DisposeAsync();
    }

    private static async IAsyncEnumerable<int> GenerateAsync(params int[] values)
    {
        foreach (int value in values)
        {
            await Task.Yield();
            yield return value;
        }
    }

    private static async IAsyncEnumerable<int> CancellableSource(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return 1;
        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
    }
}
