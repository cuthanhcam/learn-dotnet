using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class DeferredExecutionBenchmarks : LinqBenchmarkBase
    {
        [Benchmark(Baseline = true)]
        public int DeferredQueryEnumeratedTwice()
        {
            var query = Data.DeferredNumbers.Where(number => number % 2 == 0);

            int firstPass = query.Count();
            int secondPass = query.Sum();

            return firstPass + secondPass;
        }

        [Benchmark]
        public int MaterializedQueryOnce()
        {
            var materialized = Data.DeferredNumbers
                .Where(number => number % 2 == 0)
                .ToList();

            return materialized.Count + materialized.Sum();
        }
    }
}
