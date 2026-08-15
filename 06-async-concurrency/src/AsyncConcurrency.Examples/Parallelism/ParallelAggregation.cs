namespace AsyncConcurrency.Examples.Parallelism;

public static class ParallelAggregation
{
    /// <summary>
    /// Computes a CPU-bound sum using one accumulator per partition.
    /// </summary>
    public static long SumOfSquares(IEnumerable<int> values, int maxDegreeOfParallelism)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxDegreeOfParallelism);

        long total = 0;
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegreeOfParallelism
        };

        Parallel.ForEach(
            values,
            options,
            localInit: static () => 0L,
            body: static (value, _, localTotal) =>
                // Convert before multiplication so int overflow cannot occur first.
                checked(localTotal + ((long)value * value)),
            localFinally: localTotal =>
                // Contention occurs once per partition rather than once per input element.
                Interlocked.Add(ref total, localTotal));

        return total;
    }
}
