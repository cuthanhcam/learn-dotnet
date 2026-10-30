using Learning.Testing.Examples.Loans;

namespace Learning.Testing.Examples.Reminders;

public sealed class LoanReminderService
{
    private readonly ILoanReader _loans;
    private readonly IReminderSender _sender;
    private readonly TimeProvider _clock;

    public LoanReminderService(ILoanReader loans, IReminderSender sender, TimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(loans);
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(clock);
        _loans = loans;
        _sender = sender;
        _clock = clock;
    }

    public async Task<ReminderOutcome> SendAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        if (loanId == Guid.Empty)
            throw new ArgumentException("A loan ID is required.", nameof(loanId));

        cancellationToken.ThrowIfCancellationRequested();
        Loan? loan = await _loans.FindAsync(loanId, cancellationToken);
        // A port can finish without observing cancellation. Do not start the next side effect afterward.
        cancellationToken.ThrowIfCancellationRequested();
        if (loan is null)
            return ReminderOutcome.NotFound;

        DateTimeOffset evaluatedAt = _clock.GetUtcNow();
        if (!ReminderPolicy.IsDue(loan, evaluatedAt))
            return ReminderOutcome.NotRequired;

        await _sender.SendAsync(new LoanReminder(loan.Id, loan.MemberId, loan.DueAt, evaluatedAt), cancellationToken);
        return ReminderOutcome.Sent;
    }
}
