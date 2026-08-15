using Microsoft.AspNetCore.RateLimiting;

namespace Learning.Api.Operations;

public static class TrafficPolicyNames
{
    public const string BrowserClient = "browser-client";
    public const string DemoRateLimit = "demo-rate-limit";
}

public static class TrafficPolicyEndpoints
{
    public static IEndpointRouteBuilder MapTrafficPolicyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/traffic-policy-demo")
            .WithTags("Traffic policies")
            .RequireCors(TrafficPolicyNames.BrowserClient);

        group.MapGet("/compressed", () => TypedResults.Text(
                new string('x', 8 * 1024),
                contentType: "text/plain"))
            .WithSummary("Return a compressible payload for content-negotiation experiments");

        group.MapGet("/limited", () => TypedResults.Ok(new { Accepted = true }))
            .WithSummary("Demonstrate a client-partitioned fixed-window rate limit")
            .RequireRateLimiting(TrafficPolicyNames.DemoRateLimit);

        return endpoints;
    }
}
