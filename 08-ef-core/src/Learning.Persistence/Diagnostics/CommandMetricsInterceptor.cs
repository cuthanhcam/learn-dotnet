using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Learning.Persistence.Diagnostics;

/// <summary>
/// Converts EF command lifecycle callbacks into safe application metrics. The interceptor keeps no
/// mutable per-command state, so one instance can safely serve concurrent contexts.
/// </summary>
public sealed class CommandMetricsInterceptor(ICommandObservationSink sink) : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        Complete(eventData, succeeded: true, errorType: null);
        return result;
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        Complete(eventData, succeeded: true, errorType: null);
        return ValueTask.FromResult(result);
    }

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        Complete(eventData, succeeded: true, errorType: null);
        return result;
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        Complete(eventData, succeeded: true, errorType: null);
        return ValueTask.FromResult(result);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        Complete(eventData, succeeded: true, errorType: null);
        return result;
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        Complete(eventData, succeeded: true, errorType: null);
        return ValueTask.FromResult(result);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData) =>
        Complete(eventData, succeeded: false, eventData.Exception.GetType().Name);

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        Complete(eventData, succeeded: false, eventData.Exception.GetType().Name);
        return Task.CompletedTask;
    }

    private void Complete(CommandEndEventData eventData, bool succeeded, string? errorType)
    {
        sink.Record(new CommandObservation(
            Operation: eventData.ExecuteMethod.ToString(),
            Duration: eventData.Duration,
            Succeeded: succeeded,
            ErrorType: errorType));
    }
}
