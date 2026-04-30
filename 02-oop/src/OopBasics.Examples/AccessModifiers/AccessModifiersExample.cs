namespace OopBasics.Examples.AccessModifiers
{
    /// <summary>
    /// Overview of access modifiers in C#
    /// </summary>
    public class AccessModifiersExample
    {
        public static void Run()
        {
            Console.WriteLine("AccessModifiersExample: Overview");

            var demo = new DemoClass();
            demo.PublicMethod();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Access modifiers control visibility.");
            Console.WriteLine("- They enforce encapsulation.");
            Console.WriteLine("- Important for API design and maintainability.");
        }
    }

    public class DemoClass
    {
        public void PublicMethod()
        {
            Console.WriteLine("Public method called.");
        }

        private void PrivateMethod()
        {
            Console.WriteLine("Private method (not accessible outside).");
        }
    }
}
