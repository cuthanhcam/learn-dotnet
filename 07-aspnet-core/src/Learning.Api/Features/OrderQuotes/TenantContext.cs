namespace Learning.Api.Features.OrderQuotes;

/// <summary>
/// Holds request-specific tenant data after the HTTP boundary validates it. This type must remain
/// scoped: a singleton would leak mutable tenant identity between concurrent requests.
/// </summary>
public sealed class TenantContext
{
    public string? TenantId { get; private set; }

    public void SetTenant(string tenantId)
    {
        if (TenantId is not null)
        {
            throw new InvalidOperationException("The tenant has already been established for this request.");
        }

        TenantId = tenantId;
    }

    public string GetRequiredTenant() => TenantId ??
        throw new InvalidOperationException("Tenant-dependent behavior ran before tenant resolution.");
}
