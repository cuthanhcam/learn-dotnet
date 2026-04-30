namespace OopBasics.Examples.AccessModifiers
{
    /// <summary>
    /// Demonstrates:
    /// - internal access modifier
    /// - same assembly visibility
    /// </summary>
    public class InternalAccessExample
    {
        public static void Run()
        {
            Console.WriteLine("InternalAccessExample: same assembly access");

            var service = new InternalService();
            service.DoWork();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- internal: accessible only within the same assembly.");
            Console.WriteLine("- Common for infrastructure or helper classes.");
        }
    }

    internal class InternalService
    {
        public void DoWork()
        {
            Console.WriteLine("Internal service working...");
        }
    }
}
