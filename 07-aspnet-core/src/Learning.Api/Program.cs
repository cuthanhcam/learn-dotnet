using Learning.Api.Configuration;
using Learning.Api.Features.Products;
using Learning.Api.Middleware;
using Learning.Api.Operations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO.Compression;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Problem Details gives error responses a standard machine-readable shape instead of
// returning ad-hoc strings that every client must interpret differently.
builder.Services.AddProblemDetails(options =>
{
    // traceId is safe operational context. Exception messages and stack traces are deliberately
    // excluded because error bodies cross the application's trust boundary.
    options.CustomizeProblemDetails = context =>
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    options.Level = CompressionLevel.Fastest);

string[] allowedOrigins = builder.Configuration
    .GetSection($"{LearningOptions.SectionName}:AllowedOrigins")
    .Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy(TrafficPolicyNames.BrowserClient, policy =>
    policy.WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        await Results.Problem(
                statusCode: StatusCodes.Status429TooManyRequests,
                title: "The request rate limit was exceeded.")
            .ExecuteAsync(context.HttpContext);
    };
    options.AddPolicy(TrafficPolicyNames.DemoRateLimit, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-client",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 2,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1),
                AutoReplenishment = true
            }));
});

builder.Services
    .AddOptions<LearningOptions>()
    .Bind(builder.Configuration.GetSection(LearningOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// The repository is intentionally in-memory for this hosting phase. Phase 08 replaces the
// persistence boundary without changing endpoint contracts or application-level behavior.
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<ProductCatalog>();
builder.Services.AddScoped<Learning.Api.Features.OrderQuotes.OrderQuoteService>();
builder.Services.AddScoped<Learning.Api.Features.OrderQuotes.TenantContext>();
builder.Services.AddScoped<Learning.Api.Features.OrderQuotes.RequireTenantFilter>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHealthChecks()
    .AddCheck<CatalogReadinessHealthCheck>(
        "product-catalog",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready"]);

WebApplication app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseResponseCompression();
app.UseCors();
app.UseRateLimiter();

// The document endpoint is a development-time diagnostic surface. Publishing it publicly is an
// explicit product/security decision because it exposes the complete reachable API contract.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // ASP.NET Core 10 can serialize the same generated contract as YAML without a UI dependency.
    app.MapOpenApi("/openapi/{documentName}.yaml");
}

// Liveness answers only whether this process can serve HTTP. Readiness additionally checks the
// dependencies required for useful work, allowing an orchestrator to stop routing traffic safely.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = HealthResponseWriter.WriteAsync
}).WithName("Liveness").WithTags("Operations");

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = HealthResponseWriter.WriteAsync
}).WithName("Readiness").WithTags("Operations");

// Preserve the introductory endpoint as a compatibility alias while making its semantics explicit.
app.MapGet("/health", () => TypedResults.Redirect("/health/live", permanent: false))
    .ExcludeFromDescription();

app.MapProductEndpoints();
app.MapTrafficPolicyEndpoints();
app.MapControllers();

app.Run();

// WebApplicationFactory discovers this entry point from the test project. Keeping the type
// partial adds no runtime behavior and avoids exposing application internals only for tests.
public partial class Program;
