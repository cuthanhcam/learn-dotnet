using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSharpBasics.Examples.Strings
{
    /// <summary>
    /// Comprehensive lesson on StringBuilder for efficient string building.
    ///
    /// StringBuilder is critical for performance when:
    /// - Building strings in loops
    /// - Concatenating many strings
    /// - Building dynamic queries/content
    /// - Number of concatenations unknown at compile-time
    ///
    /// Key topics:
    /// - String immutability problem
    /// - StringBuilder performance benefits
    /// - Append vs Concatenation patterns
    /// - StringBuilder capacity management
    /// - Common StringBuilder patterns
    ///
    /// String concatenation problem:
    /// Using + operator in loops creates:
    /// - Temp string for each concatenation
    /// - Garbage collection pressure
    /// - O(n²) complexity for n concatenations
    ///
    /// StringBuilder solution:
    /// - Single mutable buffer
    /// - Reusable capacity
    /// - O(n) complexity
    /// - Allocates only final string
    ///
    /// Best practices:
    /// - Use StringBuilder for dynamic content in loops
    /// - Use string interpolation for simple formatting
    /// - Initialize capacity if size is known
    /// - Use ToString() only when final string needed
    /// - StringBuilder methods return reference to same instance (fluent pattern)
    ///
    /// Performance characteristics:
    /// - StringBuilder: linear complexity O(n)
    /// - String concatenation (+): quadratic complexity O(n²)
    /// - Gap widens exponentially with iteration count
    /// - Break-even point: ~3-4 concatenations
    ///
    /// Real-world scenarios:
    /// - Building CSV/JSON content
    /// - Generating HTML dynamically
    /// - Building SQL queries (though parameterized better)
    /// - Log message aggregation
    /// </summary>
    public static class StringBuilderExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} StringBuilderExample {new string('=', 5)}");

            PrintSection("STRING CONCATENATION PROBLEM");
            DemoConcatenationProblem();

            PrintSection("STRINGBUILDER SOLUTION");
            DemoStringBuilder();

            PrintSection("PERFORMANCE COMPARISON");
            DemoPerformanceComparison();

            PrintSection("COMMON PATTERNS");
            DemoCommonPatterns();

            PrintSection("CAPACITY MANAGEMENT");
            DemoCapacityManagement();

            Console.WriteLine();
        }

        // =========================================================
        // PUBLIC METHODS
        // =========================================================

        /// <summary>
        /// Builds CSV text using naive concatenation.
        /// Educational anti-pattern to demonstrate allocation cost.
        /// Time complexity (cumulative copy cost): O(n^2).
        /// </summary>
        public static string BuildCsvLineNaive(string[] values)
        {
            if (values is null || values.Length == 0)
                return string.Empty;

            string result = string.Empty;
            foreach (string value in values)
            {
                result += value + ",";
            }

            return result.TrimEnd(',');
        }

        /// <summary>
        /// Builds CSV text using StringBuilder with pre-sized capacity.
        /// Demonstrates recommended approach for repeated appends.
        /// Time complexity: O(n).
        /// </summary>
        public static string BuildCsvLineOptimal(string[] values)
        {
            if (values is null || values.Length == 0)
                return string.Empty;

            StringBuilder sb = new(capacity: values.Sum(v => v.Length + 1));
            sb.AppendJoin(',', values);
            return sb.ToString();
        }

        /// <summary>
        /// Builds a platform-aware path using separator from Path API.
        /// </summary>
        public static string BuildPath(params string[] segments)
        {
            if (segments is null || segments.Length == 0)
                return string.Empty;

            StringBuilder sb = new(segments[0]);
            foreach (string segment in segments.Skip(1))
            {
                sb.Append(Path.DirectorySeparatorChar).Append(segment);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Builds a simple HTML table row.
        /// Note: in production web code, ensure values are HTML-encoded.
        /// </summary>
        public static string BuildTableRow(params string[] cells)
        {
            if (cells is null || cells.Length == 0)
                return "<tr></tr>";

            StringBuilder sb = new("<tr>");
            foreach (string cell in cells)
            {
                sb.Append("<td>").Append(cell).Append("</td>");
            }

            sb.Append("</tr>");
            return sb.ToString();
        }

        /// <summary>
        /// Repeats a word with optional separator.
        /// Example: RepeatWord("hi", 3, "-") -> "hi-hi-hi".
        /// </summary>
        public static string RepeatWord(string word, int count, string separator = "")
        {
            if (string.IsNullOrEmpty(word) || count <= 0)
                return string.Empty;

            StringBuilder sb = new(word.Length * count + separator.Length * (count - 1));
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sb.Append(separator);
                sb.Append(word);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Builds key-value lines from tuple sequence.
        /// </summary>
        public static string BuildKeyValueTable(IEnumerable<(string key, string value)> items)
        {
            if (items is null)
                return string.Empty;

            StringBuilder sb = new();
            foreach (var (key, value) in items)
            {
                sb.Append(key).Append(": ").Append(value).AppendLine();
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Repeats text count times.
        /// Time complexity: O(count * text.Length).
        /// </summary>
        public static string Repeat(string text, int count)
        {
            if (string.IsNullOrEmpty(text) || count <= 0)
                return string.Empty;

            StringBuilder sb = new(text.Length * count);
            for (int i = 0; i < count; i++)
                sb.Append(text);

            return sb.ToString();
        }

        /// <summary>
        /// Builds a human-readable debug block.
        /// Useful for logging diagnostics.
        /// </summary>
        public static string BuildDebugInfo(string objectName, Dictionary<string, object> properties)
        {
            objectName ??= "Object";
            if (properties is null || properties.Count == 0)
                return $"{objectName} {{ EMPTY }}";

            StringBuilder sb = new($"{objectName} {{{Environment.NewLine}");
            foreach (var (key, value) in properties)
            {
                sb.Append("  ").Append(key).Append(" = ").Append(value).AppendLine();
            }

            sb.Append('}');
            return sb.ToString();
        }

        // =========================================================
        // PRIVATE DEMO METHODS
        // =========================================================

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        private static void DemoConcatenationProblem()
        {
            Console.WriteLine("Using '+' in loops creates many temporary strings.");
            Console.WriteLine("This increases allocations and GC pressure.");
        }

        private static void DemoStringBuilder()
        {
            StringBuilder sb = new();
            sb.Append("Hello").Append(' ').Append("StringBuilder").Append('!');
            Console.WriteLine($"Result: {sb}");

            StringBuilder withCapacity = new(capacity: 64);
            withCapacity.Append("abc");
            Console.WriteLine($"Length/Capacity: {withCapacity.Length}/{withCapacity.Capacity}");
        }

        private static void DemoPerformanceComparison()
        {
            const int iterations = 1000;
            string[] values = Enumerable.Range(1, 100).Select(i => $"Value{i}").ToArray();

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iterations; i++) _ = BuildCsvLineNaive(values);
            sw.Stop();
            long naiveMs = sw.ElapsedMilliseconds;

            sw.Restart();
            for (int i = 0; i < iterations; i++) _ = BuildCsvLineOptimal(values);
            sw.Stop();
            long builderMs = sw.ElapsedMilliseconds;

            Console.WriteLine($"Naive: {naiveMs} ms");
            Console.WriteLine($"StringBuilder: {builderMs} ms");
        }

        private static void DemoCommonPatterns()
        {
            Console.WriteLine($"Path: {BuildPath(@"C:\", "Users", "workspace", "file.txt")}");
            Console.WriteLine($"HTML row: {BuildTableRow("Name", "Age", "City")}");
            Console.WriteLine($"RepeatWord: {RepeatWord("hi", 3, "-")}");

            var pairs = new List<(string key, string value)>
            {
                ("Name", "Product"),
                ("Price", "99.99")
            };
            Console.WriteLine(BuildKeyValueTable(pairs));
        }

        private static void DemoCapacityManagement()
        {
            StringBuilder defaultSb = new();
            StringBuilder tunedSb = new(1000);

            Console.WriteLine($"Default capacity: {defaultSb.Capacity}");
            Console.WriteLine($"Tuned capacity: {tunedSb.Capacity}");
            Console.WriteLine("Set capacity when expected output size is known.");
        }
    }
}
