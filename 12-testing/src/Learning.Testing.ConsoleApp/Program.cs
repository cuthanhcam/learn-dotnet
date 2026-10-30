using Learning.Testing.Examples.Loans;
using Learning.Testing.Examples.Reminders;

DateTimeOffset now = new(2026, 10, 25, 0, 0, 0, TimeSpan.Zero);
Guid member = Guid.Parse("10000000-0000-0000-0000-000000000001");
Loan[] loans = [
    new(Guid.Parse("20000000-0000-0000-0000-000000000001"), member, now),
    new(Guid.Parse("20000000-0000-0000-0000-000000000002"), member, now.AddDays(1)),
    new(Guid.Parse("20000000-0000-0000-0000-000000000003"), member, now.AddDays(-1), isReturned: true)
];
var service = new LoanReminderService(new DemoLoanReader(loans), new ConsoleReminderSender(), new DemoClock(now));
Console.WriteLine("Loan reminder demo (fixed clock, no real messages)");
foreach (Loan loan in loans)
    Console.WriteLine($"{loan.Id}: {await service.SendAsync(loan.Id)}");
Console.WriteLine($"Missing loan: {await service.SendAsync(Guid.Parse("20000000-0000-0000-0000-000000000004"))}");

internal sealed class DemoClock(DateTimeOffset now) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => now;
}

internal sealed class DemoLoanReader(IEnumerable<Loan> loans) : ILoanReader
{
    private readonly Dictionary<Guid, Loan> _loans = loans.ToDictionary(loan => loan.Id);

    public Task<Loan?> FindAsync(Guid loanId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_loans.GetValueOrDefault(loanId));
    }
}

internal sealed class ConsoleReminderSender : IReminderSender
{
    public Task SendAsync(LoanReminder reminder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine($"Recorded reminder at {reminder.EvaluatedAt:O}");
        return Task.CompletedTask;
    }
}
