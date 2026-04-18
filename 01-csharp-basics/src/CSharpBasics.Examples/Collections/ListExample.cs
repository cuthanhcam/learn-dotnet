namespace CSharpBasics.Examples.Collections
{
    /// <summary>
    /// Comprehensive lesson for List<T> operations and mutation patterns.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// List<T> vs Array:
    /// - List<T>: Dynamic size, mutable, flexible
    /// - Array: Fixed size, best for performance-critical code
    /// 
    /// Key topics:
    /// - List creation and initialization
    /// - Filtering and querying with LINQ
    /// - Mutation (Add, Remove, RemoveAll, Insert)
    /// - Aggregation (Count, Sum, Average)
    /// - Enumerating and preserving order
    /// 
    /// Best practices:
    /// - Use IReadOnlyList<T> for parameters when you don't need to modify
    /// - Use IEnumerable<T> for methods returning results that might be lazily evaluated
    /// - Use List<T> when you need full control and mutation capability
    /// - Avoid modifying a list while iterating (use RemoveAll or ToList first)
    /// </summary>
    public static class ListExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} ListExample {new string('=', 5)}");

            PrintSection("INITIALIZATION & CREATION");
            DemoInitialization();

            PrintSection("FILTERING WITH LINQ");
            DemoFiltering();

            PrintSection("MUTATION PATTERNS");
            DemoMutation();

            PrintSection("SAFE ACCESS & VALIDATION");
            DemoSafeAccess();

            PrintSection("AGGREGATION");
            DemoAggregation();

            PrintSection("INSERT, REMOVE & RANGE OPERATIONS");
            DemoRangeOperations();

            PrintSection("SEARCHING IN LIST");
            DemoSearching();

            PrintSection("PERFORMANCE NOTES");
            DemoPerformanceNotes();

            Console.WriteLine();
        }

        // PUBLIC METHODS (Learning examples)

        /// <summary>
        /// Creates a new list from initial values.
        /// Demonstrates collection initializer syntax.
        /// </summary>
        public static List<string> CreateColorList()
        {
            List<string> colors =
            [
                "Red",
                "Green",
                "Blue",
                "Yellow"
            ];

            return colors;
        }

        /// <summary>
        /// Filters ages to find adults (18+).
        /// Returns a new List<T> (materialized), not a lazy IEnumerable<T>.
        /// Time complexity: O(n), space: O(n) for output.
        /// </summary>
        public static List<int> FilterAdults(IEnumerable<int> ages)
        {
            ArgumentNullException.ThrowIfNull(ages);
            return ages.Where(age => age >= 18).ToList();
        }

        /// <summary>
        /// Removes negative numbers from a list in-place.
        /// Demonstrates mutation with RemoveAll predicate.
        /// Modifies the original list.
        /// Time complexity: O(n).
        /// </summary>
        public static void RemoveNegativeNumbers(List<int> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);
            numbers.RemoveAll(number => number < 0);
        }

        /// <summary>
        /// Calculates average of a collection safely.
        /// Returns 0 instead of throwing for empty collection.
        /// Time complexity: O(n).
        /// </summary>
        public static double CalculateAverage(IReadOnlyCollection<int> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);

            if (numbers.Count == 0)
                return 0;

            return numbers.Average();
        }

        /// <summary>
        /// Inserts an item at the beginning of the list.
        /// O(n) complexity due to element shifting.
        /// </summary>
        public static void InsertAtStart(List<int> values, int item)
        {
            ArgumentNullException.ThrowIfNull(values);
            values.Insert(0, item);
        }

        /// <summary>
        /// Returns the first N items from a list.
        /// Returns empty list if count is invalid.
        /// Materializes the result immediately.
        /// </summary>
        public static List<int> TakeFirst(List<int> values, int count)
        {
            ArgumentNullException.ThrowIfNull(values);

            if (count <= 0)
                return [];

            return values.Take(count).ToList();
        }

        /// <summary>
        /// Groups items by a predicate and counts occurrences.
        /// Useful for categorizing data.
        /// Time complexity: O(n), average dictionary operations O(1).
        /// </summary>
        public static Dictionary<string, int> GroupByCategory(IEnumerable<int> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            Dictionary<string, int> categories = new();
            foreach (int value in values)
            {
                string category = value switch
                {
                    < 0 => "Negative",
                    0 => "Zero",
                    <= 10 => "Small",
                    <= 100 => "Medium",
                    _ => "Large"
                };

                if (categories.ContainsKey(category))
                    categories[category]++;
                else
                    categories[category] = 1;
            }

            return categories;
        }

        /// <summary>
        /// Safely reads a value from list by index without throwing.
        /// Useful when index comes from user input or external data.
        /// </summary>
        public static bool TryGetAt<T>(IReadOnlyList<T> values, int index, out T? value)
        {
            ArgumentNullException.ThrowIfNull(values);

            if (index < 0 || index >= values.Count)
            {
                value = default;
                return false;
            }

            value = values[index];
            return true;
        }

        /// <summary>
        /// Searches a sorted list using binary search.
        /// Returns index if found; otherwise -1.
        /// Time complexity: O(log n).
        /// </summary>
        public static int BinarySearchSorted(List<int> sortedValues, int target)
        {
            ArgumentNullException.ThrowIfNull(sortedValues);

            if (!IsSortedAscending(sortedValues))
            {
                throw new ArgumentException("List must be sorted in ascending order.", nameof(sortedValues));
            }

            int left = 0;
            int right = sortedValues.Count - 1;

            while (left <= right)
            {
                int middle = left + ((right - left) / 2);
                int value = sortedValues[middle];

                if (value == target)
                {
                    return middle;
                }

                if (value < target)
                {
                    left = middle + 1;
                }
                else
                {
                    right = middle - 1;
                }
            }

            return -1;
        }

        public static bool IsSortedAscending(IReadOnlyList<int> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] < values[i - 1])
                {
                    return false;
                }
            }

            return true;
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates multiple ways to initialize a List<T>.
        /// </summary>
        private static void DemoInitialization()
        {
            // Empty list
            List<int> empty = [];
            Console.WriteLine($"Empty list count: {empty.Count}");

            // Initialized with collection initializer
            List<string> colors = CreateColorList();
            Console.WriteLine($"Colors: {string.Join(", ", colors)}");

            // Created from existing collection (materializes IEnumerable)
            int[] numbers = [1, 2, 3, 4, 5];
            List<int> converted = numbers.ToList();
            Console.WriteLine($"From array: {string.Join(", ", converted)}");

            // Capacity planning helps reduce internal reallocation and copying.
            List<int> withCapacity = new(capacity: 10);
            withCapacity.AddRange([1, 2, 3]);
            Console.WriteLine($"Count/Capacity: {withCapacity.Count}/{withCapacity.Capacity}");
        }

        /// <summary>
        /// Demonstrates filtering with LINQ Where clause.
        /// </summary>
        private static void DemoFiltering()
        {
            int[] ages = [12, 18, 24, 15, 30, -1];
            List<int> adults = FilterAdults(ages);
            Console.WriteLine($"Adults (18+): {string.Join(", ", adults)}");

            // Filter and materialize in one step
            var evenNumbers = ages.Where(x => x > 0 && x % 2 == 0).ToList();
            Console.WriteLine($"Even positive ages: {string.Join(", ", evenNumbers)}");
        }

        /// <summary>
        /// Demonstrates in-place mutations on a list.
        /// </summary>
        private static void DemoMutation()
        {
            List<int> numbers = [5, -3, 10, -1, 20, 0, 15];
            Console.WriteLine($"Before removing negatives: {string.Join(", ", numbers)}");

            RemoveNegativeNumbers(numbers);
            Console.WriteLine($"After removing negatives: {string.Join(", ", numbers)}");

            // Append item at tail: usually O(1) amortized.
            numbers.Add(25);
            Console.WriteLine($"After adding 25: {string.Join(", ", numbers)}");

            // Add many at once for clearer intent.
            numbers.AddRange([30, 35]);
            Console.WriteLine($"After adding [30, 35]: {string.Join(", ", numbers)}");

            // Contains check
            Console.WriteLine($"Contains 20: {numbers.Contains(20)}");
        }

        /// <summary>
        /// Demonstrates safe access pattern for unknown indexes.
        /// </summary>
        private static void DemoSafeAccess()
        {
            List<string> students = ["Cam", "Linh", "An"];

            if (TryGetAt(students, 1, out string? secondStudent))
            {
                Console.WriteLine($"Index 1 => {secondStudent}");
            }

            bool hasIndex10 = TryGetAt(students, 10, out string? value);
            Console.WriteLine(hasIndex10
                ? $"Index 10 => {value}"
                : "Index 10 is out of range (handled safely)");
        }

        /// <summary>
        /// Demonstrates aggregation operations.
        /// </summary>
        private static void DemoAggregation()
        {
            List<int> scores = [85, 92, 78, 95, 88];
            Console.WriteLine($"Scores: {string.Join(", ", scores)}");
            Console.WriteLine($"Count: {scores.Count}");
            Console.WriteLine($"Sum: {scores.Sum()}");
            Console.WriteLine($"Average: {CalculateAverage(scores):0.00}");
            Console.WriteLine($"Max: {scores.Max()}");
            Console.WriteLine($"Min: {scores.Min()}");
        }

        /// <summary>
        /// Demonstrates insertion, removal, and range operations.
        /// </summary>
        private static void DemoRangeOperations()
        {
            List<int> items = [10, 20, 30, 40, 50];
            Console.WriteLine($"Initial: {string.Join(", ", items)}");

            // Insert at start
            InsertAtStart(items, 5);
            Console.WriteLine($"After inserting 5 at start: {string.Join(", ", items)}");

            // Insert at specific index
            items.Insert(3, 25);
            Console.WriteLine($"After inserting 25 at index 3: {string.Join(", ", items)}");

            // Remove at index
            items.RemoveAt(0);
            Console.WriteLine($"After removing first item: {string.Join(", ", items)}");

            // Take first N
            var top3 = TakeFirst(items, 3);
            Console.WriteLine($"Top 3 items: {string.Join(", ", top3)}");

            // Display grouped categorization
            var grouped = GroupByCategory(items);
            Console.WriteLine("Items grouped by size category:");
            foreach (var (category, count) in grouped.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"  {category}: {count} items");
            }
        }

        /// <summary>
        /// Demonstrates linear vs binary search choices for List<T>.
        /// </summary>
        private static void DemoSearching()
        {
            List<int> sorted = [10, 20, 30, 40, 50, 60, 70];
            int target = 50;

            int linearIndex = sorted.FindIndex(x => x == target);
            int binaryIndex = BinarySearchSorted(sorted, target);
            int frameworkBinaryIndex = sorted.BinarySearch(target);

            Console.WriteLine($"FindIndex (linear, O(n)): {linearIndex}");
            Console.WriteLine($"Custom binary search (O(log n)): {binaryIndex}");
            Console.WriteLine($"List.BinarySearch (O(log n)): {frameworkBinaryIndex}");
        }

        private static void DemoPerformanceNotes()
        {
            Console.WriteLine("List<T> grows dynamically; occasional resize copies data.");
            Console.WriteLine("Set capacity when expected size is known to reduce allocations.");
            Console.WriteLine("Insert/Remove near beginning is O(n) due to shifting.");
            Console.WriteLine("Prefer HashSet<T> for fast existence checks in large datasets.");
            Console.WriteLine("Use ReadOnlyCollection/IReadOnlyList to protect API boundaries.");
        }
    }
}
