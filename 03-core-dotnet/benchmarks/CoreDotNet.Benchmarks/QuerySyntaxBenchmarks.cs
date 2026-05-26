using BenchmarkDotNet.Attributes;

namespace CoreDotNet.Benchmarks
{
    [MemoryDiagnoser]
    public class QuerySyntaxBenchmarks : LinqBenchmarkBase
    {
        [Benchmark(Baseline = true)]
        public decimal QuerySyntax_FilterOrderProject()
        {
            var query =
                from product in Data.Products
                where product.Category == "Electronics" && product.Price >= 100m
                orderby product.Price descending
                select product.Price;

            return query.Sum();
        }

        [Benchmark]
        public decimal MethodSyntax_FilterOrderProject()
        {
            return Data.Products
                .Where(product => product.Category == "Electronics" && product.Price >= 100m)
                .OrderByDescending(product => product.Price)
                .Select(product => product.Price)
                .Sum();
        }
    }
}
