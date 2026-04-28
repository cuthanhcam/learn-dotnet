namespace OopBasics.Examples.Classes
{
    /// <summary>
    /// Demonstrates:
    /// - Auto-properties
    /// - Property validation (getter/setter)
    /// - Object initializers
    /// - Encapsulation at property level
    /// </summary>
    public class PropertiesExample
    {
        public static void Run()
        {
            Console.WriteLine("PropertiesExample: Auto-properties & Object Initializers");

            // Object initializer (clean & readable)
            var car = new Car
            {
                Make = "Toyota",
                Model = "Corolla",
                Year = 2022
            };

            Console.WriteLine($"Car: {car}");

            // Updating property (setter with validation)
            car.Year = 2024;
            Console.WriteLine($"Updated car: {car}");

            // Demonstrate validation
            try
            {
                car.Year = 1500; // invalid
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation caught: {ex.Message}");
            }

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Auto-properties are ideal for simple data models.");
            Console.WriteLine("- Use setters to enforce validation rules.");
            Console.WriteLine("- Object initializers improve readability.");
            Console.WriteLine("- Avoid public setters in domain models (use methods instead).");
        }
    }

    /// <summary>
    /// Represents a simple Car model.
    /// Focus:
    /// - Property-based design
    /// - Validation inside setters
    /// - Safe defaults to avoid nullable warnings
    /// </summary>
    public class Car
    {
        private int _year;

        /// <summary>
        /// Auto-property with default value to avoid CS8618
        /// </summary>
        public string Make { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Property with validation logic
        /// </summary>
        public int Year
        {
            get => _year;
            set
            {
                if (value < 1886 || value > DateTime.Now.Year + 1)
                    throw new ArgumentException("Year must be between 1886 and next year.");

                _year = value;
            }
        }

        /// <summary>
        /// Readable output for debugging/logging
        /// </summary>
        public override string ToString()
        {
            return $"{Make} {Model} ({Year})";
        }
    }
}
