namespace Learning.Testing.Examples.Loans;

public static class ReminderPolicy
{
    public static bool IsDue(Loan loan, DateTimeOffset asOf)
    {
        ArgumentNullException.ThrowIfNull(loan);

        // Equality belongs to the overdue interval. DateTimeOffset compares instants, not wall-clock labels.
        return !loan.IsReturned && asOf >= loan.DueAt;
    }
}
