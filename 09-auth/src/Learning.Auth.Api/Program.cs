WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

// WebApplicationFactory discovers this type when running integration tests.
public partial class Program;
