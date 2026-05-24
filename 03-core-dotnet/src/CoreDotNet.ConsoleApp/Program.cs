using CoreDotNet.Examples.Collections;
using CoreDotNet.Examples.Generics;
using CoreDotNet.Examples.ExceptionHandling;
using CoreDotNet.Examples.LINQ;
using CoreDotNet.Examples.DelegatesAndEvents;
using CoreDotNet.Examples.FileIO;
using CoreDotNet.Examples.DateTimeAndTimeZone;
using CoreDotNet.Examples.Attributes;
using CoreDotNet.Examples.NullableReferenceTypes;

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

            RunSection("Generics", static () =>
            {
                RunExample(GenericsExample.Run);
            });

            RunSection("Exception Handling", static () =>
            {
                RunExample(ExceptionHandlingExample.Run);
            });

            RunSection("LINQ", static () =>
            {
                RunExample(LINQExample.Run);
            });

            RunSection("Delegates & Events", static () =>
            {
                RunExample(DelegatesAndEventsExample.Run);
            });

            RunSection("File I/O", static () =>
            {
                RunExample(FileIOExample.Run);
            });

            RunSection("DateTime & TimeZone", static () =>
            {
                RunExample(DateTimeAndTimeZoneExample.Run);
            });

            RunSection("Attributes", static () =>
            {
                RunExample(AttributesExample.Run);
            });

            RunSection("Nullable Reference Types", static () =>
            {
                RunExample(NullableReferenceTypesExample.Run);
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
