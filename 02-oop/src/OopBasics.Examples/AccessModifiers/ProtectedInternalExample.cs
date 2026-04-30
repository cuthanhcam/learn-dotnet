namespace OopBasics.Examples.AccessModifiers
{
    /// <summary>
    /// Demonstrates:
    /// - protected internal vs private protected
    /// </summary>
    public class ProtectedInternalExample
    {
        public static void Run()
        {
            Console.WriteLine("ProtectedInternalExample: advanced access modifiers");

            var derived = new AdvancedDerived();
            derived.Test();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- protected internal: same assembly OR derived.");
            Console.WriteLine("- private protected: same assembly AND derived.");
        }
    }

    public class AdvancedBase
    {
        protected internal void ProtectedInternalMethod()
        {
            Console.WriteLine("ProtectedInternal method");
        }

        private protected void PrivateProtectedMethod()
        {
            Console.WriteLine("PrivateProtected method");
        }
    }

    public class AdvancedDerived : AdvancedBase
    {
        public void Test()
        {
            ProtectedInternalMethod(); // good
            PrivateProtectedMethod();  // good (same assembly + derived)
        }
    }
}
