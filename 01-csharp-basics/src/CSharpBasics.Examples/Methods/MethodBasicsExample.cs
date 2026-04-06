using System;
using System.Collections.Generic;

namespace CSharpBasics.Examples.Methods
{
    /// <summary>
    /// Comprehensive lesson for method fundamentals, validation, and return patterns.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Key topics:
    /// - Pure functions (no side effects)
    /// - Input validation with guard clauses
    /// - Return types and patterns
    /// - Exception handling and throwing
    /// - Tuple returns for multiple values
    /// - Try-parse style pattern
    /// 
    /// Method design principles:
    /// - Single responsibility: method does one thing
    /// - Pure functions: no global state mutations
    /// - Validate inputs: fail fast with clear errors
    /// - Clear return types: avoid out parameters when possible
    /// - Use tuple returns for multiple values in C# 7.0+
    /// 
    /// Best practices:
    /// - Use meaningful method names (verb + object)
    /// - Keep methods small and focused
    /// - Validate preconditions at method entry
    /// - Use expression-bodied members for simple logic
    /// - Return meaningful error messages
    /// - Avoid overuse of out parameters
    /// </summary>
    public static class MethodBasicsExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} MethodBasicsExample {new string('=', 5)}");

            PrintSection("PURE METHODS");
            DemoPureMethods();

            PrintSection("INPUT VALIDATION");
            DemoInputValidation();

            PrintSection("TRY-PARSE PATTERN");
            DemoTryParsePattern();

            PrintSection("MULTIPLE RETURNS WITH TUPLES");
            DemoTupleReturns();

            PrintSection("ERROR HANDLING");
            DemoErrorHandling();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Adds two integers.
        /// Pure function: no side effects, deterministic result.
        /// Uses expression-bodied member for clarity.
        /// </summary>
        public static int Add(int left, int right) => left + right;

        /// <summary>
        /// Multiplies two numbers.
        /// Pure function example.
        /// </summary>
        public static double Multiply(double left, double right) => left * right;

        /// <summary>
        /// Applies percentage discount to a price.
        /// Demonstrates guard clauses with range validation.
        /// </summary>
        public static decimal ApplyDiscount(decimal originalPrice, decimal discountPercent)
        {
            if (originalPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(originalPrice), "Price cannot be negative.");

            if (discountPercent < 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), "Discount must be between 0 and 100.");

            return originalPrice * (1 - discountPercent / 100m);
        }

        /// <summary>
        /// Builds a welcome message with validation.
        /// Demonstrates input validation using guard clauses.
        /// Throws ArgumentException with nameof for clarity.
        /// </summary>
        public static string BuildWelcomeMessage(string studentName, string moduleName)
        {
            if (string.IsNullOrWhiteSpace(studentName))
                throw new ArgumentException("Student name is required and cannot be empty.", nameof(studentName));

            if (string.IsNullOrWhiteSpace(moduleName))
                throw new ArgumentException("Module name is required and cannot be empty.", nameof(moduleName));

            return $"Welcome {studentName.Trim()}! You are studying {moduleName.Trim()}.";
        }

        /// <summary>
        /// Parses age from string or returns default.
        /// Demonstrates try-parse pattern for safe conversion.
        /// Returns a sensible default instead of throwing.
        /// </summary>
        public static int ParseAgeOrDefault(string? rawValue, int defaultValue)
        {
            if (defaultValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(defaultValue), "Default age must be positive.");

            if (int.TryParse(rawValue, out int parsed) && parsed > 0)
                return parsed;

            return defaultValue;
        }

        /// <summary>
        /// Finds min and max values in a collection.
        /// Returns tuple of two values (C# 7.0+ feature).
        /// Preferred over out parameters in modern C#.
        /// </summary>
        public static (int Min, int Max) GetMinMax(IReadOnlyList<int> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);

            if (numbers.Count == 0)
                throw new ArgumentException("Collection cannot be empty.", nameof(numbers));

            int min = numbers[0];
            int max = numbers[0];

            for (int i = 1; i < numbers.Count; i++)
            {
                if (numbers[i] < min)
                    min = numbers[i];

                if (numbers[i] > max)
                    max = numbers[i];
            }

            return (min, max);
        }

        /// <summary>
        /// Finds min, max, and average in a single pass.
        /// Demonstrates tuple returns with named fields for clarity.
        /// </summary>
        public static (int Min, int Max, double Average) GetStatistics(IReadOnlyList<int> numbers)
        {
            ArgumentNullException.ThrowIfNull(numbers);

            if (numbers.Count == 0)
                throw new ArgumentException("Collection cannot be empty.", nameof(numbers));

            int min = numbers[0];
            int max = numbers[0];
            long sum = 0;

            for (int i = 0; i < numbers.Count; i++)
            {
                if (numbers[i] < min)
                    min = numbers[i];

                if (numbers[i] > max)
                    max = numbers[i];

                sum += numbers[i];
            }

            double average = (double)sum / numbers.Count;
            return (min, max, average);
        }

        /// <summary>
        /// Safely divides two numbers.
        /// Returns false if division isn't possible (denominator = 0).
        /// Demonstrates safe operation with boolean result.
        /// </summary>
        public static bool TryDivide(decimal numerator, decimal denominator, out decimal result)
        {
            if (denominator == 0)
            {
                result = 0;
                return false;
            }

            result = numerator / denominator;
            return true;
        }

        /// <summary>
        /// Modern alternative to out-parameters for division.
        /// Returning a tuple keeps call sites concise and readable.
        /// </summary>
        public static (bool Success, decimal Value, string? Error) DivideResult(decimal numerator, decimal denominator)
        {
            if (denominator == 0)
                return (false, 0, "Denominator cannot be zero.");

            return (true, numerator / denominator, null);
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates pure functions that always produce same output for same input.
        /// </summary>
        private static void DemoPureMethods()
        {
            Console.WriteLine($"Add(7, 5) = {Add(7, 5)}");
            Console.WriteLine($"Multiply(3.5, 2.0) = {Multiply(3.5, 2.0)}");
            Console.WriteLine($"ApplyDiscount(120, 15) = {ApplyDiscount(120m, 15m):0.00}");
            Console.WriteLine("Pure methods: same input always produces same output (no side effects)");
        }

        /// <summary>
        /// Demonstrates input validation with guard clauses.
        /// </summary>
        private static void DemoInputValidation()
        {
            try
            {
                string message = BuildWelcomeMessage("Cam", "C# Fundamentals");
                Console.WriteLine($"{message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            try
            {
                string message = BuildWelcomeMessage("", "C# Fundamentals");
                Console.WriteLine($"{message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Caught validation error: {ex.ParamName} - {ex.Message}");
            }
        }

        /// <summary>
        /// Demonstrates try-parse pattern for safe conversions.
        /// </summary>
        private static void DemoTryParsePattern()
        {
            int validAge = ParseAgeOrDefault("29", 18);
            int invalidAge = ParseAgeOrDefault("abc", 18);
            int negativeAge = ParseAgeOrDefault("-5", 18);

            Console.WriteLine($"Parse '29' with default 18 => {validAge}");
            Console.WriteLine($"Parse 'abc' with default 18 => {invalidAge}");
            Console.WriteLine($"Parse '-5' with default 18 => {negativeAge}");
            Console.WriteLine("Negative values treated as invalid");
        }

        /// <summary>
        /// Demonstrates tuple returns with named fields.
        /// </summary>
        private static void DemoTupleReturns()
        {
            int[] numbers = [5, 2, 9, 1, 7, 4];

            (int min, int max) = GetMinMax(numbers);
            Console.WriteLine($"Array: {string.Join(", ", numbers)}");
            Console.WriteLine($"Min: {min}, Max: {max}");

            var (minVal, maxVal, avg) = GetStatistics(numbers);
            Console.WriteLine($"Statistics - Min: {minVal}, Max: {maxVal}, Avg: {avg:0.00}");
            Console.WriteLine("↳ Named tuple fields make results self-documenting");

            var division = DivideResult(22, 7);
            Console.WriteLine($"DivideResult(22, 7) - success: {division.Success}, value: {division.Value:0.0000}");

            var divisionError = DivideResult(10, 0);
            Console.WriteLine($"DivideResult(10, 0) - success: {divisionError.Success}, error: {divisionError.Error}");
        }

        /// <summary>
        /// Demonstrates error handling with safe operations.
        /// </summary>
        private static void DemoErrorHandling()
        {
            if (TryDivide(10, 2, out decimal result1))
            {
                Console.WriteLine($"10 / 2 = {result1}");
            }

            if (!TryDivide(10, 0, out decimal result2))
            {
                Console.WriteLine($"10 / 0 caught safely (returned false)");
            }

            Console.WriteLine("Safe methods: return bool to indicate success, no exception throwing");
        }
    }
}
