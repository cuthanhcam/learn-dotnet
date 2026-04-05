using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpBasics.Examples.Variables
{
    /// <summary>
    /// Demostrates the tradeoffs between static typing and dynamic behavior in C#.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Key topics:
    /// - Compile-time type checking vs runtime type resolution
    /// - Performance and safety implications of dynamic types
    /// - Exception handling for runtime failures
    /// - When to use typed vs dynamic approaches
    /// - Common pitfalls of dynamic programming
    /// </summary>
    public static class DynamicVsTypedExample
    {
        /// <summary>
        /// Entry point to run all demos.   
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("=== DynamicVsTypedExample ===");

            PrintSection("TYPED OPERATIONS (COMPILE-TIME SAFE)");
            DemoTypedOperations();

            PrintSection("DYNAMIC OPERATIONS (RUNTIME RESOLVED)");
            DemoDynamicOperations();

            PrintSection("ERROR HANDLING WITH DYNAMIC");
            DemoErrorHandling();

            PrintSection("PERFORMANCE COMPARISON");
            DemoPerformanceImpact();

            Console.WriteLine();
        }

        // TYPED OPERATIONS (recommended for known types)

        /// <summary>
        /// Adds two integers using static typing.
        /// - Advantages: Compile-time checking, optimal performance, IntelliSense support
        /// - Best practice: Use typed methods for known types
        /// </summary>
        public static int AddTyped(int left, int right) => left + right; // Expression-bodied method for simplicity

        /// <summary>
        /// Multiplies two doubles using static typing.
        /// </summary>
        public static double MultiplyTyped(double left, double right) => left * right; // return left * right; // Traditional method body for variety

        /// <summary>
        /// Concatenates two strings using static typing.
        /// </summary>
        public static string ConcatenateTyped(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left))
            {
                throw new ArgumentException("Left string cannot be null or whitespace.", nameof(left));
            }

            if (string.IsNullOrWhiteSpace(right))
            {
                throw new ArgumentException("Right string cannot be null or whitespace.", nameof(right));
            }

            return $"{left.Trim()} {right.Trim()}";
        }

        // DYNAMIC OPERATIONS (Avoid when possible) 
        // - Advantages: Flexibility to work with unknown types, dynamic behavior
        // - Disadvantages: No compile-time safety, potential runtime errors, performance overhead

        /// <summary>
        /// Adds two values using dynamic typing.
        /// 
        /// Disadvantages:
        /// - Type checking deferred to runtime
        /// - No compile-time IntelliSense or error detection
        /// - Performance overhead due to reflection/binding
        /// - Stack traces can be confusing for errors
        /// - Only consider when truly dealing with unknown types (e.g., JSON deserialization)
        /// </summary>
        public static dynamic AddDynamic(dynamic left, dynamic right)
        {
            try
            {
                return left + right;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                throw new InvalidOperationException("Runtime binding failed for addition operation.", ex);
            }
        }

        /// <summary>
        /// Demonstrates safe dynamic operation with error handling.
        /// </summary>
        public static bool TryMultiplyDynamic(dynamic left, dynamic right, out dynamic result)
        {
            try
            {
                result = left * right;
                return true;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                result = null;
                return false;
            }
        }

        // DYNAMIC PROPERTY ACCESS (Runtime-resolved)

        /// <summary>
        /// Attempts to access a property that may not exist.
        /// Useful for working with JSON or loosely-typed objects.
        /// 
        /// Risk: Runtime exceptions if property doesn't exist
        /// </summary>
        public static bool TryAccessProperty(string propertyName, out object? value)
        {
            dynamic person = new ExpandoObject();
            person.Name = "Charlie Cu";
            person.Title = "Software Engineer";

            try
            {
                value = person.GetType().GetProperty(propertyName)?.GetValue(person);
                return value != null;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// Attempts to call a method that may not exist at compile-time.
        /// </summary>
        public static bool TryInvokeDynamicMethod(out string errorMessage)
        {
            dynamic obj = new ExpandoObject();
            obj.Name = "Test";

            try
            {
                obj.NonExistentMethod();
                errorMessage = string.Empty;
                return true;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Attempts to access a missing property on a dynamic object.
        /// Returns false when property doesn't exist.
        /// </summary>
        public static bool TryAccessMissingProperty()
        {
            dynamic obj = new ExpandoObject();
            obj.Name = "Test Object";

            try
            {
                // Try to access property that doesn't exist
                string result = obj.NonExistentProperty;
                return result != null;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to call a method that doesn't exist on a dynamic object.
        /// Returns false and captures error message when method doesn't exist.
        /// </summary>
        public static bool TryCallUnknownMethod(out string errorMessage)
        {
            dynamic obj = new ExpandoObject();
            obj.Data = "Some data";

            try
            {
                obj.UnknownMethod();
                errorMessage = string.Empty;
                return true;
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"{new string('-', 3)} {title} {new string('-', 3)}");
        }

        /// <summary>
        /// Demonstrates typed operations with predictable compile-time checking.
        /// </summary>
        private static void DemoTypedOperations()
        {
            int sum = AddTyped(10, 20);
            Console.WriteLine($"Typed addition (10 + 20): {sum}");

            double product = MultiplyTyped(2.5, 4.0);
            Console.WriteLine($"Typed multiplication (2.5 * 4.0): {product}");

            string greeting = ConcatenateTyped("Hello", "C#");
            Console.WriteLine($"Typed concatenation: {greeting}");

            Console.WriteLine("✓ All operations completed safely at compile-time");
        }

        /// <summary>
        /// Demonstrates dynamic operations that defer type checking to runtime.
        /// </summary>
        private static void DemoDynamicOperations()
        {
            dynamic sum = AddDynamic(10, 20);
            Console.WriteLine($"Dynamic addition (10 + 20): {sum}");

            dynamic stringSum = AddDynamic("Hello", " World");
            Console.WriteLine($"Dynamic addition (\"Hello\" + \" World\"): {stringSum}");

            if (TryMultiplyDynamic(3, 4, out dynamic result))
            {
                Console.WriteLine($"Dynamic multiplication (3 * 4): {result}");
            }

            Console.WriteLine("All operations checked at runtime");
        }

        /// <summary>
        /// Demonstrates error handling when dynamic operations fail.
        /// </summary>
        private static void DemoErrorHandling()
        {
            Console.WriteLine("Attempting unsafe dynamic operations...");

            if (!TryMultiplyDynamic("text", 5, out _))
            {
                Console.WriteLine("Cannot multiply string * int (safely caught)");
            }

            if (!TryInvokeDynamicMethod(out string errorMessage))
            {
                Console.WriteLine($"Method invocation failed (safely caught)");
                Console.WriteLine($"Error: {errorMessage}");
            }
        }

        /// <summary>
        /// Demonstrates performance difference between typed and dynamic.
        /// Dynamic is significantly slower due to runtime binding overhead.
        /// </summary>
        private static void DemoPerformanceImpact()
        {
            const int iterations = 10_000_000;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Typed operation
            int typedResult = 0;
            for (int i = 0; i < iterations; i++)
            {
                typedResult = AddTyped(10, 20);
            }
            sw.Stop();
            long typedMs = sw.ElapsedMilliseconds;

            sw.Restart();
            // Dynamic operation
            dynamic dynamicResult = 0;
            for (int i = 0; i < iterations; i++)
            {
                dynamicResult = AddDynamic(10, 20);
            }
            sw.Stop();
            long dynamicMs = sw.ElapsedMilliseconds;

            Console.WriteLine($"Typed ({iterations:N0} iterations): {typedMs} ms");
            Console.WriteLine($"Dynamic ({iterations:N0} iterations): {dynamicMs} ms");
            Console.WriteLine($"Overhead ratio: {(double)dynamicMs / typedMs:0.0}x slower");
            Console.WriteLine();
            Console.WriteLine("Recommendation: Use typed methods unless dealing with truly unknown types.");
        }
    }
}
