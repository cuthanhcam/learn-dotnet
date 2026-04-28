namespace OopBasics.Examples.Inheritance
{
    /// <summary>
    /// Demonstrates:
    /// - Constructor chaining
    /// - Calling base constructor
    /// - Initialization flow
    /// </summary>
    public class BaseConstructorExample
    {
        public static void Run()
        {
            Console.WriteLine("BaseConstructorExample: Constructor chaining");

            var employee = new Employee("Alice", 30, "Software Engineer");

            Console.WriteLine(employee);

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Derived constructors must call base constructors.");
            Console.WriteLine("- Base class initializes shared state.");
            Console.WriteLine("- Helps enforce consistency.");
        }
    }

    public class Person
    {
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Person(string name, int age)
        {
            Console.WriteLine("Person constructor called");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException();

            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age));

            Name = name;
            Age = age;
        }
    }

    public class Employee : Person
    {
        public string Role { get; private set; }

        public Employee(string name, int age, string role)
            : base(name, age)
        {
            Console.WriteLine("Employee constructor called");

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException();

            Role = role;
        }

        public override string ToString()
        {
            return $"Employee(Name: {Name}, Age: {Age}, Role: {Role})";
        }
    }
}
