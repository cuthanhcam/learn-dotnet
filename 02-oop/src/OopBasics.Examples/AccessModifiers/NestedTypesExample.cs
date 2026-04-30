namespace OopBasics.Examples.AccessModifiers
{
    /// <summary>
    /// Demonstrates:
    /// - Access modifiers in nested types
    /// </summary>
    public class NestedTypesExample
    {
        public static void Run()
        {
            Console.WriteLine("NestedTypesExample: nested class access");

            var outer = new Outer();
            outer.UseInner();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Nested types can access private members of outer class.");
            Console.WriteLine("- Useful for tightly coupled helper logic.");
        }
    }

    public class Outer
    {
        private string secret = "Hidden Data";

        public void UseInner()
        {
            var inner = new Inner(this);
            inner.ShowSecret();
        }

        private class Inner
        {
            private readonly Outer _outer;

            public Inner(Outer outer)
            {
                _outer = outer;
            }

            public void ShowSecret()
            {
                Console.WriteLine($"Accessing: {_outer.secret}");
            }
        }
    }
}
