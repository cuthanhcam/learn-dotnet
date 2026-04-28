namespace OopBasics.Examples.Classes
{
    public class ClassBasicsExample
    {
        public static void Run()
        {
            Console.WriteLine("ClassBasicsExample: Clean OOP with encapsulation");

            var person = new Person("Charlie", 22);

            Console.WriteLine($"Created: {person}");

            person.CelebrateBirthday();
            Console.WriteLine($"After birthday: {person}");

            person.ChangeName("Charles");
            Console.WriteLine($"After name change: {person}");

            Console.WriteLine($"Is adult? {person.IsAdult}");

            Console.WriteLine("\nBest Practices:");
            Console.WriteLine("- Use private setters to protect state");
            Console.WriteLine("- Modify state via methods (behavior)");
            Console.WriteLine("- Validate data in one place");
        }
    }

    /// <summary>
    /// A well-encapsulated Person class
    /// Demonstrates:
    /// - Encapsulation with private setters
    /// - Validation
    /// - Behavior-driven design
    /// - Computed properties
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Public read, private write → controlled mutation
        /// </summary>
        public string Name { get; private set; }

        public int Age { get; private set; }

        /// <summary>
        /// Computed property (no stored state)
        /// </summary>
        public bool IsAdult => Age >= 18;

        /// <summary>
        /// Constructor ensures valid object from the start
        /// </summary>
        public Person(string name, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (age < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
            }

            Name = name;
            Age = age;
        }

        /// <summary>
        /// Domain behavior: birthday
        /// </summary>
        public void CelebrateBirthday()
        {
            Age++;
        }

        /// <summary>
        /// Domain behavior: change name
        /// </summary>
        public void ChangeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            Name = name;
        }

        /// <summary>
        /// Internal validation logic
        /// </summary>
        private void SetAge(int age)
        {
            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");

            Age = age;
        }

        public override string ToString()
        {
            return $"Person(Name: {Name}, Age: {Age}, IsAdult: {IsAdult})";
        }
    }
}
