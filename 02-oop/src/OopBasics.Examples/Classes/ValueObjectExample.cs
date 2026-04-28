using System;

namespace OopBasics.Examples.Classes
{
    /// <summary>
    /// Demonstrates:
    /// - Value Objects
    /// - Encapsulating domain concepts
    /// - Validation inside small types
    /// </summary>
    public class ValueObjectExample
    {
        public static void Run()
        {
            Console.WriteLine("ValueObjectExample: Encapsulating domain values");

            var email = new Email("user@example.com");

            Console.WriteLine($"Email: {email}");

            try
            {
                var invalid = new Email("invalid-email");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Validation error: {ex.Message}");
            }

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Value Objects represent concepts, not primitives.");
            Console.WriteLine("- They enforce validation at creation.");
            Console.WriteLine("- They improve type safety.");
        }
    }

    public class Email
    {
        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                throw new ArgumentException("Invalid email format.");

            Value = value;
        }

        public override string ToString() => Value;

        public override bool Equals(object? obj)
        {
            return obj is Email other && Value == other.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}
