using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBasics.Examples.Methods
{
    /// <summary>
    /// Comprehensive lesson for ref, out, and in parameter modifiers.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Parameter passing mechanisms:
    /// - by value: Copy of data passed (default for value types)
    /// - by reference (ref): Alias to original variable
    /// - by output (out): For returning multiple values
    /// - by readonly reference (in): Avoid unnecessary copying
    /// 
    /// Key topics:
    /// - ref: Modify variable, input + output
    /// - out: Return values, must be assigned
    /// - in: Optimization, prevent accidental copying, readonly
    /// - struct passing (performance implications)
    /// - multiple return values
    /// 
    /// Ref vs Out:
    /// - ref: Both input and output; variable must be initialized first
    /// - out: Only output; doesn't care about initial value
    /// 
    /// When to use:
    /// - ref: Explicit aliasing needed, rarely needed in modern C#
    /// - out: Multiple return values (less preferred than tuples in C# 7.0+)
    /// - in: Large struct parameters for performance
    /// 
    /// Best practices:
    /// - Prefer tuples over out parameters (C# 7.0+)
    /// - Use in for large value types (structs)
    /// - Avoid ref unless explicitly needed
    /// - Document why modifiers are used
    /// - Consider overloads for clarity
    /// 
    /// ⚠ WARNING: These are advanced features. Use carefully!
    /// Modern alternatives are usually better:
    /// - Multiple returns → use tuples
    /// - Variable lookup → use nullable types
    /// - Performance → measure before optimizing with in
    /// </summary>
    public static class ParamModifiersExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} ParamModifiersExample {new string('=', 5)}");

            PrintSection("REF PARAMETER (MUTABLE REFERENCE)");
            DemoRefParameter();

            PrintSection("OUT PARAMETERS (RETURN VALUES)");
            DemoOutParameters();

            PrintSection("IN PARAMETER (READONLY REFERENCE)");
            DemoInParameter();

            PrintSection("PRACTICAL EXAMPLES");
            DemoPracticalUsage();

            Console.WriteLine();
        }

        // REF EXAMPLES (Modify variable by reference)

        /// <summary>
        /// Increments a value by reference.
        /// The caller's variable is directly modified.
        /// Requires 'ref' both on method and call site.
        /// </summary>
        public static void Increment(ref int value) => value++;

        /// <summary>
        /// Doubles a value by reference.
        /// </summary>
        public static void Double(ref double value) => value *= 2;

        /// <summary>
        /// Swaps two integers using ref parameters.
        /// Classic application of ref (though out-of-favor).
        /// </summary>
        public static void Swap<T>(ref T left, ref T right)
        {
            (left, right) = (right, left);
        }

        // OUT EXAMPLES (Return multiple values)

        /// <summary>
        /// Attempts integer division and returns components.
        /// Returns: success flag as return value, quotient and remainder as out params.
        /// Note: Tuple return is preferred in C# 7.0+ (see alternative below).
        /// </summary>
        public static bool TryDivide(int dividend, int divisor, out int quotient, out int remainder)
        {
            if (divisor == 0)
            {
                quotient = 0;
                remainder = 0;
                return false;
            }

            quotient = dividend / divisor;
            remainder = dividend % divisor;
            return true;
        }

        /// <summary>
        /// Modern alternative: Use tuple instead of out parameters.
        /// Easier to read and use (no 'out' keyword needed).
        /// PREFERRED approach in modern C#.
        /// </summary>
        public static (bool Success, int Quotient, int Remainder) TryDivideModern(int dividend, int divisor)
        {
            if (divisor == 0)
                return (false, 0, 0);

            return (true, dividend / divisor, dividend % divisor);
        }

        /// <summary>
        /// Parses multiple parts of a formatted string.
        /// Uses out parameters to return parsed components.
        /// Example: "John:25" → name="John", age=25
        /// </summary>
        public static bool TryParseNameAge(string input, out string name, out int age)
        {
            name = string.Empty;
            age = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] parts = input.Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[1], out int parsedAge))
                return false;

            name = parts[0].Trim();
            age = parsedAge;
            return true;
        }

        // IN EXAMPLES (Readonly reference for performance)

        /// <summary>
        /// Sums two integers using 'in' parameters.
        /// 'in' prevents unnecessary copying for performance.
        /// Most useful for large structs, not primitives.
        /// </summary>
        public static int Sum(in int left, in int right) => left + right;

        /// <summary>
        /// Compares two large structs efficiently.
        /// Using 'in' avoids copying the struct to the stack.
        /// Example of legitimate 'in' usage.
        /// </summary>
        public static bool AreEqual(in ComplexValue a, in ComplexValue b)
        {
            return a.Id == b.Id && a.Name == b.Name && a.Value == b.Value;
        }

        /// <summary>
        /// Example struct that benefits from 'in' parameter.
        /// Large structs should use 'in' to avoid copying.
        /// </summary>
        public struct ComplexValue
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Value { get; set; }
            public byte[] Data { get; set; }
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates ref parameter behavior.
        /// Shows how caller's variable is modified.
        /// </summary>
        private static void DemoRefParameter()
        {
            int value = 10;
            Console.WriteLine($"Before: value = {value}");
            Increment(ref value);
            Console.WriteLine($"After Increment(ref value): value = {value}");

            double amount = 5.0;
            Console.WriteLine($"Before: amount = {amount}");
            Double(ref amount);
            Console.WriteLine($"After Double(ref amount): amount = {amount}");

            Console.WriteLine();
            Console.WriteLine("Swap example:");
            int a = 1;
            int b = 9;
            Console.WriteLine($"Before: a = {a}, b = {b}");
            Swap(ref a, ref b);
            Console.WriteLine($"After Swap(ref a, ref b): a = {a}, b = {b}");

            Console.WriteLine();
            Console.WriteLine("ref requires explicit 'ref' keyword at call site (more visible, less hidden)");
        }

        /// <summary>
        /// Demonstrates out parameter behavior.
        /// Shows old-style multiple return values.
        /// </summary>
        private static void DemoOutParameters()
        {
            // Old style with out parameters
            bool success = TryDivide(17, 5, out int quotient, out int remainder);
            Console.WriteLine($"TryDivide(17, 5) → success: {success}, quotient: {quotient}, remainder: {remainder}");

            bool failDiv = TryDivide(17, 0, out int q, out int r);
            Console.WriteLine($"TryDivide(17, 0) → success: {failDiv}");

            Console.WriteLine();
            Console.WriteLine("Modern style with tuples (PREFERRED):");
            (bool ok, int quot, int rem) = TryDivideModern(17, 5);
            Console.WriteLine($"TryDivideModern(17, 5) → success: {ok}, quotient: {quot}, remainder: {rem}");

            Console.WriteLine();
            Console.WriteLine("Parsing example:");
            if (TryParseNameAge("John:25", out string name, out int age))
            {
                Console.WriteLine($"Parsed: name = {name}, age = {age}");
            }

            if (!TryParseNameAge("InvalidFormat", out _, out _))
            {
                Console.WriteLine($"Failed to parse (safely handled)");
            }
        }

        /// <summary>
        /// Demonstrates in parameter for performance.
        /// </summary>
        private static void DemoInParameter()
        {
            int x = 30;
            int y = 12;
            Console.WriteLine($"Sum(in {x}, in {y}) = {Sum(in x, in y)}");

            Console.WriteLine();
            Console.WriteLine("Most useful for large structs:");

            var val1 = new ComplexValue { Id = 1, Name = "Item", Value = 10.5 };
            var val2 = new ComplexValue { Id = 1, Name = "Item", Value = 10.5 };

            bool equal = AreEqual(in val1, in val2);
            Console.WriteLine($"Comparing structs: {val1.Name} == {val2.Name} ? {equal}");

            Console.WriteLine();
            Console.WriteLine("'in' is mostly for optimization. Use sparingly!");
            Console.WriteLine("  Premature optimization usually not worth the added complexity.");
        }

        /// <summary>
        /// Shows practical, real-world usage scenarios.
        /// </summary>
        private static void DemoPracticalUsage()
        {
            Console.WriteLine("Common pattern: Chaining modifications");
            int counter = 0;
            Increment(ref counter);
            Increment(ref counter);
            Increment(ref counter);
            Console.WriteLine($"After 3 increments: counter = {counter}");

            Console.WriteLine();
            Console.WriteLine("Parsing structured data:");
            string[] inputs = ["Alice:30", "Bob:25", "Charlie:35"];
            foreach (string input in inputs)
            {
                if (TryParseNameAge(input, out string n, out int a))
                {
                    Console.WriteLine($"  {n}: {a} years old");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Use cases are rare in modern C#");
            Console.WriteLine("  - ref: Rarely needed (structs are immutable in practice)");
            Console.WriteLine("  - out: Replaced by tuples (C# 7.0+)");
            Console.WriteLine("  - in: Needed only for large struct optimization");
        }
    }
}
