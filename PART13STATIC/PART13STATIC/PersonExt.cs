using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART13STATIC
{
    internal static class PersonExt
    {
        public static void Print(this Person person)
        {
            Console.WriteLine($"Id: {person.Id}, Name: {person.Name}");
        }
    }
}
