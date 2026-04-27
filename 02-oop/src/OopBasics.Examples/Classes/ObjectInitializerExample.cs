namespace OopBasics.Examples.Classes
{
    /// <summary>
    /// Demonstrates:
    /// - Object initializers
    /// - init-only properties (immutability)
    /// - required properties (C# 11+)
    /// - Clean ToString override
    /// </summary>
    public class ObjectInitializerExample
    {
        public static void Run()
        {
            Console.WriteLine("ObjectInitializerExample: Init-only & Required Properties");

            // Object initializer with required + init
            var book = new Book
            {
                Title = "C# in Depth",
                Author = "Jon Skeet",
                Pages = 900
            };

            Console.WriteLine($"Book: {book}");

            // Uncomment to see compile-time error (required property missing)
            // var invalidBook = new Book { Title = "Incomplete Book" };

            // Uncomment to see immutability (init-only)
            // book.Title = "New Title"; // not allowed

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- 'init' allows setting properties only during initialization.");
            Console.WriteLine("- 'required' ensures important fields are always provided.");
            Console.WriteLine("- This pattern is ideal for immutable data models.");
        }
    }

    /// <summary>
    /// Represents a book using modern C# features.
    /// Focus:
    /// - Immutable after creation
    /// - Required properties
    /// - Object initializer-friendly
    /// </summary>
    public class Book
    {
        private int _pages;

        /// <summary>
        /// Required + init → must be set during initialization
        /// </summary>
        public required string Title { get; init; }

        public required string Author { get; init; }

        /// <summary>
        /// Validation with init accessor
        /// </summary>
        public int Pages
        {
            get => _pages;
            init
            {
                if (value <= 0)
                    throw new ArgumentException("Pages must be greater than zero.");

                _pages = value;
            }
        }

        public override string ToString()
        {
            return $"'{Title}' by {Author}, {Pages} pages";
        }
    }
}
