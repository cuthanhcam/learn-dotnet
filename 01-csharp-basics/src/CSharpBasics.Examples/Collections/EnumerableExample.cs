namespace CSharpBasics.Examples.Collections
{
    /// <summary>
    /// Comprehensive lesson for IEnumerable, yield, and lazy-style helpers.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// IEnumerable vs IEnumerable<T>:
    /// - IEnumerable: Non-generic, boxes value types (avoid)
    /// - IEnumerable<T>: Preferred, type-safe, generic
    /// 
    /// Key topics:
    /// - Lazy evaluation with yield return
    /// - Deferred execution patterns
    /// - LINQ methods vs manual iteration
    /// - Generator methods
    /// - Filtering, projection, aggregation
    /// - Windowing operations (Take, Skip)
    /// - Batching (chunking) data
    /// 
    /// When to use IEnumerable<T>:
    /// - Streaming data from sources
    /// - Lazy evaluation needed
    /// - Composing multiple operations
    /// - Unknown or infinite sequences
    /// - Generator methods with yield
    /// 
    /// When NOT to use IEnumerable<T>:
    /// - Need indexed access → use List<T> or T[]
    /// - Need to modify original → explicit materialization
    /// - Need count without iteration → materialize
    /// - Performance-critical with multiple iterations → cache to List<T>
    /// 
    /// Best practices:
    /// - Return IEnumerable<T> from methods, not List<T>
    /// - Use yield return for generator methods
    /// - Be careful with side effects in predicates
    /// - Document if IEnumerable is consumed multiple times
    /// - Materialize (ToList) only when necessary
    /// - Use LINQ methods for consistency
    /// </summary>
    public static class EnumerableExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} EnumerableExample {new string('=', 5)}");

            PrintSection("LAZY EVALUATION WITH YIELD");
            DemoLazyEvaluation();

            PrintSection("FILTERING WITH YIELD");
            DemoFiltering();

            PrintSection("PROJECTION");
            DemoProjection();

            PrintSection("AGGREGATION");
            DemoAggregation();

            PrintSection("WINDOWING (TAKE & SKIP)");
            DemoWindowing();

            PrintSection("BATCHING (CHUNK)");
            DemoBatching();

            PrintSection("DEFERRED EXECUTION GOTCHA");
            DemoDeferredExecutionGotcha();

            PrintSection("INFINITE SEQUENCES");
            DemoInfiniteSequences();

            PrintSection("PERFORMANCE NOTES");
            DemoPerformanceNotes();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Generates even numbers using yield return.
        /// Lazy evaluation: numbers generated on-demand.
        /// Time complexity: O(n) when fully enumerated.
        /// </summary>
        public static IEnumerable<int> FilterEvenNumbers(IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            foreach (int number in numbers)
            {
                if (number % 2 == 0)
                    yield return number;
            }
        }

        /// <summary>
        /// Converts words to uppercase, skipping null/whitespace values.
        /// Deferred execution until enumeration starts.
        /// </summary>
        public static IEnumerable<string> ToUpperWords(IEnumerable<string?> words)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words));

            return words
                .Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => word!.ToUpperInvariant());
        }

        /// <summary>
        /// Sums numbers manually.
        /// Demonstrates explicit iteration for control.
        /// Time complexity: O(n), space: O(1).
        /// </summary>
        public static int Sum(IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            int total = 0;
            foreach (int number in numbers)
            {
                total += number;
            }

            return total;
        }

        /// <summary>
        /// Takes first N elements using yield return.
        /// Lazy: stops iteration as soon as N elements yielded.
        /// Time complexity: O(min(n, count)).
        /// </summary>
        public static IEnumerable<int> Take(IEnumerable<int> values, int count)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (count <= 0)
                yield break;  // Early exit

            int yielded = 0;
            foreach (int value in values)
            {
                yield return value;
                yielded++;

                if (yielded >= count)
                    yield break;  // Stop after N items
            }
        }

        /// <summary>
        /// Skips first N elements using yield return.
        /// Lazy: doesn't skip until iteration starts.
        /// Time complexity: O(n) when fully enumerated.
        /// </summary>
        public static IEnumerable<int> Skip(IEnumerable<int> values, int count)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Skip count cannot be negative.");

            int skipped = 0;
            foreach (int value in values)
            {
                if (skipped < count)
                {
                    skipped++;
                    continue;
                }

                yield return value;
            }
        }

        /// <summary>
        /// Batches source values into chunks of fixed size.
        /// Materializes each chunk to an array.
        /// Time complexity: O(n), space: O(size) per yielded batch.
        /// </summary>
        public static IEnumerable<int[]> Batch(IEnumerable<int> values, int size)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "Batch size must be greater than zero.");

            List<int> bucket = new(size);
            foreach (int value in values)
            {
                bucket.Add(value);
                if (bucket.Count == size)
                {
                    yield return [.. bucket];
                    bucket.Clear();
                }
            }

            if (bucket.Count > 0)
            {
                yield return [.. bucket];
            }
        }

        /// <summary>
        /// Generates infinite sequence of numbers.
        /// Demonstrates power of lazy evaluation with yield.
        /// WARNING: Only consume with Take() to avoid infinite loop!
        /// </summary>
        public static IEnumerable<int> CountFrom(int start = 0)
        {
            int current = start;
            while (true)
            {
                yield return current++;
            }
        }

        /// <summary>
        /// Generates Fibonacci sequence lazily.
        /// Another infinite sequence example.
        /// </summary>
        public static IEnumerable<long> GenerateFibonacci()
        {
            long a = 0;
            long b = 1;

            while (true)
            {
                yield return a;
                (a, b) = (b, a + b);
            }
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates lazy evaluation benefit.
        /// The generator method doesn't execute until we enumerate.
        /// </summary>
        private static void DemoLazyEvaluation()
        {
            Console.WriteLine("Creating enumerable (not executed yet):");
            IEnumerable<int> evens = FilterEvenNumbers([1, 2, 3, 4, 5, 6, 7, 8]);
            Console.WriteLine("  → IEnumerable created (no iteration)");

            Console.WriteLine("Enumerating (now executed):");
            foreach (int num in evens)
            {
                Console.Write($"{num} ");
            }
            Console.WriteLine();

            Console.WriteLine("✓ Benefit: Only processes data as needed!");
        }

        /// <summary>
        /// Demonstrates filtering with yield.
        /// </summary>
        private static void DemoFiltering()
        {
            int[] numbers = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
            var evens = FilterEvenNumbers(numbers);
            Console.WriteLine($"Even numbers: {string.Join(", ", evens)}");

            // Each call re-enumerates source unless materialized with ToList/ToArray.
            Console.WriteLine($"Even count: {evens.Count()}");
            Console.WriteLine($"Even sum: {evens.Sum()}");
            Console.WriteLine($"Max even: {evens.Max()}");
        }

        /// <summary>
        /// Demonstrates projection with LINQ Select.
        /// </summary>
        private static void DemoProjection()
        {
            string?[] words = ["csharp", null, " ", "fundamentals", "learning"];
            var upperWords = ToUpperWords(words);

            Console.WriteLine($"Original: {string.Join(", ", words.Select(w => w ?? "<null>"))}");
            Console.WriteLine($"Uppercase (non-empty only): {string.Join(", ", upperWords)}");
        }

        /// <summary>
        /// Demonstrates aggregation operations.
        /// </summary>
        private static void DemoAggregation()
        {
            int[] numbers = [1, 2, 3, 4, 5];
            Console.WriteLine($"Numbers: {string.Join(", ", numbers)}");
            Console.WriteLine($"Sum: {Sum(numbers)}");
            Console.WriteLine($"Count: {numbers.Count()}");
            Console.WriteLine($"Average: {numbers.Average():0.00}");
        }

        /// <summary>
        /// Demonstrates Take and Skip operations.
        /// </summary>
        private static void DemoWindowing()
        {
            int[] numbers = [10, 20, 30, 40, 50, 60, 70, 80];
            Console.WriteLine($"Numbers: {string.Join(", ", numbers)}");

            var first3 = Take(numbers, 3);
            Console.WriteLine($"Take(3): {string.Join(", ", first3)}");

            var skipped2 = Skip(numbers, 2);
            Console.WriteLine($"Skip(2): {string.Join(", ", skipped2)}");

            // Combine: Skip 2, then take 3
            var window = Take(Skip(numbers, 2), 3);
            Console.WriteLine($"Skip(2).Take(3): {string.Join(", ", window)}");
        }

        /// <summary>
        /// Demonstrates batching operation.
        /// </summary>
        private static void DemoBatching()
        {
            int[] numbers = [1, 2, 3, 4, 5, 6, 7];
            var batches = Batch(numbers, 3);

            Console.WriteLine("Batches of size 3:");
            foreach (int[] batch in batches)
            {
                Console.WriteLine($"  [{string.Join(", ", batch)}]");
            }
        }

        /// <summary>
        /// Demonstrates deferred execution gotcha.
        /// </summary>
        private static void DemoDeferredExecutionGotcha()
        {
            List<int> source = [1, 2, 3];
            IEnumerable<int> query = source.Where(x => x >= 2);

            // Query has not executed yet; this mutation is still observed later.
            source.Add(4);

            Console.WriteLine("Deferred query reflects latest source state:");
            Console.WriteLine($"Query result: {string.Join(", ", query)}");

            List<int> cached = query.ToList();
            // Cached list is now detached from source changes.
            source.Add(5);
            Console.WriteLine($"Cached result (materialized): {string.Join(", ", cached)}");
        }

        /// <summary>
        /// Demonstrates infinite sequences with lazy evaluation.
        /// Shows power and danger of IEnumerable!
        /// </summary>
        private static void DemoInfiniteSequences()
        {
            Console.WriteLine("Infinite counter (first 5):");
            foreach (int num in CountFrom(0).Take(5))
            {
                Console.Write($"{num} ");
            }
            Console.WriteLine();

            Console.WriteLine("Fibonacci sequence (first 10):");
            foreach (long num in GenerateFibonacci().Take(10))
            {
                Console.Write($"{num} ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Demonstrates performance notes and best practices.
        /// </summary>
        private static void DemoPerformanceNotes()
        {
            Console.WriteLine("IEnumerable pipelines are lazy by default and memory-efficient.");
            Console.WriteLine("Repeated enumeration can recompute work; materialize when reused.");
            Console.WriteLine("Use ToList/ToArray at API boundaries when snapshot behavior is required.");
            Console.WriteLine("Avoid heavy side effects inside Where/Select predicates.");
        }
    }
}
