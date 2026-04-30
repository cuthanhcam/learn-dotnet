namespace OopBasics.Exercises;

public static class InheritanceExercises
{
    public abstract class Animal
    {
        public string Name { get; }
        public int Age { get; }

        protected Animal(string name, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty.");
            if (age < 0)
                throw new ArgumentOutOfRangeException(nameof(age), "Age cannot be negative.");
            Name = name;
            Age = age;
        }

        public abstract string Speak();
    }

    public class Dog : Animal
    {
        public Dog(string name, int age) : base(name, age) { }
        public override string Speak() => $"{Name} barks.";
    }

    public class Cat : Animal
    {
        public Cat(string name, int age) : base(name, age) { }
        public override string Speak() => $"{Name} meows.";
    }
}
