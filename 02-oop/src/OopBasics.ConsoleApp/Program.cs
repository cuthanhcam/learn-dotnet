using OopBasics.Examples.Classes;

namespace OopBasics.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"{new string('=',20)} C# OOP Demo Runner {new string('=', 20)}");
            Console.WriteLine(new string('-', 60));

            RunSection("Classes & Objects", static () =>
            {
                ClassBasicsExample.Run();
            });
        }

        public static void RunSection(string title, Action action)
        {
            Console.WriteLine($"\n{new string('-', 20)} {title} {new string('-', 20)}");
            action();
        }
    }
}
