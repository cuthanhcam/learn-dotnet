using System.Collections.Concurrent;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Learning.Persistence.Tests.Infrastructure;

public sealed class CommandCaptureInterceptor : DbCommandInterceptor
{
    private readonly ConcurrentQueue<string> _commands = new();
    private readonly ConcurrentQueue<CommandSnapshot> _snapshots = new();

    public IReadOnlyList<string> Commands => _commands.ToArray();
    public IReadOnlyList<CommandSnapshot> Snapshots => _snapshots.ToArray();

    private void Capture(DbCommand command)
    {
        _commands.Enqueue(command.CommandText);
        _snapshots.Enqueue(new CommandSnapshot(
            command.CommandText,
            command.Parameters.Cast<DbParameter>()
                .Select(parameter => new CommandParameterSnapshot(parameter.ParameterName, parameter.Value))
                .ToArray()));
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        Capture(command);
        return result;
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        Capture(command);
        return ValueTask.FromResult(result);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        Capture(command);
        return result;
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        Capture(command);
        return ValueTask.FromResult(result);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        Capture(command);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Capture(command);
        return ValueTask.FromResult(result);
    }
}

public sealed record CommandSnapshot(
    string CommandText,
    IReadOnlyList<CommandParameterSnapshot> Parameters);

public sealed record CommandParameterSnapshot(string Name, object? Value);
