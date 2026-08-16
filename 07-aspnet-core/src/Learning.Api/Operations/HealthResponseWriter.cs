using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Learning.Api.Operations;

public static class HealthResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Task WriteAsync(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        // Do not serialize HealthReport directly: exception objects and dependency details can leak
        // topology or credentials. Shape an allowlisted operational contract instead.
        var response = new
        {
            status = report.Status.ToString(),
            durationMilliseconds = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                durationMilliseconds = Math.Round(entry.Value.Duration.TotalMilliseconds, 2)
            })
        };

        return context.Response.WriteAsJsonAsync(response, JsonOptions);
    }
}
