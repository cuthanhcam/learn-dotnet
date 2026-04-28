namespace OopBasics.Examples.Polymorphism
{
    /// <summary>
    /// Demonstrates:
    /// - Abstract classes
    /// - Shared logic + abstract methods
    /// </summary>
    public class AbstractClassExample
    {
        public static void Run()
        {
            Console.WriteLine("AbstractClassExample: Shared logic with abstraction");

            Notification email = new EmailNotification();
            Notification sms = new SmsNotification();

            email.Send("Hello via Email");
            sms.Send("Hello via SMS");

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Abstract classes provide shared behavior.");
            Console.WriteLine("- Derived classes must implement abstract methods.");
            Console.WriteLine("- Useful when classes share common logic.");
        }
    }

    public abstract class Notification
    {
        public void Send(string message)
        {
            Log(message);
            Deliver(message);
        }

        protected abstract void Deliver(string message);

        protected void Log(string message)
        {
            Console.WriteLine($"[LOG]: Sending message: {message}");
        }
    }

    public class EmailNotification : Notification
    {
        protected override void Deliver(string message)
        {
            Console.WriteLine($"Email sent: {message}");
        }
    }

    public class SmsNotification : Notification
    {
        protected override void Deliver(string message)
        {
            Console.WriteLine($"SMS sent: {message}");
        }
    }
}
