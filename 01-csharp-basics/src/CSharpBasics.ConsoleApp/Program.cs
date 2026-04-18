using CSharpBasics.Examples.ControlFlow;
using CSharpBasics.Examples.Variables;
using CSharpBasics.Examples.Methods;
using CSharpBasics.Examples.Collections;
using CSharpBasics.Examples.Strings;

namespace CSharpBasics.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("C# Fundamentals Demo Runner");
            Console.WriteLine(new string('-', 60));

            RunSection("Variables & Types", static () =>
            {
                VariablesExamples.Run();
                DynamicVsTypedExample.Run();
            });

            RunSection("Control Flow", static () =>
            {
                IfElseExample.Run();
                SwitchExample.Run();
                LoopsExample.Run();
            });

            RunSection("Methods", static () =>
            {
                MethodBasicsExample.Run();
                ParamModifiersExample.Run();
                OverloadingExample.Run();
                OptionalParametersExample.Run();
            });
            
            RunSection("Collections", static () =>
            {
                ArraysExample.Run();
                ListExample.Run();
                DictionaryExample.Run();
                HashSetExample.Run();
                EnumerableExample.Run();
            });

            RunSection("Strings", static () =>
            {
                StringBasicsExample.Run();
                StringBuilderExample.Run();
            });
        }
        
        public static void RunSection(string title, Action action)
        {
            Console.WriteLine($"\n{new string('-', 20)} {title} {new string('-', 20)}");
            action();
        }
    }
}

