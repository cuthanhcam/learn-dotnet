namespace CoreDotNet.Examples.NullableReferenceTypes
{
    /// <summary>
    /// Comprehensive examples for nullable reference types (NRT).
    ///
    /// This lesson shows how to model nullability intentionally:
    /// - Make nullable intent explicit in method signatures.
    /// - Use null-coalescing and null-conditional operators for safe flow.
    /// - Favor guard clauses and Try-patterns over silent failures.
    /// - Keep nullable boundaries narrow in public APIs.
    ///
    /// Best practices:
    /// - Enable NRT at project level.
    /// - Use appropriate null handling patterns.
    /// - Avoid null-forgiving operator when possible.
    /// - Document nullable parameters with XML comments.
    /// - Use defensive null checks in public APIs.
    /// </summary>
    public static class NullableReferenceTypesExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} Nullable Reference Types Examples {new string('=', 5)}");

            PrintSection("NULLABLE ANNOTATIONS");
            DemoNullableAnnotations();

            PrintSection("NULL-COALESCING OPERATORS");
            DemoNullCoalescing();

            PrintSection("NULL-CONDITIONAL OPERATORS");
            DemoNullConditional();

            PrintSection("PATTERN MATCHING");
            DemoPatternMatching();

            PrintSection("TRY-PATTERN AND FALLBACKS");
            DemoTryPatternAndFallbacks();

            PrintSection("DEFENSIVE PROGRAMMING");
            DemoDefensiveProgramming();

            PrintSection("NULLABLE VALUE TYPES");
            DemoNullableValueTypes();

            Console.WriteLine();
        }

        private static void DemoNullableAnnotations()
        {
            // Nullable reference type
            string? nullableString = null;
            Console.WriteLine($"Nullable string is null: {nullableString == null}");

            // Non-nullable reference type
            string nonNullableString = "Hello";
            Console.WriteLine($"Non-nullable string: {nonNullableString}");

            // Assigning null to non-nullable would generate warning (at compile time)
            // This would be flagged by the compiler:
            // nonNullableString = null; // Warning: Cannot assign null

            // But we can explicitly use nullable type
            Person? person = null;
            Console.WriteLine($"Person is null: {person == null}");

            person = new Person { Name = "Alice" };
            Console.WriteLine($"Person name: {person.Name}");
        }

        private static void DemoNullCoalescing()
        {
            string? nullableValue = null;

            // Null-coalescing operator (??) - provide default
            string result = nullableValue ?? "default value";
            Console.WriteLine($"Result: {result}");

            // Chaining null-coalescing
            string? first = null;
            string? second = null;
            string? third = "found it";
            string chained = first ?? second ?? third ?? "nothing found";
            Console.WriteLine($"Chained result: {chained}");

            // Null-coalescing assignment (??=)
            string? name = null;
            name ??= "Default Name";
            Console.WriteLine($"Name after ??=: {name}");

            // Won't reassign if already has value
            name ??= "Another Name";
            Console.WriteLine($"Name after second ??=: {name}");
        }

        private static void DemoNullConditional()
        {
            Person? person = null;

            // Null-conditional operator (?.) - safe navigation
            string? name = person?.Name;
            Console.WriteLine($"Null person name: {name}");

            // Length of null-conditional result is nullable
            int? length = person?.Name.Length;
            Console.WriteLine($"Length of null name: {length}");

            person = new Person { Name = "Alice" };

            // Now it returns actual value
            name = person?.Name;
            Console.WriteLine($"Non-null person name: {name}");

            length = person?.Name.Length;
            Console.WriteLine($"Length of non-null name: {length}");

            // Chaining null-conditional
            string? displayName = person?.GetDisplayInfo()?.Name ?? "Unknown";
            Console.WriteLine($"Display name: {displayName}");
        }

        private static void DemoPatternMatching()
        {
            object?[] values = { "text", null, 42, new Person { Name = "Bob" }, null };

            foreach (object? value in values)
            {
                string description = value switch
                {
                    null => "Is null",
                    string s => $"String: {s}",
                    int i => $"Integer: {i}",
                    Person p => $"Person: {p.Name}",
                    _ => "Unknown"
                };
                Console.WriteLine($"  {description}");
            }

            // Pattern matching with is keyword
            var sampleValue = new Person { Name = "Charlie" };
            if (sampleValue is Person { Name: not null } person)
            {
                Console.WriteLine($"Found person with name: {person.Name}");
            }
        }

        private static void DemoDefensiveProgramming()
        {
            var person = GetPerson();

            // Defensive approach: check before using
            if (person is not null)
            {
                Console.WriteLine($"Person exists: {person.Name}");
            }

            // Process with null-coalescing for safety
            string name = person?.Name ?? "Unknown Person";
            Console.WriteLine($"Name: {name}");

            // Defensive in method
            ProcessUser(person);

            // With null object pattern
            var user = new { Name = "Charlie", Email = "charlie@outlook.com" };
            DisplayUserInfo(user);
        }

        private static void DemoTryPatternAndFallbacks()
        {
            if (TryBuildDisplayName("Cam", "Cu", out string? displayName))
            {
                Console.WriteLine($"Built display name: {displayName}");
            }

            if (!TryBuildDisplayName(null, "Cu", out displayName))
            {
                Console.WriteLine($"Could not build display name, fallback is: {displayName ?? "<none>"}");
            }

            string? preferred = null;
            string fallback = preferred ?? "Guest learner";
            Console.WriteLine($"Fallback text: {fallback}");
        }

        private static void DemoNullableValueTypes()
        {
            // Nullable value type (Nullable<T>)
            int? nullableInt = null;
            Console.WriteLine($"Nullable int is null: {nullableInt == null}");
            Console.WriteLine($"Nullable int HasValue: {nullableInt.HasValue}");

            nullableInt = 42;
            Console.WriteLine($"After assignment, HasValue: {nullableInt.HasValue}, Value: {nullableInt.Value}");

            // Safe access with null-coalescing
            int result = nullableInt ?? 0;
            Console.WriteLine($"Int with default: {result}");

            // GetValueOrDefault
            int defaulted = nullableInt.GetValueOrDefault(-1);
            Console.WriteLine($"GetValueOrDefault: {defaulted}");

            // Arithmetic with nullable
            int? x = 10;
            int? y = 20;
            int? sum = x + y;
            Console.WriteLine($"Sum of nullable ints: {sum}");

            y = null;
            int? result2 = x + y;
            Console.WriteLine($"Sum with null: {result2 ?? -1}"); // -1 as default
        }

        private static Person? GetPerson()
        {
            // Simulating a method that might return null
            return new Random().Next(2) == 0 ? new Person { Name = "Test" } : null;
        }

        private static void ProcessUser(Person? person)
        {
            if (person?.Name is { Length: > 0 } name)
            {
                Console.WriteLine($"Processing user: {name}");
            }
            else
            {
                Console.WriteLine("Cannot process: user or name is null");
            }
        }

        private static void DisplayUserInfo(dynamic user)
        {
            // Flexible null handling
            string? name = user?.Name;
            string? email = user?.Email;
            Console.WriteLine($"User: {name ?? "Unknown"}, Email: {email ?? "No email"}");
        }

        private static bool TryBuildDisplayName(string? firstName, string? lastName, out string? displayName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                displayName = null;
                return false;
            }

            displayName = $"{firstName.Trim()} {lastName.Trim()}";
            return true;
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }

        public PersonInfo? GetDisplayInfo()
        {
            return new PersonInfo { Name = this.Name };
        }
    }

    public class PersonInfo
    {
        public string Name { get; set; } = string.Empty;
    }
}
