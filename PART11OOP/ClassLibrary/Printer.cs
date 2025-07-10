using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Printer
    {
        private string _message;

        public required int Page { get; set; }

        public Printer()
        {
            _message = string.Empty;
            Console.WriteLine("Printer instance created.");
        }

        public Printer(string message)
        {
            _message = message;
            Console.WriteLine($"Printer instance created with message: {message}");
        }

        public void Print(string message)
        {
            Console.WriteLine(message);
        }
    }
}
