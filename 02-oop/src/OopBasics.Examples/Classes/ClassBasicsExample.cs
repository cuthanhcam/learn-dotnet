using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OopBasics.Examples.Classes
{
    public class ClassBasicsExample
    {
        public static void Run()
        {
            Console.WriteLine("ClassBasicExample: Creating and using a Person Object");
            var person = new Person("Charlie", 22);
            Console.WriteLine($"Created person: {person}");
            person.Birthday();
            Console.WriteLine($"After birthday: {person}");
            Console.WriteLine("Tip: Use ToString() for readable output and encapsulate behavior in methods.");
        }
    }

    /// <summary>
    /// Represents a person with a name and age, demonstrating basic class structure, properties, and methods in C#.
    /// </summary>
    public class Person
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public Person(string name, int age)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Name cannot be null or empty.");
            }

            if (age < 0)
            {
                throw new ArgumentOutOfRangeException("Age cannot be negative.");
            }

            Name = name;
            Age = age;
        }

        /// <summary>
        /// Increments the person's age by one year, simulating a birthday.
        /// </summary>
        public void Birthday()
        {
            Age++;
        }

        public override string ToString() => $"Person(Name: {Name}, Age: {Age})";
    }
}
