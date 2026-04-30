namespace OopBasics.Examples.Constructors
{
    /// <summary>
    /// Demonstrates:
    /// - Constructor overloading
    /// - Constructor chaining
    /// - Object initialization rules
    /// </summary>
    public class ConstructorsExample
    {
        public static void Run()
        {
            Console.WriteLine("ConstructorsExample: Overloading & chaining");

            var user1 = new User("Alice");
            var user2 = new User("Bob", 25);

            Console.WriteLine(user1);
            Console.WriteLine(user2);

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Constructors initialize objects.");
            Console.WriteLine("- Overloading allows multiple ways to create objects.");
            Console.WriteLine("- 'this(...)' enables constructor chaining.");
        }
    }

    public class User
    {
        public string Name { get; private set; }
        public int Age { get; private set; }

        /// <summary>
        /// Constructor with default age
        /// </summary>
        public User(string name) : this(name, 0)
        {
        }

        /// <summary>
        /// Main constructor
        /// </summary>
        public User(string name, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age));

            Name = name;
            Age = age;
        }

        public override string ToString()
        {
            return $"User(Name: {Name}, Age: {Age})";
        }
    }
}
