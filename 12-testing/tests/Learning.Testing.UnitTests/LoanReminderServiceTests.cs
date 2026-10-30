using Learning.Testing.Examples.Loans;
using Learning.Testing.Examples.Reminders;

namespace Learning.Testing.UnitTests;

[Trait("Category", "Unit")]
public sealed class LoanReminderServiceTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 25, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task SendAsync_DueLoan_SendsMatchingPayloadWithOneTimeSnapshot()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Now);
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(loan));
        var sender = new RecordingSender();
        var clock = new FixedClock(Now);
        var service = new LoanReminderService(reader, sender, clock);
        using var cancellation = new CancellationTokenSource();

        ReminderOutcome outcome = await service.SendAsync(loan.Id, cancellation.Token);

        Assert.Equal(ReminderOutcome.Sent, outcome);
        Assert.Equal((loan.Id, cancellation.Token), Assert.Single(reader.Calls));
        var sent = Assert.Single(sender.Calls);
        Assert.Equal(new LoanReminder(loan.Id, loan.MemberId, loan.DueAt, Now), sent.Reminder);
        Assert.Equal(cancellation.Token, sent.Token);
        Assert.Equal(1, clock.Reads);
    }

    [Fact]
    public async Task SendAsync_MissingLoan_DoesNotReadClockOrSend()
    {
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(null));
        var sender = new RecordingSender();
        var clock = new FixedClock(Now);

        var outcome = await new LoanReminderService(reader, sender, clock).SendAsync(Guid.NewGuid());

        Assert.Equal(ReminderOutcome.NotFound, outcome);
        Assert.Empty(sender.Calls);
        Assert.Equal(0, clock.Reads);
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, -1)]
    public async Task SendAsync_LoanDoesNotNeedReminder_DoesNotSend(bool returned, int dueInDays)
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Now.AddDays(dueInDays), returned);
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(loan));
        var sender = new RecordingSender();

        var outcome = await new LoanReminderService(reader, sender, new FixedClock(Now)).SendAsync(loan.Id);

        Assert.Equal(ReminderOutcome.NotRequired, outcome);
        Assert.Empty(sender.Calls);
    }

    [Fact]
    public async Task SendAsync_EmptyId_RejectsBeforeReading()
    {
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(null));
        var sender = new RecordingSender();
        var service = new LoanReminderService(reader, sender, new FixedClock(Now));

        var exception = await Assert.ThrowsAsync<ArgumentException>(() => service.SendAsync(Guid.Empty));

        Assert.Equal("loanId", exception.ParamName);
        Assert.Empty(reader.Calls);
        Assert.Empty(sender.Calls);
    }

    [Fact]
    public async Task SendAsync_PreCanceled_DoesNotStartReadOrSend()
    {
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(null));
        var sender = new RecordingSender();
        var service = new LoanReminderService(reader, sender, new FixedClock(Now));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.SendAsync(Guid.NewGuid(), cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Empty(reader.Calls);
        Assert.Empty(sender.Calls);
    }

    [Fact]
    public async Task SendAsync_CanceledDuringRead_DoesNotStartSenderEvenIfReaderIgnoresToken()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Now);
        using var cancellation = new CancellationTokenSource();
        var reader = new StubLoanReader((_, _) =>
        {
            cancellation.Cancel();
            return Task.FromResult<Loan?>(loan);
        });
        var sender = new RecordingSender();
        var clock = new FixedClock(Now);
        var service = new LoanReminderService(reader, sender, clock);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => service.SendAsync(loan.Id, cancellation.Token));

        Assert.Empty(sender.Calls);
        Assert.Equal(0, clock.Reads);
    }

    [Fact]
    public async Task SendAsync_ReadFailure_PropagatesWithoutSending()
    {
        var failure = new IOException("Reader unavailable.");
        var reader = new StubLoanReader((_, _) => Task.FromException<Loan?>(failure));
        var sender = new RecordingSender();
        var service = new LoanReminderService(reader, sender, new FixedClock(Now));

        var actual = await Assert.ThrowsAsync<IOException>(() => service.SendAsync(Guid.NewGuid()));

        Assert.Same(failure, actual);
        Assert.Empty(sender.Calls);
    }

    [Fact]
    public async Task SendAsync_SenderFailure_PropagatesWithoutRetrying()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Now);
        var failure = new IOException("Sender unavailable.");
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(loan));
        var sender = new RecordingSender { Complete = _ => Task.FromException(failure) };
        var service = new LoanReminderService(reader, sender, new FixedClock(Now));

        var actual = await Assert.ThrowsAsync<IOException>(() => service.SendAsync(loan.Id));

        Assert.Same(failure, actual);
        Assert.Single(sender.Calls);
    }

    [Fact]
    public async Task SendAsync_SenderCancels_PropagatesCallerToken()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Now);
        using var cancellation = new CancellationTokenSource();
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(loan));
        var sender = new RecordingSender
        {
            Complete = token =>
            {
                cancellation.Cancel();
                return Task.FromCanceled(token);
            }
        };
        var service = new LoanReminderService(reader, sender, new FixedClock(Now));

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.SendAsync(loan.Id, cancellation.Token));

        Assert.Equal(cancellation.Token, exception.CancellationToken);
        Assert.Single(sender.Calls);
    }

    [Fact]
    public async Task SendAsync_PendingSender_DoesNotReportSentBeforeCompletion()
    {
        var loan = new Loan(Guid.NewGuid(), Guid.NewGuid(), Now);
        var reader = new StubLoanReader((_, _) => Task.FromResult<Loan?>(loan));
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var sender = new RecordingSender { Complete = _ => completion.Task };
        var service = new LoanReminderService(reader, sender, new FixedClock(Now));

        Task<ReminderOutcome> pending = service.SendAsync(loan.Id);
        Assert.False(pending.IsCompleted);
        completion.SetResult();

        Assert.Equal(ReminderOutcome.Sent, await pending.WaitAsync(TimeSpan.FromSeconds(5)));
    }
}
