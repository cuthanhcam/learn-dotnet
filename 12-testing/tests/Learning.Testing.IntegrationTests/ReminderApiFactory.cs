using System.Collections.Concurrent;
using Learning.Testing.Examples.Loans;
using Learning.Testing.Examples.Reminders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Learning.Testing.IntegrationTests;

public sealed class ReminderApiFactory : WebApplicationFactory<Program>
{
    public static readonly DateTimeOffset Now = new(2026, 10, 25, 0, 0, 0, TimeSpan.Zero);
    public CaptureSender Sender { get; } = new();
    public ILoanReader? ReaderOverride { get; init; }
    public Func<CancellationToken, Task>? SenderOverride { get; init; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<TimeProvider>();
            services.AddSingleton<TimeProvider>(new FixedClock());
            services.RemoveAll<IReminderSender>();
            Sender.Complete = SenderOverride ?? (_ => Task.CompletedTask);
            services.AddSingleton<IReminderSender>(Sender);
            if (ReaderOverride is not null)
            {
                services.RemoveAll<ILoanReader>();
                services.AddSingleton(ReaderOverride);
            }
        });
    }

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }
}

public sealed class CaptureSender : IReminderSender
{
    public ConcurrentQueue<LoanReminder> Attempts { get; } = new();
    internal Func<CancellationToken, Task> Complete { get; set; } = _ => Task.CompletedTask;

    public Task SendAsync(LoanReminder reminder, CancellationToken cancellationToken)
    {
        Attempts.Enqueue(reminder);
        return Complete(cancellationToken);
    }
}

internal sealed class FaultingLoanReader(Exception failure) : ILoanReader
{
    public Task<Loan?> FindAsync(Guid loanId, CancellationToken cancellationToken) => Task.FromException<Loan?>(failure);
}

internal sealed class BlockingLoanReader : ILoanReader
{
    public TaskCompletionSource Started { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource CancellationObserved { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public async Task<Loan?> FindAsync(Guid loanId, CancellationToken cancellationToken)
    {
        Started.TrySetResult();
        try
        {
            // An infinite cancellable wait models pending I/O; elapsed time is not an assertion.
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return null;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            CancellationObserved.TrySetResult();
            throw;
        }
    }
}
