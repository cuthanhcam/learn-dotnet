using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpBasics.Examples.Strings
{
    /// <summary>
    /// Comprehensive lesson for practical string APIs.
    ///
    /// This covers the most commonly used string operations in real applications.
    /// String methods are the foundation for text processing, parsing, and validation.
    ///
    /// Key topics:
    /// - Tokenization and splitting
    /// - Case-insensitive operations
    /// - String searching and containment checks
    /// - Prefix/suffix operations
    /// - String replacement strategies
    /// - Index-based searching (IndexOf, LastIndexOf)
    /// - Substring extraction and slicing
    /// - Character-level operations
    ///
    /// Common patterns in production code:
    /// - Always use StringComparison parameters for case-insensitive
    /// - Use OrdinalIgnoreCase for case-insensitive comparison
    /// - Use CurrentCultureIgnoreCase ONLY when culture-specific matching required
    /// - Prefer string slicing (..) over Substring() for modern C#
    /// - Use Contains with StringComparison instead of .ToLower().Contains
    ///
    /// Performance notes:
    /// - String.Contains is O(n) search
    /// - String.IndexOf can use efficient search algorithms
    /// - String.Replace creates new string (use PadLeft/PadRight for padding)
    /// - String slicing efficient when indices known
    /// - Avoid case conversion in loops; use StringComparison param instead
    ///
    /// Best practices:
    /// - Always specify StringComparison parameter
    /// - Use null-safe pattern: input?.Contains(...) ?? false
    /// - Validate input before processing
    /// - Consider regex only for complex patterns
    /// - String methods faster than regex for simple operations
    /// </summary>
    public static class StringMethodsExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} StringMethodsExample {new string('=', 5)}");

            PrintSection("TOKENIZATION");
            DemoTokenization();

            PrintSection("SEARCHING & CONTAINMENT");
            DemoSearching();

            PrintSection("REPLACEMENT");
            DemoReplacement();

            PrintSection("PREFIX & SUFFIX");
            DemoPrefixSuffix();

            PrintSection("INDEXING");
            DemoIndexing();

            PrintSection("EXTRACTION");
            DemoExtraction();

            PrintSection("PERFORMANCE NOTES");
            DemoPerformanceNotes();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Splits words ignoring whitespace variations.
        /// Returns non-empty, trimmed words.
        /// Time complexity: O(n), where n is input length.
        /// </summary>
        public static string[] SplitWords(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return [];

            return input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        /// <summary>
        /// Joins multiple words with separator.
        /// Inverse of SplitWords.
        /// Materializes filtered sequence immediately via string.Join.
        /// </summary>
        public static string JoinWords(IEnumerable<string> words, string separator)
        {
            ArgumentNullException.ThrowIfNull(words);
            separator ??= string.Empty;

            return string.Join(separator, words.Where(word => !string.IsNullOrEmpty(word)));
        }

        /// <summary>
        /// Case-insensitive containment check.
        /// Always prefer this over .ToLower().Contains().
        /// </summary>
        public static bool ContainsIgnoreCase(string input, string keyword)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(keyword))
                return false;

            return input.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Replaces all occurrences case-insensitively.
        /// Safer than .Replace with case conversion.
        /// Returns original input when replacement cannot be applied.
        /// </summary>
        public static string ReplaceWord(string input, string oldValue, string newValue)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(oldValue))
                return input;

            return input.Replace(oldValue, newValue, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Counts whitespace-separated tokens.
        /// Time complexity: O(n).
        /// </summary>
        public static int CountTokens(string input)
            => SplitWords(input).Length;

        /// <summary>
        /// Case-insensitive prefix check.
        /// </summary>
        public static bool StartsWithIgnoreCase(string input, string prefix)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(prefix))
                return false;

            return input.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Case-insensitive suffix check.
        /// </summary>
        public static bool EndsWithIgnoreCase(string input, string suffix)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(suffix))
                return false;

            return input.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Finds index of substring case-insensitively.
        /// Returns -1 if not found.
        /// </summary>
        public static int FindIndexIgnoreCase(string input, string substring)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(substring))
                return -1;

            return input.IndexOf(substring, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Finds last occurrence index case-insensitively.
        /// Returns -1 if not found.
        /// </summary>
        public static int FindLastIndexIgnoreCase(string input, string substring)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(substring))
                return -1;

            return input.LastIndexOf(substring, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Extracts substring between start and end indices using slice notation.
        /// Modern .NET approach (more efficient than Substring).
        /// </summary>
        public static string ExtractSlice(string input, int start, int end)
        {
            if (string.IsNullOrEmpty(input) || start < 0 || end > input.Length || start > end)
                return string.Empty;

            return input[start..end];
        }

        /// <summary>
        /// Try-pattern extraction between two markers.
        /// Useful for parsing lightweight templated text.
        /// </summary>
        public static bool TryExtractBetween(string input, string startMarker, string endMarker, out string value)
        {
            value = string.Empty;

            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(startMarker) || string.IsNullOrEmpty(endMarker))
                return false;

            int start = input.IndexOf(startMarker, StringComparison.Ordinal);
            if (start < 0)
                return false;

            start += startMarker.Length;
            int end = input.IndexOf(endMarker, start, StringComparison.Ordinal);
            if (end < 0 || end < start)
                return false;

            value = input[start..end];
            return true;
        }

        /// <summary>
        /// Pads string to length with character on left.
        /// Useful for formatting numbers, IDs, etc.
        /// </summary>
        public static string PadLeft(string input, int totalWidth, char paddingChar = ' ')
        {
            if (string.IsNullOrEmpty(input))
                return new string(paddingChar, totalWidth);

            return input.PadLeft(totalWidth, paddingChar);
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates tokenization patterns.
        /// </summary>
        private static void DemoTokenization()
        {
            string[] testCases = ["  hello   world  ", "csharp fundamentals module", "  single  ", "   "];

            Console.WriteLine("Splitting with empty entry removal:");
            foreach (string test in testCases)
            {
                string[] tokens = SplitWords(test);
                string result = tokens.Length == 0 ? "(empty)" : string.Join(" | ", tokens);
                Console.WriteLine($"  '{test}' -> {result}");
            }

            Console.WriteLine();
            Console.WriteLine("Joining words:");
            string[] words = ["Learn", "C#", "step", "by", "step"];
            Console.WriteLine($"  {JoinWords(words, " ")}");
            Console.WriteLine($"  {JoinWords(words, "-")}");
            Console.WriteLine($"  {JoinWords(words, " -> ")}");
        }

        /// <summary>
        /// Demonstrates searching operations.
        /// </summary>
        private static void DemoSearching()
        {
            string text = "The quick brown fox jumps over the lazy dog";

            Console.WriteLine("Case-insensitive containment:");
            Console.WriteLine($"  Contains 'QUICK': {ContainsIgnoreCase(text, "QUICK")}");
            Console.WriteLine($"  Contains 'FOX': {ContainsIgnoreCase(text, "FOX")}");
            Console.WriteLine($"  Contains 'cat': {ContainsIgnoreCase(text, "cat")}");

            Console.WriteLine();
            Console.WriteLine("Finding indices:");
            Console.WriteLine($"  Index of 'brown': {FindIndexIgnoreCase(text, "brown")}");
            Console.WriteLine($"  Index of 'THE': {FindIndexIgnoreCase(text, "THE")}");
            Console.WriteLine($"  Last index of 'THE': {FindLastIndexIgnoreCase(text, "THE")}");
            Console.WriteLine($"  Index of (not found): {FindIndexIgnoreCase(text, "xyz")}");
        }

        /// <summary>
        /// Demonstrates replacement patterns.
        /// </summary>
        private static void DemoReplacement()
        {
            string text = "The Quick Fox jumped over the quick dog";

            Console.WriteLine("Case-insensitive replacement:");
            Console.WriteLine($"  Original: {text}");

            string replaced = ReplaceWord(text, "quick", "SLOW");
            Console.WriteLine($"  After replace: {replaced}");

            Console.WriteLine();
            Console.WriteLine("Token counting:");
            Console.WriteLine($"  Tokens: {CountTokens(text)}");
            Console.WriteLine($"  Empty string tokens: {CountTokens("   ")}");
        }

        /// <summary>
        /// Demonstrates prefix/suffix checking.
        /// </summary>
        private static void DemoPrefixSuffix()
        {
            string url = "https://example.com";
            string email = "user@example.com";

            Console.WriteLine("Prefix checking (case-insensitive):");
            Console.WriteLine($"  '{url}' starts with 'HTTPS': {StartsWithIgnoreCase(url, "HTTPS")}");
            Console.WriteLine($"  '{url}' starts with 'ftp': {StartsWithIgnoreCase(url, "ftp")}");

            Console.WriteLine();
            Console.WriteLine("Suffix checking (case-insensitive):");
            Console.WriteLine($"  '{email}' ends with '.COM': {EndsWithIgnoreCase(email, ".COM")}");
            Console.WriteLine($"  '{email}' ends with '.org': {EndsWithIgnoreCase(email, ".org")}");

            Console.WriteLine();
            Console.WriteLine("Real-world examples:");
            string[] fileNames = ["document.PDF", "image.PNG", "script.js", "data.CSV"];
            foreach (string fileName in fileNames)
            {
                bool isPdf = EndsWithIgnoreCase(fileName, ".pdf");
                bool isImage = EndsWithIgnoreCase(fileName, ".png") || EndsWithIgnoreCase(fileName, ".jpg");
                Console.WriteLine($"  {fileName}: PDF={isPdf}, Image={isImage}");
            }
        }

        /// <summary>
        /// Demonstrates index-based operations.
        /// </summary>
        private static void DemoIndexing()
        {
            string csv = "Name,Age,City";

            Console.WriteLine("Finding field positions:");
            int ageComma = FindIndexIgnoreCase(csv, ",Age,");
            Console.WriteLine($"  CSV: {csv}");
            Console.WriteLine($"  Position of Age field: {(ageComma >= 0 ? ageComma : "not found")}");

            Console.WriteLine();
            Console.WriteLine("Handling multiple occurrences:");
            string text = "apple apple apple";
            int first = FindIndexIgnoreCase(text, "apple");
            int last = FindLastIndexIgnoreCase(text, "apple");
            Console.WriteLine($"  Text: {text}");
            Console.WriteLine($"  First 'apple' at: {first}");
            Console.WriteLine($"  Last 'apple' at: {last}");
        }

        /// <summary>
        /// Demonstrates extraction and padding.
        /// </summary>
        private static void DemoExtraction()
        {
            string text = "C# Programming Fundamentals";

            Console.WriteLine("String slicing (modern approach):");
            Console.WriteLine($"  Original: {text}");
            Console.WriteLine($"  [0..1]: {ExtractSlice(text, 0, 1)}");
            Console.WriteLine($"  [0..10]: {ExtractSlice(text, 0, 10)}");
            Console.WriteLine($"  [3..13]: {ExtractSlice(text, 3, 13)}");

            Console.WriteLine();
            Console.WriteLine("TryExtractBetween markers:");
            string template = "Order[id=SO-1001,status=Paid]";
            if (TryExtractBetween(template, "id=", ",", out string orderId))
            {
                Console.WriteLine($"  Template: {template}");
                Console.WriteLine($"  Extracted id: {orderId}");
            }

            Console.WriteLine();
            Console.WriteLine("Padding for formatting:");
            Console.WriteLine("ID".PadRight(10) + "Name".PadRight(15) + "Score");
            Console.WriteLine("-".PadRight(40, '-'));
            Console.WriteLine("001".PadRight(10) + "Alice".PadRight(15) + "95");
            Console.WriteLine("002".PadRight(10) + "Bob".PadRight(15) + "87");
            Console.WriteLine("003".PadRight(10) + "Charlie".PadRight(15) + "92");

            Console.WriteLine();
            Console.WriteLine("Zero-padding numbers:");
            int[] ids = [1, 42, 999];
            foreach (int id in ids)
            {
                Console.WriteLine($"  ID: {PadLeft(id.ToString(), 4, '0')}");
            }
        }

        /// <summary>
        /// Demonstrates performance notes and best practices.
        /// </summary>
        private static void DemoPerformanceNotes()
        {
            Console.WriteLine("Use Contains/IndexOf with StringComparison instead of ToLower().");
            Console.WriteLine("Use try-pattern methods for parsing untrusted text.");
            Console.WriteLine("Prefer string methods over regex for simple operations.");
        }
    }
}
