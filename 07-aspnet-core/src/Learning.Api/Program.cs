using Learning.Api.Configuration;
using Learning.Api.Features.Products;
using Learning.Api.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Problem Details gives error responses a standard machine-readable shape instead of
// returning ad-hoc strings that every client must interpret differently.
builder.Services.AddProblemDetails();

builder.Services
    .AddOptions<LearningOptions>()
    .Bind(builder.Configuration.GetSection(LearningOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// The repository is intentionally in-memory for this hosting phase. Phase 08 replaces the
// persistence boundary without changing endpoint contracts or application-level behavior.
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton(TimeProvider.System);

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapGet("/health", () => TypedResults.Ok(new { Status = "healthy" }))
    .WithName("Health")
    .WithTags("Operations");

app.MapProductEndpoints();

app.Run();

// WebApplicationFactory discovers this entry point from the test project. Keeping the type
// partial adds no runtime behavior and avoids exposing application internals only for tests.
public partial class Program;
