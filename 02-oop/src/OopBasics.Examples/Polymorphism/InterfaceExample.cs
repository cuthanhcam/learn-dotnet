namespace OopBasics.Examples.Polymorphism
{
    /// <summary>
    /// Demonstrates:
    /// - Interfaces
    /// - Multiple implementations
    /// - Polymorphic behavior
    /// </summary>
    public class InterfaceExample
    {
        public static void Run()
        {
            Console.WriteLine("InterfaceExample: Polymorphism via interface");

            IPaymentService[] services =
            {
                new CreditCardPayment(),
                new PaypalPayment()
            };

            foreach (var service in services)
            {
                service.Pay(100);
            }

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Interfaces define contracts.");
            Console.WriteLine("- Multiple classes can implement the same interface.");
            Console.WriteLine("- Enables flexible and decoupled design.");
        }
    }

    public interface IPaymentService
    {
        void Pay(decimal amount);
    }

    public class CreditCardPayment : IPaymentService
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Paid {amount} using Credit Card.");
        }
    }

    public class PaypalPayment : IPaymentService
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Paid {amount} using PayPal.");
        }
    }
}
