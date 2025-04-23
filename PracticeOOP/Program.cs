using System;

namespace PracticeOOP
{
    public class Animal
    {
        public string? Name { get; set; }

        public Animal()
        {
            Name = "Unknown";
        }

        public Animal(string name)
        {
            Name = name;
        }

        public Animal(Animal other)
        {
            Name = other.Name;
        }

        public virtual void Move()
        {
            Console.WriteLine("Animal is moving.");
        }
    }

    public class Dog : Animal
    {
        public Dog() : base()
        {
        }

        public Dog(string name) : base(name)
        {
        }

        public Dog(Dog other) : base(other)
        {
        }

        public override void Move()
        {
            Console.WriteLine("Dog is Moving.");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Animal a = new Animal("Animal");
            a.Move();

            Animal b = new Dog("Dog");
            b.Move();
        }
    }
}