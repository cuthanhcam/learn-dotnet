using OopBasics.Examples.AccessModifiers;
using OopBasics.Examples.Classes;
using OopBasics.Examples.Constructors;
using OopBasics.Examples.Inheritance;
using OopBasics.Examples.Polymorphism;

namespace OopBasics.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PrintHeader("C# OOP Demo Runner");

            RunSection("Classes & Objects", static () =>
            {
                RunExample(ClassBasicsExample.Run);
                RunExample(PropertiesExample.Run);
                RunExample(ObjectInitializerExample.Run);
                RunExample(EncapsulationExample.Run);
                RunExample(ImmutableObjectExample.Run);
                RunExample(ValueObjectExample.Run);
                
            });

            RunSection("Inheritance", static () => 
            {
                RunExample(InheritanceExample.Run);
                RunExample(BaseConstructorExample.Run);
                RunExample(SealedAndOverrideExample.Run);
            });

            RunSection("Polymorphism", static () => 
            {
                RunExample(InterfaceExample.Run);
                RunExample(AbstractClassExample.Run);
                RunExample(VirtualOverrideExample.Run);
            });

            RunSection("Access Modifiers", static () => 
            {
                RunExample(AccessModifiersExample.Run);
                RunExample(InheritanceAccessExample.Run);
                RunExample(InternalAccessExample.Run);
                RunExample(ProtectedInternalExample.Run);
                RunExample(NestedTypesExample.Run);
            });

            RunSection("Constructors & Destructors", static () => 
            {
                RunExample(ConstructorsExample.Run);
                RunExample(DestructorExample.Run);
                RunExample(IDisposableExample.Run);
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
