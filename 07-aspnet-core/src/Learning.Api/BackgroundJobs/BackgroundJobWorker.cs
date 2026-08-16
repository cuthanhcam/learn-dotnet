namespace Learning.Api.BackgroundJobs;

public sealed class BackgroundJobWorker(
    BackgroundJobQueue queue,
    BackgroundJobStore store,
    IServiceScopeFactory scopeFactory,
    ILogger<BackgroundJobWorker> logger) : IHostedService
{
    private readonly CancellationTokenSource _forcedStop = new();
    private Task? _worker;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _worker = RunAsync(_forcedStop.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop accepting new work and drain queued jobs. Only the host's shutdown deadline forces
        // cancellation; this differs from cancelling all work immediately when shutdown begins.
        queue.Complete();
        using CancellationTokenRegistration registration =
            cancellationToken.Register(_forcedStop.Cancel);

        if (_worker is not null)
        {
            await _worker.WaitAsync(cancellationToken);
        }
    }

    private async Task RunAsync(CancellationToken cancellationToken)
    {
        await foreach (BackgroundJob job in queue.ReadAllAsync(cancellationToken))
        {
            store.MarkRunning(job);
            try
            {
                // Hosted services are singletons. Create a scope per unit of work before resolving
                // scoped application or persistence services.
                await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                IBackgroundJobProcessor processor =
                    scope.ServiceProvider.GetRequiredService<IBackgroundJobProcessor>();
                await processor.ProcessAsync(job, cancellationToken);
                store.MarkCompleted(job);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                store.MarkFailed(job, "shutdown-cancelled");
                throw;
            }
            catch (Exception exception)
            {
                store.MarkFailed(job, "processing-failed");
                logger.LogError(exception, "Background job {JobId} failed", job.Id);
                // One poison job must not terminate the worker and strand all later jobs.
            }
        }
    }
}
