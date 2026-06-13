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
}
