using System.Diagnostics;

namespace MemoryPerformance.Examples.Profiling;

/// <summary>
/// Small measurement helpers for learning. Use BenchmarkDotNet for serious conclusions.
/// </summary>
public static class ProfilingExample
{
    public static void Run()
    {
        MeasurementResult result = Measure("allocate 100 arrays", static () =>
        {
            for (int i = 0; i < 100; i++)
            {
                _ = new byte[256];
            }
        });

        Console.WriteLine(result);
    }

    public static MeasurementResult Measure(string name, Action action)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(action);

        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        int gen0Before = GC.CollectionCount(0);
        int gen1Before = GC.CollectionCount(1);
        int gen2Before = GC.CollectionCount(2);

        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();

        long allocatedAfter = GC.GetAllocatedBytesForCurrentThread();

        return new MeasurementResult(
            Name: name,
            Elapsed: stopwatch.Elapsed,
            AllocatedBytes: allocatedAfter - allocatedBefore,
            Gen0Collections: GC.CollectionCount(0) - gen0Before,
            Gen1Collections: GC.CollectionCount(1) - gen1Before,
            Gen2Collections: GC.CollectionCount(2) - gen2Before);
    }
}

public readonly record struct MeasurementResult(
    string Name,
    TimeSpan Elapsed,
    long AllocatedBytes,
    int Gen0Collections,
    int Gen1Collections,
    int Gen2Collections)
{
    public override string ToString()
    {
        return $"{Name}: {Elapsed.TotalMilliseconds:F3} ms, {AllocatedBytes:N0} bytes, GC({Gen0Collections}/{Gen1Collections}/{Gen2Collections})";
    }
}
