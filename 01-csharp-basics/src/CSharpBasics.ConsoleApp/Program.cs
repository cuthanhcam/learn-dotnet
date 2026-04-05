using CSharpBasics.Examples.ControlFlow;
using CSharpBasics.Examples.Variables;

namespace CSharpBasics.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("C# Fundamentasl Demo Runner");
            Console.WriteLine(new string('-', 60));

            RunSection("Variables & Types", static () =>
            {
                VariablesExamples.Run();
                DynamicVsTypedExample.Run();
            });

            RunSection("Control Flow", static () =>
            {
                IfElseExample.Run();
            });
        }
        
        public static void RunSection(string title, Action action)
        {
            Console.WriteLine($"\n{new string('-', 20)} {title} {new string('-', 20)}");
            action();
        }
    }
}

