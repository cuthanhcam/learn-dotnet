namespace OopBasics.Examples.Classes
{
    /// <summary>
    /// Demonstrates:
    /// - Encapsulation
    /// - Protecting internal state
    /// - Behavior-driven design
    /// </summary>
    public class EncapsulationExample
    {
        public static void Run()
        {
            Console.WriteLine("EncapsulationExample: Protecting internal state with methods");

            var account = new BankAccount("Alice", 1000);

            Console.WriteLine(account);

            account.Deposit(500);
            Console.WriteLine($"After deposit: {account}");

            account.Withdraw(300);
            Console.WriteLine($"After withdrawal: {account}");

            try
            {
                account.Withdraw(5000); // invalid
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nNotes:");
            Console.WriteLine("- Encapsulation hides internal state.");
            Console.WriteLine("- State changes should go through methods.");
            Console.WriteLine("- Prevents invalid operations.");
        }
    }

    public class BankAccount
    {
        public string Owner { get; private set; }
        public decimal Balance { get; private set; }

        public BankAccount(string owner, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner cannot be empty.");

            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.");

            Owner = owner;
            Balance = initialBalance;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive.");

            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds.");

            Balance -= amount;
        }

        public override string ToString()
        {
            return $"Account(Owner: {Owner}, Balance: {Balance})";
        }
    }
}
