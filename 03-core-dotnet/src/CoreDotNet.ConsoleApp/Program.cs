using CoreDotNet.Examples.Collections;

namespace CoreDotNet.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PrintHeader("C# Core Demo Runner");

            RunSection("Collections", static () =>
            {
                RunExample(CollectionsExample.Run);
            });

            PrintFooter();
        }

        // ===== Layout Helpers =====

        public static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 70));
            Console.WriteLine(title.ToUpper().PadLeft((70 + title.Length) / 2));
            Console.WriteLine(new string('=', 70));
            Console.WriteLine();
        }

        public static void PrintFooter()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 70));
            Console.WriteLine("END OF DEMO".PadLeft(40));
            Console.WriteLine(new string('=', 70));
            Console.WriteLine();
        }

        public static void RunSection(string title, Action action)
        {
            Console.WriteLine();
            Console.WriteLine(new string('-', 70));
            Console.WriteLine(title.ToUpper().PadLeft((70 + title.Length) / 2));
            Console.WriteLine(new string('-', 70));
            Console.WriteLine();

            action();

            Console.WriteLine(); // spacing after section
        }

        public static void RunExample(Action example)
        {
            example();

            // spacing between examples
            Console.WriteLine();
            Console.WriteLine(new string('.', 40));
            Console.WriteLine();
        }
    }
}
