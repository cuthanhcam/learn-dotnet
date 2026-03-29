using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART12POLYMORPHISM
{
    internal class Fish : Animal
    {
        public sealed override void Move()
        {
            Console.WriteLine("Fish is swimming");
        }
    }
}
