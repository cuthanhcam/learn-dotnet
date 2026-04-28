namespace OopBasics.Examples.Inheritance
{
    /// <summary>
    /// Demonstrates:
    /// - Basic inheritance
    /// - Base and derived classes
    /// - Method reuse
    /// </summary>
    public class InheritanceExample
    {
        public static void Run()
        {
            Console.WriteLine("InheritanceExample: Base and Derived classes");

            var dog = new Dog("Buddy", 3);
            var cat = new Cat("Whiskers", 2);

            dog.Speak();
            cat.Speak();

            Console.WriteLine($"Dog: {dog}");
            Console.WriteLine($"Cat: {cat}");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Inheritance allows code reuse.");
            Console.WriteLine("- Derived classes extend base behavior.");
            Console.WriteLine("- Use 'is-a' relationship (Dog is an Animal).");
        }
    }

    public class Animal
    {
        public string Name { get; private set; }
        public int Age { get; private set; }

        public Animal(string name, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");

            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age));

            Name = name;
            Age = age;
        }

        public virtual void Speak()
        {
            Console.WriteLine($"{Name} makes a sound.");
        }

        public override string ToString()
        {
            return $"{GetType().Name}(Name: {Name}, Age: {Age})";
        }
    }

    public class Dog : Animal
    {
        public Dog(string name, int age) : base(name, age) { }

        public override void Speak()
        {
            Console.WriteLine($"{Name} barks.");
        }
    }

    public class Cat : Animal
    {
        public Cat(string name, int age) : base(name, age) { }

        public override void Speak()
        {
            Console.WriteLine($"{Name} meows.");
        }
    }
}
