using MemoryPerformance.Examples.Profiling;

namespace MemoryPerformance.Tests.Profiling;

public class ProfilingExampleTests
{
    [Fact]
    public void Measure_Returns_Name_And_NonNegative_Metrics()
    {
        MeasurementResult result = ProfilingExample.Measure("noop", static () => { });

        Assert.Equal("noop", result.Name);
        Assert.True(result.Elapsed >= TimeSpan.Zero);
        Assert.True(result.AllocatedBytes >= 0);
    }

    [Fact]
    public void Run_Prints_Measurement()
    {
        string output = ConsoleCapture.Run(ProfilingExample.Run);

        Assert.Contains("allocate 100 arrays", output);
        Assert.Contains("bytes", output);
    }

    [Fact]
    public async Task MeasureAsync_AwaitsWork_AndReturnsNonNegativeMetrics()
    {
        bool completed = false;

        MeasurementResult result = await ProfilingExample.MeasureAsync(
            "async operation",
            async cancellationToken =>
            {
                await Task.Yield();
                cancellationToken.ThrowIfCancellationRequested();
                completed = true;
            });

        Assert.True(completed);
        Assert.Equal("async operation", result.Name);
        Assert.True(result.Elapsed >= TimeSpan.Zero);
        Assert.True(result.AllocatedBytes >= 0);
    }

    [Fact]
    public async Task MeasureAsync_PropagatesCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            ProfilingExample.MeasureAsync(
                "cancelled operation",
                cancellationToken => Task.FromCanceled(cancellationToken),
                cancellation.Token));
    }
}
