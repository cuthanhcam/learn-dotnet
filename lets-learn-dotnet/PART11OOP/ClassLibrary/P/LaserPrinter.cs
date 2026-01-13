using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.P
{
    public class LaserPrinter : Printer
    {
        public int Resolution { get; set; } = 300;

        public LaserPrinter()
        {
            Console.WriteLine("LaserPrinter instance created.");
        }

        public LaserPrinter(string message) : base(message)
        {
            Console.WriteLine($"LaserPrinter instance created with message: {message}");
        }

        public override void MyAbstractMethod()
        {
            
        }
    }
}
