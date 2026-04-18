namespace CSharpBasics.Examples.Strings
{
    /// <summary>
    /// Comprehensive lesson for core string operations.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// String characteristics:
    /// - Immutable in .NET
    /// - Reference type
    /// - Unicode support
    /// - Verbatim strings (@) for escapes
    /// - Interpolation ($) for formatting
    /// 
    /// Key topics:
    /// - String interpolation (most modern approach)
    /// - Formatting and escaping
    /// - Normalization (trim, case, spacing)
    /// - Case-insensitive operations
    /// - Comparison and equality
    /// - Performance implications
    /// 
    /// String immutability:
    /// - Most transformation operations return a new string instance
    /// - Use StringBuilder for heavy concatenation
    /// - Avoid concatenation in loops
    /// 
    /// Best practices:
    /// - Use string interpolation whenever possible
    /// - Use string.IsNullOrWhiteSpace for validation
    /// - Use string.Equals with StringComparison param for case-insensitive
    /// - Use StringBuilder for heavy manipulation
    /// - Use Trim/TrimStart/TrimEnd appropriately
    /// - Normalize case before comparisons
    /// 
    /// String comparison modes:
    /// - Ordinal: code-unit comparison (fastest, culture-independent)
    /// - OrdinalIgnoreCase: code-unit comparison, case-insensitive
    /// - CurrentCulture: culture-aware (slower)
    /// - InvariantCulture: culture-independent (slower than Ordinal)
    /// </summary>
    public static class StringBasicsExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} StringBasicsExample {new string('=', 5)}");

            PrintSection("STRING INTERPOLATION");
            DemoInterpolation();

            PrintSection("FORMATTING & ESCAPING");
            DemoFormattingEscaping();

            PrintSection("NORMALIZATION");
            DemoNormalization();

            PrintSection("CASE OPERATIONS");
            DemoCaseOperations();

            PrintSection("COMPARISON");
            DemoComparison();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Builds a profile line with name and age.
        /// Uses string interpolation (modern, recommended).
        /// </summary>
        public static string BuildProfileLine(string name, int age) => $"Name: {name}, Age: {age}";

        /// <summary>
        /// Builds an escaped file path.
        /// Demonstrates string escaping and path construction.
        /// </summary>
        public static string BuildEscapedPath(string root, string fileName) => $"{root}\\{fileName}";

        /// <summary>
        /// Builds escaped path using verbatim string (@).
        /// Alternative approach: verbatim raw string.
        /// </summary>
        public static string BuildPathVerbatim(string root, string fileName) => $@"{root}\{fileName}";

        /// <summary>
        /// Normalizes a name (trim, lowercase, capitalize words).
        /// Demonstrates multi-step string manipulation.
        /// Time complexity: O(n), where n is input length.
        /// </summary>
        public static string NormalizeName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                return string.Empty;

            string trimmed = rawName.Trim().ToLowerInvariant();
            return string.Join(' ', trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
        }

        /// <summary>
        /// Extracts initials from full name.
        /// Example: "John Doe Smith" → "JDS"
        /// Time complexity: O(k), where k is number of words.
        /// </summary>
        public static string BuildInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            string[] parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            // Guard: handle single-character words
            return string.Concat(parts
                .Where(part => part.Length > 0)
                .Select(part => char.ToUpperInvariant(part[0])));
        }

        /// <summary>
        /// Compares two strings case-insensitively.
        /// Always use OrdinalIgnoreCase for predictable comparisons.
        /// </summary>
        public static bool AreEqualIgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Validates if string looks like an email.
        /// Simple heuristic check (not regex-based).
        /// </summary>
        public static bool LooksLikeEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value.Contains('@') && value.Contains('.') && value.IndexOf('@') < value.LastIndexOf('.');
        }

        /// <summary>
        /// Truncates string to max length with ellipsis.
        /// Useful for display names, titles, etc.
        /// Preserves original text when length is already within limit.
        /// </summary>
        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (maxLength < 3)
                throw new ArgumentOutOfRangeException(nameof(maxLength), "Must be at least 3 to accommodate ellipsis.");

            if (value.Length <= maxLength)
                return value;

            return value[..(maxLength - 3)] + "...";
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates string interpolation benefits.
        /// </summary>
        private static void DemoInterpolation()
        {
            string name = "Cam";
            int age = 30;

            // Modern: string interpolation
            Console.WriteLine(BuildProfileLine(name, age));

            // Old approach (discouraged):
            // string.Format("Name: {0}, Age: {1}", name, age)
            // $"Name: {name}, Age: {age}"  // Expression interpolation

            // Expressions in interpolation
            Console.WriteLine($"Next year: {name} will be {age + 1}");
        }

        /// <summary>
        /// Demonstrates formatting and escaping.
        /// </summary>
        private static void DemoFormattingEscaping()
        {
            string root = @"C:\Users\Workspace";
            string fileName = "notes.txt";

            Console.WriteLine(BuildEscapedPath(root, "file.txt"));
            Console.WriteLine(BuildPathVerbatim(root, "file.txt"));

            // Newline example
            Console.WriteLine($"Line1{Environment.NewLine}Line2");

            // Tab example
            Console.WriteLine("Name\t|\tValue");
            Console.WriteLine("-----\t|\t-----");
            Console.WriteLine("Item1\t|\t100");
        }

        /// <summary>
        /// Demonstrates name normalization.
        /// </summary>
        private static void DemoNormalization()
        {
            string[] testNames = ["   cU tHAnH  cAm   ", "charlie cu", "  mary  jane  smith  "];

            Console.WriteLine("Name normalization:");
            foreach (string name in testNames)
            {
                Console.WriteLine($"  '{name}' → '{NormalizeName(name)}'");
            }
        }

        /// <summary>
        /// Demonstrates case operations.
        /// </summary>
        private static void DemoCaseOperations()
        {
            string text = "Hello World";

            Console.WriteLine($"Original: {text}");
            Console.WriteLine($"Upper: {text.ToUpperInvariant()}");
            Console.WriteLine($"Lower: {text.ToLowerInvariant()}");

            Console.WriteLine();
            Console.WriteLine("Initials example:");
            string[] fullNames = ["Charlie Cu", "Mary Jane Smith", "Bob"];
            foreach (string fullName in fullNames)
            {
                Console.WriteLine($"  {fullName} → {BuildInitials(fullName)}");
            }

            Console.WriteLine();
            Console.WriteLine("Email-like strings:");
            string[] emails = ["charliecu@outlook.com", "invalid-email", "cuthanhcam04@gmail.com"];
            foreach (string email in emails)
            {
                Console.WriteLine($"  {email}: {(LooksLikeEmail(email) ? "✓" : "✗")}");
            }

            Console.WriteLine();
            Console.WriteLine("Truncation:");
            string longText = "This is a very long text that needs truncation";
            Console.WriteLine($"  Original: {longText}");
            Console.WriteLine($"  Truncated (20): {Truncate(longText, 20)}");
            Console.WriteLine($"  Truncated (30): {Truncate(longText, 30)}");
        }

        /// <summary>
        /// Demonstrates string comparison.
        /// </summary>
        private static void DemoComparison()
        {
            Console.WriteLine("Case-sensitive comparison:");
            Console.WriteLine($"  'DOTNET' == 'dotnet': {AreEqualIgnoreCase("DOTNET", "dotnet")}");
            Console.WriteLine($"  'CSharp' == 'csharp': {AreEqualIgnoreCase("CSharp", "csharp")}");

            Console.WriteLine();
            Console.WriteLine("StringComparison options:");
            Console.WriteLine("  Ordinal: fast, byte comparison, culture-independent");
            Console.WriteLine("  OrdinalIgnoreCase: fast, case-insensitive");
            Console.WriteLine("  CurrentCulture: slower, culture-aware");
            Console.WriteLine("  InvariantCulture: slower, but consistent");

            Console.WriteLine();
            Console.WriteLine("Best practice for case-insensitive:");
            Console.WriteLine("  Always use StringComparison.OrdinalIgnoreCase");
            Console.WriteLine("  (unless culture-specific matching is required)");
        }
    }
}
