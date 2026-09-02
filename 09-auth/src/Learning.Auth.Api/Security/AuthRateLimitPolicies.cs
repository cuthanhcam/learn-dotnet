using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Learning.Auth.Api.Security;

/// <summary>
/// Centralizes endpoint policy names and limiter construction. The sample uses process-local fixed
/// windows for observability; a scaled deployment needs a shared limiter or edge enforcement.
/// </summary>
public static class AuthRateLimitPolicies
{
    public const string Credential = "auth-credential";
    public const string Session = "auth-session";

    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    public static void AddAuthRateLimiting(RateLimiterOptions options)
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.AddPolicy(Credential, context => CreatePartition(context, Credential, permitLimit: 10));
        options.AddPolicy(Session, context => CreatePartition(context, Session, permitLimit: 20));
        options.OnRejected = async (context, _) =>
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds)
                    .ToString(CultureInfo.InvariantCulture);
            }

            await Results.Problem(
                statusCode: StatusCodes.Status429TooManyRequests,
                title: "Too many authentication requests",
                detail: "Wait before retrying this authentication operation.")
                .ExecuteAsync(context.HttpContext)
                .ConfigureAwait(false);
        };
    }

    private static RateLimitPartition<string> CreatePartition(
        HttpContext context,
        string policy,
        int permitLimit)
    {
        // Never accept X-Forwarded-For directly here. RemoteIpAddress reflects forwarded headers only
        // when the host explicitly trusts its proxy; otherwise an attacker could choose the partition.
        string networkIdentity = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter($"{policy}:{networkIdentity}", _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = Window,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    }
}
