namespace OopBasics.Exercises;

public static class EncapsulationExercises
{
    public class BankAccount
    {
        private decimal _balance;
        public string Owner { get; }

        public BankAccount(string owner, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("Owner cannot be empty.");
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");
            Owner = owner;
            _balance = initialBalance;
        }

        public decimal Balance => _balance;

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be positive.");
            _balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Withdraw must be positive.");
            if (amount > _balance)
                throw new InvalidOperationException("Insufficient funds.");
            _balance -= amount;
        }
    }
}
