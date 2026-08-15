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

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition())
        {
            await Task.Delay(1, timeout.Token);
        }
    }
}
