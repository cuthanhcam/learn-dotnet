using ClassLibrary;
using ClassLibrary.P;

namespace PART11OOP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var printer = new Printer("Test")
            //{
            //    Page = 100
            //};

            var laserPrinter = new LaserPrinter("Test2")
            {
                Page = 200
            };

            laserPrinter.Print("Hello, World!");
        }
    }
}
