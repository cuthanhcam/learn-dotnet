namespace OopBasics.Examples.Classes
{
    /// <summary>
    /// Demonstrates:
    /// - Immutable objects
    /// - Record types
    /// - Value-based equality
    /// </summary>
    public class ImmutableObjectExample
    {
        public static void Run()
        {
            Console.WriteLine("ImmutableObjectExample: Using immutable record types");

            var user1 = new User("Alice", 25);
            var user2 = user1 with { Age = 26 };

            Console.WriteLine($"Original: {user1}");
            Console.WriteLine($"Modified copy: {user2}");

            Console.WriteLine($"Are equal? {user1 == user2}");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Immutable objects cannot be changed after creation.");
            Console.WriteLine("- 'with' creates a copy with modifications.");
            Console.WriteLine("- Records use value-based equality.");
        }
    }

    public record User(string Name, int Age);
}
