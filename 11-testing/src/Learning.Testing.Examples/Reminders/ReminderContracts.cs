using Learning.Testing.Examples.Loans;

namespace Learning.Testing.Examples.Reminders;

public interface ILoanReader
{
    Task<Loan?> FindAsync(Guid loanId, CancellationToken cancellationToken);
}

public interface IReminderSender
{
    Task SendAsync(LoanReminder reminder, CancellationToken cancellationToken);
}

public sealed record LoanReminder(Guid LoanId, Guid MemberId, DateTimeOffset DueAt, DateTimeOffset EvaluatedAt);

public enum ReminderOutcome
{
    NotFound,
    NotRequired,
    Sent
}
