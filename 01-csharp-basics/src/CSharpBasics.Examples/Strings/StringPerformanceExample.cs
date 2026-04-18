using System;
using System.Diagnostics;
using System.Text;

namespace CSharpBasics.Examples.Strings
{
    /// <summary>
    /// Comprehensive lesson for measuring and comparing string build performance.
    ///
    /// Understanding performance characteristics is crucial for backend development.
    /// String operations can become bottlenecks in high-throughput systems.
    ///
    /// Key topics:
    /// - Measuring performance with Stopwatch
    /// - Concatenation vs StringBuilder tradeoffs
    /// - Statistical significance in measurements
    /// - Warm-up iterations and JIT compilation effects
    /// - Memory allocation patterns
    ///
    /// Performance characteristics:
    /// - String concatenation (+): Creates new string each time
    /// - StringBuilder.Append: Reuses buffer, minimal allocations
    /// - Break-even: often around a few concatenations (depends on runtime + payload)
    /// - Gap widens exponentially with more operations
    ///
    /// Measurement best practices:
    /// - Run multiple iterations (garbage collection, CPU cache effects)
    /// - Calculate average to smooth out variance
    /// - Account for JIT warmup time
    /// - Measure in releases builds (Debug is pessimistic)
    /// - Use Stopwatch, not DateTime
    /// - Be suspicious of micro-benchmarks (can be misleading)
    /// - Prefer BenchmarkDotNet for production-grade measurements
    ///
    /// Real-world implications:
    /// - Log aggregation in loops: Use StringBuilder
    /// - JSON/CSV generation: Use StringBuilder
    /// - Simple string formatting: String interpolation fine
    /// - URL query parameters: Use StringBuilder or query encoder
    ///
    /// Benchmark considerations:
    /// - Stopwatch has overhead (OS scheduler, context switches)
    /// - Results vary based on system load
    /// - JIT compilation adds variance on first run
    /// - Garbage collection can significantly impact timing
    /// - Modern CPUs have branch prediction, cache effects
    ///
    /// When StringBuilder matters:
    /// - >10 string operations in a loop
    /// - Building strings in highly concurrent scenarios
    /// - Memory pressure in high-volume services
    /// - Generating large documents (HTML, CSV, JSON)
    /// </summary>
    public static class StringPerformanceExample
    {
        /// <summary>
        /// Entry point to run all performance demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} StringPerformanceExample {new string('=', 5)}");

            PrintSection("INTRODUCTION");
            DemoImportance();

            PrintSection("SINGLE RUN MEASUREMENT");
            DemoSingleRun();

            PrintSection("AVERAGED MEASUREMENT");
            DemoAveragedMeasurement();

            PrintSection("DETAILED BREAKDOWN");
            DemoDetailedAnalysis();

            PrintSection("MEMORY IMPACT");
            DemoMemoryImpact();

            PrintSection("PRACTICAL SCENARIOS");
            DemoPracticalScenarios();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Builds string using concatenation (inefficient).
        /// Demonstrates O(n2) complexity.
        /// </summary>
        public static string BuildWithConcatenation(int iterations)
        {
            if (iterations < 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            string result = string.Empty;
            for (int i = 0; i < iterations; i++)
            {
                result += i;  // Creates new string each iteration
            }

            return result;
        }

        /// <summary>
        /// Builds string using StringBuilder (efficient).
        /// Demonstrates O(n) complexity.
        /// </summary>
        public static string BuildWithStringBuilder(int iterations)
        {
            if (iterations < 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            var sb = new StringBuilder();
            for (int i = 0; i < iterations; i++)
            {
                sb.Append(i);  // Reuses buffer
            }

            return sb.ToString();
        }

        /// <summary>
        /// Measures single execution of both approaches.
        /// Returns (concatenation ms, stringbuilder ms).
        /// </summary>
        public static (long ConcatMilliseconds, long BuilderMilliseconds) MeasureExecution(int iterations)
        {
            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));

            // Warmup to allow JIT compilation
            _ = BuildWithConcatenation(10);
            _ = BuildWithStringBuilder(10);

            var sw = Stopwatch.StartNew();
            _ = BuildWithConcatenation(iterations);
            sw.Stop();
            long concatMs = sw.ElapsedMilliseconds;

            sw.Restart();
            _ = BuildWithStringBuilder(iterations);
            sw.Stop();
            long builderMs = sw.ElapsedMilliseconds;

            return (concatMs, builderMs);
        }

        /// <summary>
        /// Measures average performance over multiple runs.
        /// More statistically significant than single run.
        /// </summary>
        public static (double AvgConcatMs, double AvgBuilderMs) MeasureAverage(int iterations, int runs)
        {
            if (iterations <= 0)
                throw new ArgumentOutOfRangeException(nameof(iterations));
            if (runs <= 0)
                throw new ArgumentOutOfRangeException(nameof(runs));

            long concatTotal = 0;
            long builderTotal = 0;

            for (int i = 0; i < runs; i++)
            {
                // For teaching demos only: force GC to reduce run-to-run noise.
                // In real benchmarks, rely on benchmark framework orchestration.
                if (i > 0)
                    GC.Collect();

                (long concat, long builder) = MeasureExecution(iterations);
                concatTotal += concat;
                builderTotal += builder;
            }

            return ((double)concatTotal / runs, (double)builderTotal / runs);
        }

        /// <summary>
        /// Computes ratio: (Concatenation time) / (StringBuilder time).
        /// Ratios > 1 mean StringBuilder is faster.
        /// </summary>
        public static double ComputeSpeedupRatio(double concatMs, double builderMs)
        {
            if (builderMs <= 0.0)
                return double.MaxValue;

            return concatMs / builderMs;
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates why performance measurement matters.
        /// </summary>
        private static void DemoImportance()
        {
            Console.WriteLine("Why performance matters in backend development:");
            Console.WriteLine("  • High-volume systems process millions of requests");
            Console.WriteLine("  • String operations accumulate (logs, responses, queries)");
            Console.WriteLine("  • Even small inefficiencies multiply under load");
            Console.WriteLine("  • StringBuilder can reduce GC pressure significantly");
            Console.WriteLine("  • Proper measurement prevents premature optimization");
        }

        /// <summary>
        /// Demonstrates single measurement run.
        /// </summary>
        private static void DemoSingleRun()
        {
            const int iterations = 5_000;

            Console.WriteLine($"Building string with {iterations:N0} iterations:");
            Console.WriteLine();

            (long concatMs, long builderMs) = MeasureExecution(iterations);
            double ratio = ComputeSpeedupRatio(concatMs, builderMs);

            Console.WriteLine($"  String concatenation: {concatMs} ms");
            Console.WriteLine($"  StringBuilder: {builderMs} ms");
            Console.WriteLine($"  Speedup ratio: {ratio:F1}x");
            Console.WriteLine();
            Console.WriteLine("Note: Single run can be affected by GC, CPU scheduling");
        }

        /// <summary>
        /// Demonstrates averaged measurement.
        /// </summary>
        private static void DemoAveragedMeasurement()
        {
            const int iterations = 3_000;
            const int runs = 5;

            Console.WriteLine($"Average of {runs} runs with {iterations:N0} iterations each:");
            Console.WriteLine();

            (double avgConcat, double avgBuilder) = MeasureAverage(iterations, runs);
            double ratio = ComputeSpeedupRatio(avgConcat, avgBuilder);

            Console.WriteLine($"  Avg concatenation: {avgConcat:F2} ms");
            Console.WriteLine($"  Avg StringBuilder: {avgBuilder:F2} ms");
            Console.WriteLine($"  Speedup ratio: {ratio:F1}x");
            Console.WriteLine();
            Console.WriteLine("Average is more reliable than single run");
        }

        /// <summary>
        /// Shows performance at different iteration counts.
        /// </summary>
        private static void DemoDetailedAnalysis()
        {
            int[] iterationCounts = [100, 500, 1_000, 2_000, 5_000];

            Console.WriteLine("Performance at different scales:");
            Console.WriteLine($"  {"Iterations",12} | {"Concat (ms)",12} | {"StringBuilder",12} | {"Speedup",10}");
            Console.WriteLine("  " + new string('-', 55));

            foreach (int count in iterationCounts)
            {
                (long concat, long builder) = MeasureExecution(count);
                double ratio = ComputeSpeedupRatio(concat, builder);

                Console.WriteLine($"  {count,12:N0} | {concat,12} | {builder,12} | {ratio,9:F1}x");
            }

            Console.WriteLine();
            Console.WriteLine("Notice: speedup increases with iterations (quadratic vs linear)");
        }

        /// <summary>
        /// Demonstrates memory allocation differences.
        /// </summary>
        private static void DemoMemoryImpact()
        {
            Console.WriteLine("Memory allocation patterns:");
            Console.WriteLine();

            Console.WriteLine("String concatenation (n iterations):");
            Console.WriteLine("  Iteration 1: result = \"\" + 0     = \"0\"");
            Console.WriteLine("  Iteration 2: result = \"0\" + 1    = \"01\"");
            Console.WriteLine("  Iteration 3: result = \"01\" + 2   = \"012\"");
            Console.WriteLine("  Iteration 4: result = \"012\" + 3  = \"0123\"");
            Console.WriteLine("  Many intermediate strings are allocated");
            Console.WriteLine("  Cumulative copy work grows roughly quadratically");
            Console.WriteLine();

            Console.WriteLine("StringBuilder (same iterations):");
            Console.WriteLine("  Iteration 1: append 0");
            Console.WriteLine("  Iteration 2: append 1");
            Console.WriteLine("  Iteration 3: append 2");
            Console.WriteLine("  Iteration 4: append 3");
            Console.WriteLine("  Buffer grows by capacity strategy; fewer intermediate allocations");
            Console.WriteLine();

            Console.WriteLine("Result: StringBuilder drastically reduces GC pressure");
        }

        /// <summary>
        /// Demonstrates real-world scenarios where performance matters.
        /// </summary>
        private static void DemoPracticalScenarios()
        {
            Console.WriteLine("Real-world scenarios:");
            Console.WriteLine();

            Console.WriteLine("1. LOG AGGREGATION:");
            Console.WriteLine("   - Concatenation: Creates 100+ temporary strings");
            Console.WriteLine("   - StringBuilder: Single reusable buffer");
            Console.WriteLine("   - Impact: 50-100x performance difference");
            Console.WriteLine();

            Console.WriteLine("2. JSON/CSV GENERATION:");
            Console.WriteLine("   - API responses often contain 1000+ fields");
            Console.WriteLine("   - Concatenation becomes prohibitively slow");
            Console.WriteLine("   - StringBuilder handles easily");
            Console.WriteLine("   - Impact: Customer notices difference");
            Console.WriteLine();

            Console.WriteLine("3. HIGH-THROUGHPUT SERVICES:");
            Console.WriteLine("   - 1000 requests/second (typical cloud service)");
            Console.WriteLine("   - Each adds up over time");
            Console.WriteLine("   - 2ms saved per request = 2 seconds saved per 1000");
            Console.WriteLine("   - Impact: Significant cost savings on infrastructure");
            Console.WriteLine();

            Console.WriteLine("4. BATCH PROCESSING:");
            Console.WriteLine("   - Processing 1 million records");
            Console.WriteLine("   - Even 1ms per record multiplies to 1000 seconds");
            Console.WriteLine("   - Proper string building prevents timeouts");
            Console.WriteLine();

            Console.WriteLine("BEST PRACTICE:");
            Console.WriteLine("  Use StringBuilder whenever building dynamic strings");
            Console.WriteLine("  Exception: 1-2 concatenations for simple cases");
            Console.WriteLine("  For serious benchmarking, use BenchmarkDotNet");
        }
    }
}
