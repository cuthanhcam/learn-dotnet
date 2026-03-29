using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART13STATIC
{
    internal class AccessCounter
    {
        private int counter = 0;
        private static AccessCounter accessCounter = new();

        public int Counter { get; }
        public int Inc()
        {
            counter++;
            return counter;
        }

        public static AccessCounter GetInstance()
        {
            return accessCounter; ;
        }
    }
}
