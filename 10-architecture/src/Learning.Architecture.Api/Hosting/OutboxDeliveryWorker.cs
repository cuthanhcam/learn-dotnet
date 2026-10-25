using Learning.Architecture.Infrastructure.Messaging;

namespace Learning.Architecture.Api.Hosting;

/// <summary>
/// One bounded dispatch loop. A new scope per iteration prevents failed transactions/trackers from
/// contaminating the next attempt. This lab must run as ONE dispatcher instance; no distributed lease
/// is implemented. Production scaling requirements are called out in the messaging article.
/// </summary>
public sealed class OutboxDeliveryWorker(IServiceScopeFactory scopes, ILogger<OutboxDeliveryWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using AsyncServiceScope scope = scopes.CreateAsyncScope();
                    await scope.ServiceProvider.GetRequiredService<OutboxDispatcher>()
                        .DispatchAsync(cancellationToken: stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    // Keep pending rows for retry; do not acknowledge or silently swallow failures.
                    // No user-supplied message text or identity is interpolated into the log template.
                    logger.LogError(exception, "Outbox iteration failed; pending delivery will be retried.");
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal host shutdown: unfinished/uncertain delivery remains safe to redeliver.
        }
    }
}
