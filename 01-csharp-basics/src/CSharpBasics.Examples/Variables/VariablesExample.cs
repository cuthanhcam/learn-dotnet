using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CSharpBasics.Examples.Variables.VariablesExamples;

namespace CSharpBasics.Examples.Variables
{
    /// <summary>
    /// Demonstrates variable declaration, constants, readonly fields,
    /// type inference (var), nullable types, and record struct usage in C#.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// </summary>
    public static class VariablesExamples
    {
        /// <summary>
        /// Compile-time constant.
        /// - Must be known at compile time
        /// - Implicitly static
        /// - Cannot be changed at runtime
        /// </summary>
        public const string AppName = "CSharpBasics";

        public static void Run()
        {
            Console.WriteLine($"Variables Examples");
            PrintSection("PRIMITIVE SNAPSHOT");
            DemoPrimitiveSnapshot();

            PrintSection("CONST VS READONLY");
            DemoConstVsReadonly();

            PrintSection("VAR INFERENCE");
            DemoVarInference();

            PrintSection("NULLABLE VALUE TYPES");
            DemoNullableValues();

            PrintSection("RECORD STRUCT BEHAVIOR");
            DemoRecordStructBehavior();

            Console.WriteLine();
        }

        // PUBLIC METHODS (Used for demonstration / reuse)

        /// <summary>
        /// Creates a snapshot of primitive values using a factory method.
        /// Demonstrates safe initialization and default value handling. 
        /// </summary>
        public static PrimitiveSnapshot GetPrimitiveValues()
        {
            return PrimitiveSnapshot.Create(
                integerValue: 13,
                longValue: 4_200_000_000,
                decimalValue: 1234.56m,
                doubleValue: 3.14159,
                greeting: "Hello, C#",
                isActive: true,
                initial: 'C'
            );
        }

        /// <summary>
        /// Builds a display name from last name and first name.
        /// Demonstrates validation and string handling.
        /// </summary>
        public static string BuildDisplayName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name is required.", nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name is required.", nameof(firstName));
            }

            return $"{firstName.Trim()} {lastName.Trim()}";
        }

        /// <summary>
        /// Demonstrates type inference using 'var'.
        /// Note: var is resolved at compile-time, not dynamic.
        /// </summary>
        public static int InferTypeWithVar(int seed)
        {
            var computed = seed + 20; // inferred as int
            return computed;
        }

        /// <summary>
        /// Parses a nullable integer from string input.
        /// Returns null if parsing fails.
        /// </summary>
        public static int? ParseNullableInt(string? rawValue)
        {
            return int.TryParse(rawValue, out var result) ? result : null;
        }

        // RECORD STRUCT (Value Object)

        /// <summary>
        ///  Represents an immutable snapshot of primitive values.
        ///  
        /// Why record struct?
        /// - Value type (stack allocation where possible)
        /// - Immutable by default (good for data transfer objects)
        /// - Value equality semantics (compares by value, not reference)
        /// - Support 'with' expressions for easy copying with modifications
        /// 
        /// Use cases:
        /// - DTOs
        /// - Value Objects (DDD)
        /// - Snapshots of data that should not change after creation
        /// </summary>
        public readonly record struct PrimitiveSnapshot(
            Guid Id,
            int IntegerValue,
            long LongValue,
            decimal DecimalValue,
            double DoubleValue,
            string? Greeting,
            bool IsActive,
            char Initial,
            DateTime CreatedAt
        )
        {
            /// <summary>
            /// Factory method to safety create a snapshot.
            /// Prevents constructor misuse and ensures valid defaults.
            /// </summary>
            public static PrimitiveSnapshot Create(
                int integerValue,
                long longValue,
                decimal decimalValue,
                double doubleValue,
                string? greeting,
                bool isActive,
                char initial)
            {
                return new PrimitiveSnapshot(
                    Id: Guid.NewGuid(),
                    IntegerValue: integerValue,
                    LongValue: longValue,
                    DecimalValue: decimalValue,
                    DoubleValue: doubleValue,
                    Greeting: greeting,
                    IsActive: isActive,
                    Initial: initial,
                    CreatedAt: DateTime.UtcNow // Always use UTC in backend systems
                );
            }

            /// <summary>
            /// Simple validation check.
            /// Help detect invalid/default struct values.
            /// </summary>
            public bool IsValid()
            {
                return Id != Guid.Empty && CreatedAt != default;
            }
        }

        // READONLY DEMO (Immutable state example)

        /// <summary>
        /// Demonstrates readonly field usage.
        /// 
        /// - Can be assigned only in constructor
        /// - Cannot be modified afterward
        /// - Useful for immutable configuration
        /// </summary>
        public sealed class Config
        {
            public readonly string EnvironmentName;

            public Config(string environmentName)
            {
                if (string.IsNullOrWhiteSpace(environmentName))
                    throw new ArgumentException("Environment name cannot be empty.", nameof(environmentName));

                EnvironmentName = environmentName.Trim();
            }
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"{new string('-', 3)} {title} {new string('-', 3)}");
        }

        /// <summary>
        /// Demonstrates primitive snapshot usage.
        /// </summary>
        private static void DemoPrimitiveSnapshot()
        {
            var values = GetPrimitiveValues();

            // record provides built-in ToString()
            Console.WriteLine(values);

            Console.WriteLine($"Display Name: {BuildDisplayName("Charlie", "Cu")}");
        }

        /// <summary>
        /// Demonstrates difference between const and readonly.
        /// </summary>
        private static void DemoConstVsReadonly()
        {
            Console.WriteLine($"const AppName: {AppName}");

            var devConfig = new Config("Development");
            var testConfig = new Config("Testing");

            Console.WriteLine($"readonly config #1: {devConfig.EnvironmentName}");
            Console.WriteLine($"readonly config #2: {testConfig.EnvironmentName}");
        }

        /// <summary>
        /// Demonstrates var type inference.
        /// </summary>
        private static void DemoVarInference()
        {
            Console.WriteLine($"InferTypeWithVar(100) => {InferTypeWithVar(100)}");

            var amount = 12.5m; // decimal
            var message = "Strongly typed"; // string

            Console.WriteLine($"var decimal: {amount}, var string: {message}");
        }

        /// <summary>
        /// Demonstrates nullable value types.
        /// </summary>
        private static void DemoNullableValues()
        {
            int? valid = ParseNullableInt("123");
            int? invalid = ParseNullableInt("abc");

            Console.WriteLine($"Parse valid => {valid}");
            Console.WriteLine($"Parse invalid => {(invalid.HasValue ? invalid.Value : "null")}");
        }

        /// <summary>
        /// Demonstrates record struct behavior:
        /// - Immutability
        /// - with expression
        /// - value equality
        /// - default struct pitfalls
        /// </summary>
        private static void DemoRecordStructBehavior()
        {
            var original = GetPrimitiveValues();

            // Immutability via 'with'
            var modified = original with { IntegerValue = 999 };

            Console.WriteLine($"Original IntValue: {original.IntegerValue}");
            Console.WriteLine($"Modified IntValue: {modified.IntegerValue}");

            // Value equality
            var copy = original;
            Console.WriteLine($"Value equality (original == copy): {original == copy}");

            // Default struct (can be dangerous if not validated)
            PrimitiveSnapshot defaultValue = default;
            Console.WriteLine($"Default struct: {defaultValue}");
            Console.WriteLine($"Is default valid? {defaultValue.IsValid()}");
        }
    }
}
