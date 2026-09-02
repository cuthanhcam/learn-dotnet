namespace Learning.Auth.Application.Abstractions;

/// <summary>
/// Receives a closed, non-secret security-event vocabulary. Implementations must keep credentials,
/// tokens, user-supplied identity text, and high-cardinality metric labels out of telemetry.
/// </summary>
public interface ISecurityEventSink
{
    ValueTask WriteAsync(SecurityEvent securityEvent, CancellationToken cancellationToken);
}

public sealed record SecurityEvent(
    SecurityEventType Type,
    DateTimeOffset OccurredAt,
    Guid? SubjectId = null);

public enum SecurityEventType
{
    SignInSucceeded,
    SignInRejected,
    AccountLockoutStarted,
    RefreshRotated,
    RefreshRejected,
    RefreshReplayDetected,
    SessionRevoked
}
