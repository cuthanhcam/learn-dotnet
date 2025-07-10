using ClassLibrary;
using ClassLibrary.P;

namespace PART11OOP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var printer = new Printer("Cam")
            {
                Page = 100
            };

            var laserPrinter = new LaserPrinter()
            {
                Page = 200
            };

            printer.Print("Hello, World!");
        }
    }
}
