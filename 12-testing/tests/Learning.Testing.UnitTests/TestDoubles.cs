using Learning.Testing.Examples.Loans;
using Learning.Testing.Examples.Reminders;

namespace Learning.Testing.UnitTests;

// This clock controls GetUtcNow only. It does not implement virtual timers or Task.Delay.
internal sealed class FixedClock(DateTimeOffset now) : TimeProvider
{
    public int Reads { get; private set; }

    public override DateTimeOffset GetUtcNow()
    {
        Reads++;
        return now;
    }
}

internal sealed class StubLoanReader(Func<Guid, CancellationToken, Task<Loan?>> find) : ILoanReader
{
    public List<(Guid Id, CancellationToken Token)> Calls { get; } = [];

    public Task<Loan?> FindAsync(Guid loanId, CancellationToken cancellationToken)
    {
        Calls.Add((loanId, cancellationToken));
        return find(loanId, cancellationToken);
    }
}

internal sealed class RecordingSender : IReminderSender
{
    public List<(LoanReminder Reminder, CancellationToken Token)> Calls { get; } = [];
    public Func<CancellationToken, Task> Complete { get; init; } = _ => Task.CompletedTask;

    public Task SendAsync(LoanReminder reminder, CancellationToken cancellationToken)
    {
        Calls.Add((reminder, cancellationToken));
        return Complete(cancellationToken);
    }
}
