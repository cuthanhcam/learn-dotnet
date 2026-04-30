namespace OopBasics.Examples.AccessModifiers
{
    /// <summary>
    /// Demonstrates:
    /// - private vs protected
    /// - Access in derived classes
    /// </summary>
    public class InheritanceAccessExample
    {
        public static void Run()
        {
            Console.WriteLine("InheritanceAccessExample: protected vs private");

            var child = new Child();
            child.TestAccess();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- private: NOT accessible in derived classes.");
            Console.WriteLine("- protected: accessible in derived classes.");
        }
    }

    public class Parent
    {
        private void PrivateMethod()
        {
            Console.WriteLine("Private method");
        }

        protected void ProtectedMethod()
        {
            Console.WriteLine("Protected method");
        }
    }

    public class Child : Parent
    {
        public void TestAccess()
        {
            ProtectedMethod(); // allowed

            // not allowed:
            // PrivateMethod();
        }
    }
}
