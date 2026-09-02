using Learning.Auth.Application.Abstractions;

namespace Learning.Auth.Api.Security;

/// <summary>
/// Writes controlled security facts to structured application logs. The event contract contains no
/// raw email, credential, token, header, or attacker-controlled message, preventing accidental secret
/// disclosure and log-forging through this boundary.
/// </summary>
public sealed partial class StructuredSecurityEventSink(ILogger<StructuredSecurityEventSink> logger)
    : ISecurityEventSink
{
    public ValueTask WriteAsync(SecurityEvent securityEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        LogSecurityEvent(logger, securityEvent.Type, securityEvent.SubjectId, securityEvent.OccurredAt);
        return ValueTask.CompletedTask;
    }

    [LoggerMessage(
        EventId = 9001,
        Level = LogLevel.Information,
        Message = "Security event {SecurityEventType}; subject {SubjectId}; occurred at {OccurredAt}")]
    private static partial void LogSecurityEvent(
        ILogger logger,
        SecurityEventType securityEventType,
        Guid? subjectId,
        DateTimeOffset occurredAt);
}
