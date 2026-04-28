namespace OopBasics.Examples.Inheritance
{
    /// <summary>
    /// Demonstrates:
    /// - sealed classes
    /// - override vs sealing methods
    /// </summary>
    public class SealedAndOverrideExample
    {
        public static void Run()
        {
            Console.WriteLine("SealedExample: Preventing further inheritance");

            var processor = new PaymentProcessor();
            processor.Process();

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- 'sealed' prevents inheritance.");
            Console.WriteLine("- Use it to protect critical logic.");
            Console.WriteLine("- Helps avoid unintended extension.");
        }
    }

    public class BaseProcessor
    {
        public virtual void Process()
        {
            Console.WriteLine("Base processing...");
        }
    }

    public sealed class PaymentProcessor : BaseProcessor
    {
        public override void Process()
        {
            Console.WriteLine("Processing payment securely...");
        }
    }
}
