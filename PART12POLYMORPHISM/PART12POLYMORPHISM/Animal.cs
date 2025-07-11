using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART12POLYMORPHISM
{
    internal class Animal
    {
        public virtual void Move()
        {
            Console.WriteLine("Animal is moving");
        }

        public void A()
        {
            Console.WriteLine("Animal A method called");
        }
    }
}
