using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PART14Interface
{
    internal class ConsoleReadable : IDoubleReadable
    {
        public string Name => "ConsoleReadable";

        public double ReadDouble()
        {
            Console.Write("Input double: ");
            return double.Parse(Console.ReadLine() ?? "0");
        }

        public int ReadInt()
        {
            Console.Write("Input int: ");
            return int.Parse(Console.ReadLine() ?? "0");
        }

        public string ReadString()
        {
            Console.Write("Input string: ");
            return Console.ReadLine() ?? string.Empty;  
        }
    }
}
