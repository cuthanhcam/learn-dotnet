using Learning.Api.Configuration;
using Learning.Api.Features.Products;
using Learning.Api.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Problem Details gives error responses a standard machine-readable shape instead of
// returning ad-hoc strings that every client must interpret differently.
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

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

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

// The document endpoint is a development-time diagnostic surface. Publishing it publicly is an
// explicit product/security decision because it exposes the complete reachable API contract.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // ASP.NET Core 10 can serialize the same generated contract as YAML without a UI dependency.
    app.MapOpenApi("/openapi/{documentName}.yaml");
}

app.MapGet("/health", () => TypedResults.Ok(new { Status = "healthy" }))
    .WithName("Health")
    .WithTags("Operations");

app.MapProductEndpoints();
app.MapControllers();

app.Run();

// WebApplicationFactory discovers this entry point from the test project. Keeping the type
// partial adds no runtime behavior and avoids exposing application internals only for tests.
public partial class Program;
