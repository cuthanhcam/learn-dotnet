using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART12POLYMORPHISM
{
    internal class Bird : Animal
    {
        public override void Move()
        {
            Console.WriteLine("Bird is flying");
        }

        public new void A() // This method is not overridden, so it will call the base class method
        {
            Console.WriteLine("Bird A method called");
        }
    }
}
