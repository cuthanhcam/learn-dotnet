using CSharpBasics.Examples.ControlFlow;
using CSharpBasics.Examples.Variables;
using CSharpBasics.Examples.Methods;
using CSharpBasics.Examples.Collections;
using CSharpBasics.Examples.Strings;
using CSharpBasics.Examples.Nullability;
using CSharpBasics.Examples.Memory;

namespace CSharpBasics.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PrintHeader("C# Fundamentals Demo Runner");

            RunSection("Variables & Types", static () =>
            {
                RunExample(VariablesExamples.Run);
                RunExample(DynamicVsTypedExample.Run);
            });

            RunSection("Control Flow", static () =>
            {
                RunExample(IfElseExample.Run);
                RunExample(SwitchExample.Run);
                RunExample(LoopsExample.Run);
            });

            RunSection("Methods", static () =>
            {
                RunExample(MethodBasicsExample.Run);
                RunExample(ParamModifiersExample.Run);
                RunExample(OverloadingExample.Run);
                RunExample(OptionalParametersExample.Run);
            });

            RunSection("Collections", static () =>
            {
                RunExample(ArraysExample.Run);
                RunExample(ListExample.Run);
                RunExample(DictionaryExample.Run);
                RunExample(HashSetExample.Run);
                RunExample(EnumerableExample.Run);
            });

            RunSection("Strings", static () =>
            {
                RunExample(StringBasicsExample.Run);
                RunExample(StringBuilderExample.Run);
                RunExample(StringMethodsExample.Run);
                RunExample(StringPerformanceExample.Run);
            });

            RunSection("Nullability", static () =>
            {
                RunExample(NullabilityExample.Run);
            });

            RunSection("Memory Concepts", static () =>
            {
                RunExample(MemoryConceptsExample.Run);
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

            Console.WriteLine();
            Console.WriteLine(new string('.', 40));
            Console.WriteLine();
        }
    }
}
