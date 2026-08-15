using AsyncConcurrency.Examples.Collections;

namespace AsyncConcurrency.Tests;

public sealed class AsyncMemoizerTests
{
    [Fact]
    public async Task ConcurrentCallersShareOneSuccessfulFactoryExecution()
    {
        var memoizer = new AsyncMemoizer<string, int>(StringComparer.OrdinalIgnoreCase);
        int factoryCalls = 0;
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        Task<int>[] calls = Enumerable.Range(0, 20).Select(_ => memoizer.GetOrAddAsync(
            "answer",
            async (_, _) =>
            {
                Interlocked.Increment(ref factoryCalls);
                await release.Task;
                return 42;
            })).ToArray();

        await WaitUntilAsync(() => Volatile.Read(ref factoryCalls) == 1);
        release.SetResult();

        Assert.All(await Task.WhenAll(calls), value => Assert.Equal(42, value));
        Assert.Equal(1, factoryCalls);
        Assert.Equal(1, memoizer.Count);
    }

    [Fact]
    public async Task FaultedEntryIsRemovedSoLaterCallCanRetry()
    {
        var memoizer = new AsyncMemoizer<string, int>();
        int calls = 0;

        await Assert.ThrowsAsync<IOException>(() => memoizer.GetOrAddAsync(
            "key",
            (_, _) =>
            {
                calls++;
                return Task.FromException<int>(new IOException("temporary"));
            }));

        int result = await memoizer.GetOrAddAsync(
            "key",
            (_, _) =>
            {
                calls++;
                return Task.FromResult(7);
            });

        Assert.Equal(7, result);
        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task CancelingOneWaiterDoesNotCancelSharedOperation()
    {
        var memoizer = new AsyncMemoizer<string, int>();
        var releaseFactory = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var callerCancellation = new CancellationTokenSource();
        int factoryCalls = 0;

        Task<int> canceledWait = memoizer.GetOrAddAsync(
            "shared",
            async (_, operationToken) =>
            {
                Interlocked.Increment(ref factoryCalls);
                await releaseFactory.Task.WaitAsync(operationToken);
                return 42;
            },
            callerCancellation.Token);

        Task<int> survivingWait = memoizer.GetOrAddAsync(
            "shared",
            (_, _) => Task.FromResult(-1));

        await WaitUntilAsync(() => Volatile.Read(ref factoryCalls) == 1);
        callerCancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => canceledWait);

        releaseFactory.SetResult();

        Assert.Equal(42, await survivingWait);
        Assert.Equal(1, factoryCalls);
        Assert.Equal(1, memoizer.Count);
    }

    [Fact]
    public async Task SharedLifetimeCancellationCancelsAndEvictsOperation()
    {
        using var lifetime = new CancellationTokenSource();
        var memoizer = new AsyncMemoizer<string, int>(sharedLifetimeToken: lifetime.Token);

        Task<int> operation = memoizer.GetOrAddAsync(
            "key",
            async (_, token) =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
                return 1;
            });

        lifetime.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation);
        await WaitUntilAsync(() => memoizer.Count == 0);

        Assert.Equal(0, memoizer.Count);
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition())
        {
            await Task.Delay(1, timeout.Token);
        }
    }
}
