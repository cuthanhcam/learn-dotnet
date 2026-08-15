using Learning.Api.Features.Products;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learning.Api.Operations;

public sealed class CatalogReadinessHealthCheck(IProductRepository repository) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // A very small bounded read proves that the dependency can answer. Real database checks
            // should use provider-supported probes and a short timeout, never an expensive query.
            await repository.PageAsync(skip: 0, take: 1, cancellationToken);
            return HealthCheckResult.Healthy("The product catalog is reachable.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new HealthCheckResult(
                context.Registration.FailureStatus,
                "The product catalog probe failed.",
                exception);
        }
    }
}
