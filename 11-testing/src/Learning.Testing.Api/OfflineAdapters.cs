using Learning.Testing.Examples.Loans;
using Learning.Testing.Examples.Reminders;

namespace Learning.Testing.Api;

public sealed record ReminderResponse(Guid LoanId, string Outcome);

public sealed class OfflineLoanReader : ILoanReader
{
    public static readonly Guid DueLoanId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    public static readonly Guid FutureLoanId = Guid.Parse("20000000-0000-0000-0000-000000000002");
    public static readonly Guid ReturnedLoanId = Guid.Parse("20000000-0000-0000-0000-000000000003");
    private static readonly Guid MemberId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private readonly Dictionary<Guid, Loan> _loans = new()
    {
        [DueLoanId] = new Loan(DueLoanId, MemberId, DateTimeOffset.UnixEpoch),
        [FutureLoanId] = new Loan(FutureLoanId, MemberId, DateTimeOffset.MaxValue),
        [ReturnedLoanId] = new Loan(ReturnedLoanId, MemberId, DateTimeOffset.UnixEpoch, isReturned: true)
    };

    public Task<Loan?> FindAsync(Guid loanId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_loans.GetValueOrDefault(loanId));
    }
}

// This is an offline side-effect boundary: a log entry is not an email or a durable receipt.
internal sealed class LoggingReminderSender(ILogger<LoggingReminderSender> logger) : IReminderSender
{
    public Task SendAsync(LoanReminder reminder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        logger.LogInformation("Offline reminder recorded for loan {LoanId}", reminder.LoanId);
        return Task.CompletedTask;
    }
}
