using System;
using System.Diagnostics.CodeAnalysis;

namespace CSharpBasics.Examples.Nullability;

/// <summary>
/// Demonstrates null handling patterns in modern C# 8.0+.
/// 
/// Covers:
/// - Nullable reference types (NRT) - compiler-enforced null safety
/// - Nullable value types (int?, string?, etc.)
/// - Null-coalescing operators (??, ??=)
/// - Null-conditional operators (?., ?[])
/// - Pattern matching with null
/// - Guard clauses and parameter validation
/// - TryParse and out parameters
/// 
/// Why null safety matters:
/// - Eliminates "billion-dollar mistake" (NullReferenceException)
/// - Makes intent explicit in code
/// - Compiler warns at build time, not runtime
/// </summary>
public static class NullabilityExample
{
    public static void Run()
    {
        Console.WriteLine($"{new string('=', 5)} Nullability Examples {new string('=', 5)}");

        PrintSection("NULLABLE VALUE TYPES");
        DemoNullableValueTypes();

        PrintSection("NULL-COALESCING OPERATORS");
        DemoNullCoalescing();

        PrintSection("NULL-CONDITIONAL OPERATORS");
        DemoNullConditional();

        PrintSection("PATTERN MATCHING WITH NULL");
        DemoPatternMatching();

        PrintSection("GUARD CLAUSES");
        DemoGuardClauses();

        PrintSection("TRYPARSE AND OUT");
        DemoTryParsePattern();

        Console.WriteLine();
    }

    // PUBLIC TEACHING METHODS

    /// <summary>
    /// Parses a nullable integer from string.
    /// Returns null if parsing fails.
    /// </summary>
    public static int? ParseNullableInteger(string? input)
    {
        return int.TryParse(input, out var result) ? result : null;
    }

    /// <summary>
    /// Gets the first non-null value from a set of options.
    /// Demonstrates ?? (null-coalescing) operator.
    /// </summary>
    public static string GetFirstNonNull(string? option1, string? option2, string? option3, string fallback)
    {
        return option1 ?? option2 ?? option3 ?? fallback;
    }

    /// <summary>
    /// Safely accesses nested property using ?. operator.
    /// Returns null if any intermediate value is null.
    /// </summary>
    public static int? GetUserAgeOrNull(User? user)
    {
        return user?.Profile?.Age;
    }

    /// <summary>
    /// Classifies a value using null pattern matching.
    /// Covers: null, not-null, and property patterns.
    /// </summary>
    public static string ClassifyValue(object? value)
    {
        return value switch
        {
            null => "Value is null",
            string { Length: 0 } => "Empty string",
            string s => $"Non-empty string ({s.Length} chars)",
            int n => $"Integer: {n}",
            User { Name: not null } u => $"User: {u.Name}",
            _ => "Unknown type",
        };
    }

    /// <summary>
    /// Safe method that requires non-null input.
    /// Uses guard clause pattern.
    /// </summary>
    public static void ProcessName(string? name)
    {
        // Guard clause: fail fast if invalid
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

        // Safe to use 'name' as non-null from here on
        Console.WriteLine($"Processing: {name}");
    }

    // PRIVATE DEMO METHODS

    private static void PrintSection(string title)
    {
        Console.WriteLine();
        Console.WriteLine($"{new string('-', 3)} {title} {new string('-', 3)}");
    }

    /// <summary>
    /// Demonstrates nullable value types (int?, decimal?, etc.).
    /// 
    /// Nullable value types allow:
    /// - Explicit representation of "no value"
    /// - Safer than using -1 or 0 as sentinel values
    /// - HasValue property and Value property
    /// </summary>
    private static void DemoNullableValueTypes()
    {
        int? maybeAge = null;
        int? actualAge = 25;

        Console.WriteLine($"maybeAge.HasValue: {maybeAge.HasValue}");
        Console.WriteLine($"actualAge.HasValue: {actualAge.HasValue}");
        Console.WriteLine($"actualAge.Value: {actualAge.Value}");

        // GetValueOrDefault provides fallback
        Console.WriteLine($"maybeAge.GetValueOrDefault(): {maybeAge.GetValueOrDefault(0)}");
    }

    /// <summary>
    /// Demonstrates null-coalescing operators (?? and ??=).
    /// 
    /// ?? : Return left if not null, else right
    /// ??= : Assign right to left only if left is null
    /// </summary>
    private static void DemoNullCoalescing()
    {
        string? user = null;
        string? backup = "Guest";
        string? fallback = "Anonymous";

        // ?? operator: chain multiple fallbacks
        string name = user ?? backup ?? fallback ?? "Unknown";
        Console.WriteLine($"Resolved name: {name}");

        // ??= operator: assign only if null
        user ??= "DefaultUser";
        Console.WriteLine($"After ??=: user = {user}");

        var result = user ?? "Still have value";
        Console.WriteLine($"Next ?? check: {result}");
    }

    /// <summary>
    /// Demonstrates null-conditional operators (?. and ?[]).
    /// 
    /// ?. : Safely access property/method if not null
    /// ?[] : Safely access indexed value if not null
    /// Returns null if any intermediate step is null
    /// </summary>
    private static void DemoNullConditional()
    {
        User? user = null;
        int? age = user?.Profile?.Age;  // Safe even though user is null
        Console.WriteLine($"Null user's age: {age ?? -1}");

        user = new User { Name = "Alice", Profile = new UserProfile { Age = 30 } };
        age = user?.Profile?.Age;
        Console.WriteLine($"Valid user's age: {age}");

        // Null-conditional with indexer
        string? text = null;
        char? firstChar = text?[0];  // Safe, returns null
        Console.WriteLine($"First char of null string: {(firstChar.HasValue ? firstChar.Value : "null")}");
    }

    /// <summary>
    /// Demonstrates pattern matching with null.
    /// 
    /// Patterns can match:
    /// - null
    /// - not null
    /// - Property conditions
    /// - Type conditions
    /// - Combinations with 'and'/'or'
    /// </summary>
    private static void DemoPatternMatching()
    {
        object? value1 = null;
        object? value2 = "hello";
        object? value3 = 42;

        Console.WriteLine($"null pattern: {ClassifyValue(value1)}");
        Console.WriteLine($"string pattern: {ClassifyValue(value2)}");
        Console.WriteLine($"int pattern: {ClassifyValue(value3)}");

        var user = new User { Name = "Bob", Profile = new UserProfile { Age = 35 } };
        Console.WriteLine($"User with profile: {ClassifyValue(user)}");

        var emptyUser = new User { Name = null };
        Console.WriteLine($"User without name: {ClassifyValue(emptyUser)}");
    }

    /// <summary>
    /// Demonstrates guard clause pattern for null safety.
    /// 
    /// Guard clauses:
    /// - Check preconditions first
    /// - Throw if invalid
    /// - Allow rest of method to assume non-null
    /// - More readable than nested null checks
    /// </summary>
    private static void DemoGuardClauses()
    {
        try
        {
            ProcessName(null);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Guard caught error: {ex.Message}");
        }

        try
        {
            ProcessName("ValidName");
            Console.WriteLine("Successfully processed valid name");
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Unexpected error");
        }
    }

    /// <summary>
    /// Demonstrates TryParse pattern with out parameters.
    /// 
    /// TryParse:
    /// - Returns bool success indicator
    /// - Outputs result via 'out' parameter
    /// - No exceptions on invalid input
    /// - Preferred over parse + try/catch
    /// </summary>
    private static void DemoTryParsePattern()
    {
        string?[] inputs = ["123", "not-a-number", "", null, "-999"];

        foreach (var input in inputs)
        {
            if (int.TryParse(input, out int number))
            {
                Console.WriteLine($"Successfully parsed '{input}' as {number}");
            }
            else
            {
                Console.WriteLine($"Failed to parse '{input}' as integer");
            }
        }
    }

    // SUPPORTING TYPES FOR DEMOS

    /// <summary>
    /// Simple user entity for null safety demos.
    /// </summary>
    public record User
    {
        public string? Name { get; set; }
        public UserProfile? Profile { get; set; }
    }

    /// <summary>
    /// User profile with nullable age field.
    /// </summary>
    public record UserProfile
    {
        public int? Age { get; set; }
    }
}
