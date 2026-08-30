namespace Learning.Persistence.Diagnostics;

/// <summary>
/// A deliberately low-cardinality diagnostic event. SQL text and parameter values are excluded so
/// production telemetry does not become a data-exfiltration path or create one metric series per query.
/// </summary>
public sealed record CommandObservation(
    string Operation,
    TimeSpan Duration,
    bool Succeeded,
    string? ErrorType);

public interface ICommandObservationSink
{
    void Record(CommandObservation observation);
}
