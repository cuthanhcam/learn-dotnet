using System;
using System.Collections.Generic;
using System.Numerics;

namespace CSharpBasics.Examples.Methods
{
    /// <summary>
    /// Comprehensive lesson for method overloading and params arrays.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Method overloading:
    /// - Same method name, different parameter types or counts
    /// - Resolved at compile-time (static binding)
    /// 
    /// Key topics:
    /// - Type-based overloading (int vs double vs decimal)
    /// - Arity-based overloading (different parameter counts)
    /// - Params arrays for variable-length arguments
    /// - Overload resolution rules
    /// 
    /// Overload resolution rules (in order):
    /// 1. Exact type match (int invokes int version, not object)
    /// 2. Implicit conversion (int → long)
    /// 3. Boxing/unboxing (int → object)
    /// 4. Best match among params versions
    /// 
    /// Best practices:
    /// - Overload only when semantics are identical
    /// - Keep overloads similar in behavior and performance
    /// - Document why overloads exist
    /// - Consider using optional parameters instead
    /// - Avoid too many overloads (max 3-4)
    /// - Use params sparingly, prefer explicit types
    /// 
    /// When to use overloading:
    /// - Same operation on different types
    /// - Convenience methods with common configurations
    /// - Builder patterns
    /// 
    /// When NOT to use overloading:
    /// - When methods do different things (use different names)
    /// - When optional parameters work better
    /// - When it confuses the API
    /// </summary>
    public static class OverloadingExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} OverloadingExample {new string('=', 5)}");

            PrintSection("TYPE-BASED OVERLOADING");
            DemoTypeBasedOverloading();

            PrintSection("PARAMS ARRAYS");
            DemoParamsArrays();

            PrintSection("OVERLOAD RESOLUTION");
            DemoOverloadResolution();

            PrintSection("GENERIC NUMERIC OVERLOAD");
            DemoGenericOverload();

            Console.WriteLine();
        }

        // OVERLOADED METHODS (Type-based)

        /// <summary>
        /// Multiplies two integers.
        /// </summary>
        public static int Multiply(int left, int right) => left * right;

        /// <summary>
        /// Multiplies two doubles with precision.
        /// </summary>
        public static double Multiply(double left, double right) => left * right;

        /// <summary>
        /// Multiplies two decimals (for financial calculations).
        /// </summary>
        public static decimal Multiply(decimal left, decimal right) => left * right;

        /// <summary>
        /// Multiplies multiple integers using params array.
        /// Variable-argument version for flexibility.
        /// </summary>
        public static int Multiply(params int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            if (values.Length == 0)
                return 1;  // Multiplicative identity

            int result = 1;
            foreach (int value in values)
            {
                checked
                {
                    result *= value;
                }
            }

            return result;
        }

        // ADD OVERLOADS (for demonstration)

        /// <summary>
        /// Adds two integers.
        /// </summary>
        public static int Add(int left, int right) => left + right;

        /// <summary>
        /// Adds two doubles.
        /// </summary>
        public static double Add(double left, double right) => left + right;

        /// <summary>
        /// Adds multiple integers.
        /// </summary>
        public static int Add(params int[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            int total = 0;
            foreach (int value in values)
            {
                checked
                {
                    total += value;
                }
            }

            return total;
        }

        /// <summary>
        /// Generic numeric overload using modern .NET generic math.
        /// Useful when APIs should support multiple numeric types without duplication.
        /// </summary>
        public static T Add<T>(T left, T right) where T : INumber<T> => left + right;

        // FORMAT OVERLOADS (Different responsibilities)

        /// <summary>
        /// Formats a name with default separator.
        /// </summary>
        public static string FormatPair(string left, string right) => FormatPair(left, right, " ");

        /// <summary>
        /// Formats a name with custom separator.
        /// </summary>
        public static string FormatPair(string left, string right, string separator)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            ArgumentNullException.ThrowIfNull(separator);

            return $"{left.Trim()}{separator}{right.Trim()}";
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates overload resolution based on type.
        /// Compiler chooses the exact type match.
        /// </summary>
        private static void DemoTypeBasedOverloading()
        {
            // Each call invokes the most specific matching overload
            Console.WriteLine($"Multiply(3, 4) [int] = {Multiply(3, 4)}");
            Console.WriteLine($"Multiply(2.5, 4.0) [double] = {Multiply(2.5, 4.0)}");
            Console.WriteLine($"Multiply(2.5m, 4.0m) [decimal] = {Multiply(2.5m, 4.0m)}");

            Console.WriteLine();
            Console.WriteLine("Compiler resolves to exact type match:");
            Console.WriteLine("  - 3, 4 → int version");
            Console.WriteLine("  - 2.5, 4.0 → double version");
            Console.WriteLine("  - 2.5m, 4.0m → decimal version");
        }

        /// <summary>
        /// Demonstrates params arrays for variable-length arguments.
        /// </summary>
        private static void DemoParamsArrays()
        {
            // Single element
            Console.WriteLine($"Multiply(5) = {Multiply(5)}");

            // Two elements
            Console.WriteLine($"Multiply(2, 3) = {Multiply(2, 3)}");  // Could be (int, int) or params!

            // Multiple elements
            Console.WriteLine($"Multiply(2, 3, 4) = {Multiply(2, 3, 4)}");

            // No elements
            Console.WriteLine($"Multiply() = {Multiply()}");
            Console.WriteLine("Multiply() returns 1 (multiplicative identity)");

            Console.WriteLine();
            Console.WriteLine("Addition with params:");
            Console.WriteLine($"Add(1, 2, 3, 4, 5) = {Add(1, 2, 3, 4, 5)}");

            Console.WriteLine();
            Console.WriteLine("Params allows variable-length arguments in one call");
        }

        /// <summary>
        /// Demonstrates how overload resolution works.
        /// </summary>
        private static void DemoOverloadResolution()
        {
            Console.WriteLine("Overload resolution examples:");

            // Exact type matches
            int sumInt = Add(1, 2);
            Console.WriteLine($"  Add(1, 2) → int version = {sumInt}");

            double sumDouble = Add(1.5, 2.5);
            Console.WriteLine($"  Add(1.5, 2.5) → double version = {sumDouble}");

            // Ambiguous: could be (int, int) or params!
            int ambiguous = Multiply(2, 3);
            Console.WriteLine($"  Multiply(2, 3) → resolved to int version (exact match preferred over params)");

            // String formatting with default
            Console.WriteLine();
            Console.WriteLine("Format overloads:");
            Console.WriteLine($"  FormatPair(\"John\", \"Doe\") = {FormatPair("John", "Doe")}");
            Console.WriteLine($"  FormatPair(\"John\", \"Doe\", \"-\") = {FormatPair("John", "Doe", "-")}");

            Console.WriteLine();
            Console.WriteLine("Overload resolution follows predictable rules:");
            Console.WriteLine("  1. Exact type match");
            Console.WriteLine("  2. Implicit conversion");
            Console.WriteLine("  3. Params/variable-length");
        }

        /// <summary>
        /// Demonstrates generic math overload with explicit type arguments.
        /// </summary>
        private static void DemoGenericOverload()
        {
            int intResult = Add<int>(10, 20);
            decimal decimalResult = Add<decimal>(3.25m, 4.75m);

            Console.WriteLine($"Add<int>(10, 20) = {intResult}");
            Console.WriteLine($"Add<decimal>(3.25m, 4.75m) = {decimalResult}");
            Console.WriteLine("Generic math reduces duplicate overloads when behavior is identical.");
        }
    }
}
