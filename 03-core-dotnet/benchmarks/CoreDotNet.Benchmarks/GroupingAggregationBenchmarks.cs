using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class GroupingAggregationBenchmarks : LinqBenchmarkBase
    {
        [Benchmark]
        public decimal GroupByCategorySummary()
        {
            return Data.Products
                .GroupBy(product => product.Category)
                .Sum(group => group.Sum(product => product.Price));
        }

        [Benchmark(Baseline = true)]
        public decimal ManualCategorySummary()
        {
            var totals = new Dictionary<string, decimal>(StringComparer.Ordinal);

            foreach (var product in Data.Products)
            {
                totals.TryGetValue(product.Category, out decimal total);
                totals[product.Category] = total + product.Price;
            }

            return totals.Values.Sum();
        }
    }
}
