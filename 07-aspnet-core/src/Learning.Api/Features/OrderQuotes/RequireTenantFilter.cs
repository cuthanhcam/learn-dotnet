using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Learning.Api.Features.OrderQuotes;

/// <summary>
/// Demonstrates a DI-created action filter. Authentication systems should establish real identity;
/// this learning filter only illustrates reusable controller-boundary behavior.
/// </summary>
public sealed partial class RequireTenantFilter(
    TenantContext tenantContext,
    ILogger<RequireTenantFilter> logger) : IAsyncActionFilter
{
    public const string HeaderName = "X-Tenant-Id";

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        string[] values = context.HttpContext.Request.Headers[HeaderName]
            .Where(value => value is not null)
            .Select(value => value!)
            .ToArray();

        if (values.Length != 1 || !TenantIdPattern().IsMatch(values[0]))
        {
            // Assigning Result short-circuits the action. Do not call next after deciding that the
            // request is invalid, otherwise the controller would execute despite the error.
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "A valid tenant header is required.",
                Detail = $"Send exactly one {HeaderName} containing 3-32 letters, digits, or hyphens."
            });
            return;
        }

        tenantContext.SetTenant(values[0]);
        using IDisposable? scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["TenantId"] = values[0]
        });
        await next();
    }

    [GeneratedRegex("^[A-Za-z0-9-]{3,32}$", RegexOptions.CultureInvariant)]
    private static partial Regex TenantIdPattern();
}
