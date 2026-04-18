namespace CSharpBasics.Examples.Collections
{
    /// <summary>
    /// Comprehensive lesson for Dictionary usage: counting, lookup, update, and grouping.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Dictionary<K, V> characteristics:
    /// - Key-value pair storage
    /// - Fast O(1) average lookup
    /// - Requires proper equality comparison for keys
    /// - Keys must be unique
    /// - Case sensitivity depends on key type
    /// 
    /// Key topics:
    /// - Word counting (frequency analysis)
    /// - Safe lookup with TryGetValue
    /// - Upsert pattern (insert or update)
    /// - Dictionary initialization
    /// - String comparison options (case-sensitive/insensitive)
    /// - Grouping and categorization
    /// 
    /// When to use Dictionary:
    /// - Fast key-based lookups
    /// - Counting/frequency analysis
    /// - Caching
    /// - Mapping relationships
    /// - Case-insensitive string matching
    /// 
    /// When NOT to use Dictionary:
    /// - Ordered traversal required → use SortedDictionary
    /// - All key-value pairs with same frequency → use List<T>
    /// - Need to preserve insertion order → use LinkedDictionary (custom)
    /// 
    /// Best practices:
    /// - Use TryGetValue for safe lookups (not direct indexing)
    /// - Specify StringComparer for case-insensitive lookups
    /// - Initialize with capacity when size is known (performance)
    /// - Use ContainsKey rarely; prefer TryGetValue
    /// - Document the null-handling strategy for values
    /// </summary>
    public static class DictionaryExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} DictionaryExample {new string('=', 5)}");

            PrintSection("WORD FREQUENCY COUNTING");
            DemoWordCount();

            PrintSection("LOOKUP WITH TRYGETVALUE");
            DemoLookup();

            PrintSection("UPSERT PATTERN");
            DemoUpsert();

            PrintSection("GROUPING & CATEGORIZATION");
            DemoGrouping();

            PrintSection("CASE-INSENSITIVE KEYS");
            DemoCaseInsensitivity();

            PrintSection("MERGING COUNTS");
            DemoMerging();

            PrintSection("PERFORMANCE NOTES");
            DemoPerformanceNotes();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Counts word occurrences in a sentence.
        /// Uses case-insensitive comparison and skips empty tokens.
        /// Time complexity: O(n) over token count, average O(1) update per token.
        /// </summary>
        public static Dictionary<string, int> BuildWordCount(string sentence)
        {
            if (string.IsNullOrWhiteSpace(sentence))
                return [];

            Dictionary<string, int> counts = new(StringComparer.OrdinalIgnoreCase);

            string[] words = sentence.Split(
                [' ', ',', '.', ';', ':', '!', '?'],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (string word in words)
            {
                counts[word] = counts.TryGetValue(word, out int count) ? count + 1 : 1;
            }

            return counts;
        }

        /// <summary>
        /// Looks up a capital city by country code.
        /// Returns null if not found.
        /// Uses TryGetValue to avoid KeyNotFoundException.
        /// </summary>
        public static string? TryGetCapital(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                return null;

            Dictionary<string, string> capitals = new(StringComparer.OrdinalIgnoreCase)
            {
                ["VN"] = "Hanoi",
                ["JP"] = "Tokyo",
                ["FR"] = "Paris",
                ["US"] = "Washington DC",
                ["AU"] = "Canberra"
            };

            return capitals.TryGetValue(countryCode, out string? capital) ? capital : null;
        }

        /// <summary>
        /// Increases (or initializes) a value in dictionary.
        /// This is the classic upsert pattern.
        /// </summary>
        public static void IncreaseValue(Dictionary<string, int> dictionary, string key, int amount)
        {
            ArgumentNullException.ThrowIfNull(dictionary);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            dictionary[key] = dictionary.TryGetValue(key, out int existing) ? existing + amount : amount;
        }

        /// <summary>
        /// Merges source counts into target using upsert.
        /// Useful for combining partial results.
        /// Time complexity: O(m), m = number of entries in source.
        /// </summary>
        public static void MergeCounts(Dictionary<string, int> target, IReadOnlyDictionary<string, int> source)
        {
            ArgumentNullException.ThrowIfNull(target);
            ArgumentNullException.ThrowIfNull(source);

            foreach (var (key, count) in source)
            {
                target[key] = target.TryGetValue(key, out int existing) ? existing + count : count;
            }
        }

        /// <summary>
        /// Groups items by first letter.
        /// Keeps insertion order inside each letter bucket.
        /// </summary>
        public static Dictionary<char, List<string>> GroupByFirstLetter(IEnumerable<string> values)
        {
            ArgumentNullException.ThrowIfNull(values);

            Dictionary<char, List<string>> result = [];

            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                string trimmed = value.Trim();
                char key = char.ToUpperInvariant(trimmed[0]);

                if (!result.TryGetValue(key, out List<string>? bucket))
                {
                    bucket = [];
                    result[key] = bucket;
                }

                bucket.Add(trimmed);
            }

            return result;
        }

        /// <summary>
        /// Counts character occurrences in a string.
        /// Can ignore case when requested.
        /// Time complexity: O(n), n = input length.
        /// </summary>
        public static Dictionary<char, int> CountCharacters(string input, bool ignoreCase = false)
        {
            Dictionary<char, int> counts = [];

            if (string.IsNullOrEmpty(input))
                return counts;

            foreach (char c in input)
            {
                char key = ignoreCase ? char.ToUpperInvariant(c) : c;
                counts[key] = counts.TryGetValue(key, out int count) ? count + 1 : 1;
            }

            return counts;
        }

        /// <summary>
        /// Finds most frequently occurring item.
        /// Returns null for empty input.
        /// </summary>
        public static (string Item, int Count)? FindMostFrequent(Dictionary<string, int> counts)
        {
            if (counts is null || counts.Count == 0)
                return null;

            var mostFrequent = counts.MaxBy(kvp => kvp.Value);
            return (mostFrequent.Key, mostFrequent.Value);
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        private static void DemoWordCount()
        {
            string text = "hello, world! hello csharp world dictionary world";
            Dictionary<string, int> wordCount = BuildWordCount(text);

            Console.WriteLine($"Text: \"{text}\"");
            Console.WriteLine($"Word count entries: {wordCount.Count}");

            foreach (var (word, count) in wordCount.OrderByDescending(kv => kv.Value))
            {
                Console.WriteLine($"  '{word}' appears {count} time(s)");
            }

            var (mostCommon, frequency) = FindMostFrequent(wordCount) ?? ("N/A", 0);
            Console.WriteLine($"Most frequent: '{mostCommon}' ({frequency} times)");
        }

        private static void DemoLookup()
        {
            Console.WriteLine("Capital lookups (case-insensitive):");
            string[] codes = ["VN", "vn", "JP", "jp", "XX", "US", ""];

            foreach (string code in codes)
            {
                string? capital = TryGetCapital(code);
                Console.WriteLine(capital is not null
                    ? $"  {code} → {capital}"
                    : $"  {code} → N/A (not found)");
            }
        }

        private static void DemoUpsert()
        {
            Console.WriteLine("Inventory management:");
            Dictionary<string, int> inventory = new(StringComparer.OrdinalIgnoreCase)
            {
                ["Pen"] = 10,
                ["Book"] = 5,
                ["Notebook"] = 3
            };

            Console.WriteLine("Initial inventory:");
            PrintInventory(inventory);

            // Existing key gets incremented.
            IncreaseValue(inventory, "Pen", 2);
            // New key gets initialized with amount.
            IncreaseValue(inventory, "Pencil", 5);

            Console.WriteLine("After updates:");
            PrintInventory(inventory);
        }

        private static void DemoGrouping()
        {
            Console.WriteLine("Grouping fruits by first letter:");
            string[] fruits = ["apple", "apricot", "banana", "blueberry", "cherry", "cranberry"];

            Dictionary<char, List<string>> grouped = GroupByFirstLetter(fruits);

            foreach ((char letter, List<string> items) in grouped.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"  {letter}: {string.Join(", ", items)}");
            }
        }

        private static void DemoCaseInsensitivity()
        {
            string text = "AaBbA";
            Dictionary<char, int> caseSensitive = CountCharacters(text);
            Dictionary<char, int> caseInsensitive = CountCharacters(text, ignoreCase: true);

            Console.WriteLine($"Input: {text}");
            Console.WriteLine("Case-sensitive count:");
            foreach (var (c, count) in caseSensitive.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"  '{c}' => {count}");
            }

            Console.WriteLine("Case-insensitive count:");
            foreach (var (c, count) in caseInsensitive.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"  '{c}' => {count}");
            }
        }

        private static void DemoMerging()
        {
            Dictionary<string, int> sourceA = BuildWordCount("api api dotnet");
            Dictionary<string, int> sourceB = BuildWordCount("dotnet csharp csharp");

            MergeCounts(sourceA, sourceB);

            Console.WriteLine("Merged word counts:");
            foreach (var (key, value) in sourceA.OrderByDescending(k => k.Value))
            {
                Console.WriteLine($"  {key}: {value}");
            }
        }

        private static void DemoPerformanceNotes()
        {
            Console.WriteLine("Dictionary lookup is O(1) average, but hashing quality matters.");
            Console.WriteLine("Prefer TryGetValue over ContainsKey + indexer (single lookup).");
            Console.WriteLine("Use StringComparer.OrdinalIgnoreCase for robust text-key lookups.");
            Console.WriteLine("Initialize with expected capacity for large dictionaries.");
        }

        private static void PrintInventory(Dictionary<string, int> inventory)
        {
            foreach (var (item, quantity) in inventory.OrderBy(kv => kv.Key))
            {
                Console.WriteLine($"  {item}: {quantity}");
            }
        }
    }
}
