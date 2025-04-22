using System;

namespace PracticeOOP
{
    public class Animal
    {
        public string Name { get; set; }

        public Animal()
        {
            Name = "Unknown";
        }

        public Animal(string name)
        {
            Name = name;
        }

        public Animal(Animal animal)
        {
            Name = animal.Name;
        }

        public virtual void Speak()
        {
            Console.WriteLine("Animal speaks!");
        }
    }
}

