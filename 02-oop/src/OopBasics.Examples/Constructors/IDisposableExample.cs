namespace OopBasics.Examples.Constructors
{
    /// <summary>
    /// Demonstrates:
    /// - IDisposable pattern
    /// - Deterministic resource cleanup
    /// - using statement
    /// </summary>
    public class IDisposableExample
    {
        public static void Run()
        {
            Console.WriteLine("IDisposableExample: Deterministic resource management");

            using (var resource = new FileResource())
            {
                resource.Use();
            }

            Console.WriteLine("Exited using block.");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- IDisposable allows explicit resource cleanup.");
            Console.WriteLine("- 'using' ensures Dispose() is called automatically.");
            Console.WriteLine("- Preferred over destructors for resource management.");
        }
    }

    /// <summary>
    /// Simulates a resource that needs manual cleanup
    /// </summary>
    public class FileResource : IDisposable
    {
        private bool _disposed;

        public FileResource()
        {
            Console.WriteLine("Resource acquired.");
        }

        public void Use()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileResource));

            Console.WriteLine("Using resource...");
        }

        public void Dispose()
        {
            if (_disposed) return;

            Console.WriteLine("Resource disposed.");

            _disposed = true;

            // Suppress finalizer (if exists)
            GC.SuppressFinalize(this);
        }

        ~FileResource()
        {
            Console.WriteLine("Finalizer called (fallback cleanup).");
        }
    }
}
