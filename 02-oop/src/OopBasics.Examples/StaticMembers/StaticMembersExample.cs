namespace OopBasics.Examples.StaticMembers
{
    /// <summary>
    /// Demonstrates:
    /// - Static fields and methods
    /// - Shared state across instances
    /// - Static constructor
    /// - Utility class pattern
    /// </summary>
    public class StaticMembersExample
    {
        public static void Run()
        {
            Console.WriteLine("StaticMembersExample: Shared state and utility usage");

            var user1 = new User("Alice");
            var user2 = new User("Bob");

            Console.WriteLine($"Total users created: {User.TotalUsers}");

            Console.WriteLine($"Sum: {MathHelper.Add(5, 3)}");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Static members belong to the type, not instances.");
            Console.WriteLine("- Shared across all objects.");
            Console.WriteLine("- Useful for utilities and global counters.");
        }
    }

    /// <summary>
    /// Demonstrates static field + static constructor
    /// </summary>
    public class User
    {
        public string Name { get; }

        public static int TotalUsers { get; private set; }

        /// <summary>
        /// Static constructor (runs once per type)
        /// </summary>
        static User()
        {
            Console.WriteLine("Static constructor called (once per type).");
            TotalUsers = 0;
        }

        public User(string name)
        {
            Name = name;
            TotalUsers++;
        }
    }

    /// <summary>
    /// Demonstrates static utility class pattern
    /// </summary>
    public static class MathHelper
    {
        public static int Add(int a, int b) => a + b;

        public static int Multiply(int a, int b) => a * b;
    }
}
