using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    public abstract class LinqBenchmarkBase
    {
        protected LinqBenchmarkData Data { get; private set; } = null!;

        [GlobalSetup]
        public void Setup()
        {
            Data = LinqBenchmarkData.Create();
        }
    }
}
