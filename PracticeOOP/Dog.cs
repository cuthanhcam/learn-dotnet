using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeOOP
{
    public class Dog : Animal
    {
        public string Breed { get; set; }

        public Dog() : base()
        {
            Breed = "Unknown";
        }

        public Dog(string name, string breed) : base(name)
        {
            Name = name;
            Breed = breed;
        }

        public Dog(Dog dog) : base(dog)
        {
            Breed = dog.Breed;
        }

        public override void Speak()
        {
            Console.WriteLine("Woof! Woof!");
        }
    }
}
