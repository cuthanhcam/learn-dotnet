using System.ComponentModel.DataAnnotations;

namespace Learning.Api.BackgroundJobs;

public sealed class SubmitBackgroundJobRequest
{
    [Required, StringLength(200, MinimumLength = 3)]
    public string? Description { get; init; }
}

public sealed record BackgroundJob(
    Guid Id,
    string Description,
    DateTimeOffset SubmittedAt);

public sealed record BackgroundJobState(
    Guid Id,
    string Description,
    BackgroundJobStatus Status,
    DateTimeOffset SubmittedAt,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? CompletedAt = null,
    string? ErrorCode = null);

public enum BackgroundJobStatus
{
    Queued,
    Running,
    Completed,
    Failed
}
