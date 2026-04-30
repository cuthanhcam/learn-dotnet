namespace OopBasics.Examples.AccessModifiers
{
    /// <summary>
    /// Demonstrates:
    /// - public, private, protected
    /// - internal, protected internal, private protected
    /// - Accessibility in inheritance
    /// </summary>
    public class AccessModifiersExample
    {
        public static void Run()
        {
            Console.WriteLine("AccessModifiersExample: Understanding access levels");

            var derived = new DerivedClass();

            derived.PublicMethod();
            derived.AccessProtected();

            var sameAssembly = new InternalExample();
            sameAssembly.Show();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- public: accessible everywhere.");
            Console.WriteLine("- private: accessible only inside the class.");
            Console.WriteLine("- protected: accessible in derived classes.");
            Console.WriteLine("- internal: accessible within the same assembly.");
            Console.WriteLine("- protected internal: same assembly OR derived class.");
            Console.WriteLine("- private protected: same assembly AND derived class.");
        }
    }

    /// <summary>
    /// Base class demonstrating all access modifiers
    /// </summary>
    public class BaseClass
    {
        public void PublicMethod()
        {
            Console.WriteLine("PublicMethod: Accessible everywhere");
        }

        private void PrivateMethod()
        {
            Console.WriteLine("PrivateMethod: Only inside BaseClass");
        }

        protected void ProtectedMethod()
        {
            Console.WriteLine("ProtectedMethod: Accessible in derived classes");
        }

        internal void InternalMethod()
        {
            Console.WriteLine("InternalMethod: Accessible within assembly");
        }

        protected internal void ProtectedInternalMethod()
        {
            Console.WriteLine("ProtectedInternalMethod: Same assembly OR derived");
        }

        private protected void PrivateProtectedMethod()
        {
            Console.WriteLine("PrivateProtectedMethod: Same assembly AND derived");
        }

        public void TestAccess()
        {
            PrivateMethod(); // accessible here
        }
    }

    /// <summary>
    /// Derived class demonstrating access to inherited members
    /// </summary>
    public class DerivedClass : BaseClass
    {
        public void AccessProtected()
        {
            ProtectedMethod();
            ProtectedInternalMethod();
            PrivateProtectedMethod();

            Console.WriteLine("Accessed protected members from DerivedClass");

            // ❌ Not allowed:
            // PrivateMethod(); // inaccessible
        }
    }

    /// <summary>
    /// Demonstrates internal access within same assembly
    /// </summary>
    internal class InternalExample
    {
        public void Show()
        {
            Console.WriteLine("InternalExample: Accessible within same assembly");
        }
    }
}
