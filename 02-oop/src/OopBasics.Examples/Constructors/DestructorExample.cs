namespace OopBasics.Examples.Constructors
{
    /// <summary>
    /// Demonstrates:
    /// - Destructor (finalizer)
    /// - Garbage collection behavior
    /// - Why destructors are rarely used
    /// </summary>
    public class DestructorExample
    {
        public static void Run()
        {
            Console.WriteLine("DestructorExample: Finalizer behavior");

            CreateAndRelease();

            // Force GC for demo (not recommended in real apps)
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Garbage collection completed.");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Destructors are called by the GC, not manually.");
            Console.WriteLine("- Execution time is NOT deterministic.");
            Console.WriteLine("- Prefer IDisposable for resource management.");
        }

        private static void CreateAndRelease()
        {
            var resource = new ResourceHolder();
            Console.WriteLine("Resource created.");
        }
    }

    public class ResourceHolder
    {
        public ResourceHolder()
        {
            Console.WriteLine("Constructor: Resource acquired.");
        }

        ~ResourceHolder()
        {
            Console.WriteLine("Destructor: Resource cleaned up.");
        }
    }
}
