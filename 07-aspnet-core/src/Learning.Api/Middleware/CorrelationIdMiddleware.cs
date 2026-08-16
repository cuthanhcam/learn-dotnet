using Microsoft.Extensions.Primitives;

namespace Learning.Api.Middleware;

public sealed class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = ResolveCorrelationId(context.Request.Headers[HeaderName]);
        // Add the header immediately before response headers are sent. Output caching can then store
        // the reusable representation without persisting one request's correlation ID and replaying
        // it to another caller.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // A logging scope enriches every structured log written during the remaining pipeline.
        // It avoids passing correlation IDs through every method signature as business data.
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await next(context).ConfigureAwait(false);
        }
    }

    private static string ResolveCorrelationId(StringValues suppliedValues)
    {
        string? supplied = suppliedValues.Count == 1 ? suppliedValues[0] : null;

        // Bound and validate untrusted header data before reflecting it to a response or logs.
        return supplied is { Length: > 0 and <= 128 } &&
               supplied.All(character => char.IsLetterOrDigit(character) || character is '-' or '_')
            ? supplied
            : Guid.NewGuid().ToString("N");
    }
}
