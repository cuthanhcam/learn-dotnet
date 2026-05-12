namespace CoreDotNet.Examples.Collections
{
    /// <summary>
    /// Comprehensive examples for the core collection types in .NET.
    ///
    /// This lesson is intentionally practical and specific:
    /// - It contrasts read-only and mutable collection contracts.
    /// - It shows when to prefer List<T>, Dictionary<TKey, TValue>, HashSet<T>, Queue<T>, or Stack<T>.
    /// - It demonstrates indexing, lookup, uniqueness, ordering, and safe mutation.
    /// - It includes small, realistic examples that mirror service, catalog, and workflow scenarios.
    ///
    /// Best practices:
    /// - Use IReadOnlyCollection<T> for read-only parameters.
    /// - Use IEnumerable<T> for deferred or streaming results.
    /// - Use List<T> when you need indexing and frequent appends.
    /// - Use Dictionary<TKey, TValue> for fast key lookups.
    /// - Use HashSet<T> when uniqueness matters.
    /// - Avoid modifying collections while iterating over them.
    /// </summary>
    public static class CollectionsExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} Collections Examples {new string('=', 5)}");

            PrintSection("LIST INITIALIZATION & BASIC OPERATIONS");
            DemoListOperations();
        }

        private static void DemoListOperations()
        {
            // Initialization patterns
            var list = new List<string> { "apple", "banana", "cherry" };
            Console.WriteLine($"Initial list: {string.Join(", ", list)}");

            // Add items
            list.Add("date");
            Console.WriteLine($"After Add: {string.Join(", ", list)}");

            // Insert at position
            list.Insert(1, "blueberry");
            Console.WriteLine($"After Insert at 1: {string.Join(", ", list)}");

            // Remove and RemoveAll
            list.Remove("blueberry");
            Console.WriteLine($"After Remove: {string.Join(", ", list)}");

            // Count and access
            Console.WriteLine($"Count: {list.Count}, First: {list[0]}, Last: {list[list.Count - 1]}");

            // Snapshot a list before exposing it outside the method boundary
            var learningTopics = list.Select(item => item.ToUpperInvariant()).ToList();
            Console.WriteLine($"Upper-case snapshot: {string.Join(", ", learningTopics)}");
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }
}
