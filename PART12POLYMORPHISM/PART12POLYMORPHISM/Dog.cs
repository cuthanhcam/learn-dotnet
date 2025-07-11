using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART12POLYMORPHISM
{
    internal class Dog : Animal
    {
        public override void Move()
        {
            Console.WriteLine("Dog is running");
        }
    }
}
