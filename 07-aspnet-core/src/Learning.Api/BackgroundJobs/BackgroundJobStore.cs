using System.Collections.Concurrent;

namespace Learning.Api.BackgroundJobs;

public sealed class BackgroundJobStore(TimeProvider timeProvider)
{
    private readonly ConcurrentDictionary<Guid, BackgroundJobState> _states = new();

    public BackgroundJob Create(string description)
    {
        var job = new BackgroundJob(Guid.NewGuid(), description.Trim(), timeProvider.GetUtcNow());
        _states[job.Id] = new BackgroundJobState(
            job.Id, job.Description, BackgroundJobStatus.Queued, job.SubmittedAt);
        return job;
    }

    public BackgroundJobState? Find(Guid id) => _states.GetValueOrDefault(id);

    public void Remove(Guid id) => _states.TryRemove(id, out _);

    public void MarkRunning(BackgroundJob job) => _states.AddOrUpdate(
        job.Id,
        _ => throw new InvalidOperationException("A queued job state must exist."),
        (_, current) => current with
        {
            Status = BackgroundJobStatus.Running,
            StartedAt = timeProvider.GetUtcNow()
        });

    public void MarkCompleted(BackgroundJob job) => UpdateTerminal(job.Id, BackgroundJobStatus.Completed);

    public void MarkFailed(BackgroundJob job, string errorCode) =>
        UpdateTerminal(job.Id, BackgroundJobStatus.Failed, errorCode);

    private void UpdateTerminal(Guid id, BackgroundJobStatus status, string? errorCode = null) =>
        _states.AddOrUpdate(
            id,
            _ => throw new InvalidOperationException("A running job state must exist."),
            (_, current) => current with
            {
                Status = status,
                CompletedAt = timeProvider.GetUtcNow(),
                ErrorCode = errorCode
            });
}
