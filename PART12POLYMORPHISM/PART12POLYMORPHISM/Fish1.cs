using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART12POLYMORPHISM
{
    internal class Fish1 : Dog // Inheriting from Dog to demonstrate sealed method behavior
    {
        // This method cannot be overridden because Fish.Move() is sealed
        public override void Move()
        {
            base.Move();
        }
    }
}
