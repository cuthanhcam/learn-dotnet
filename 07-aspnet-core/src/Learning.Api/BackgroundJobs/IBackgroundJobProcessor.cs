namespace Learning.Api.BackgroundJobs;

public interface IBackgroundJobProcessor
{
    Task ProcessAsync(BackgroundJob job, CancellationToken cancellationToken);
}

public sealed class DemonstrationJobProcessor(ILogger<DemonstrationJobProcessor> logger)
    : IBackgroundJobProcessor
{
    public async Task ProcessAsync(BackgroundJob job, CancellationToken cancellationToken)
    {
        // A real processor would perform idempotent application work. Delay is asynchronous so this
        // learning worker demonstrates cancellation without blocking a ThreadPool thread.
        await Task.Delay(TimeSpan.FromMilliseconds(25), cancellationToken);
        logger.LogInformation("Completed background job {JobId}", job.Id);
    }
}
