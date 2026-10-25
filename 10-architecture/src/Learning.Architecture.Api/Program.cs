using Learning.Architecture.Api.Features;
using Learning.Architecture.Api.Hosting;
using Learning.Architecture.Api.Security;
using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Application.Offerings;
using Learning.Architecture.Contracts;
using Learning.Architecture.Domain.Courses;
using Learning.Architecture.Infrastructure.Messaging;
using Learning.Architecture.Infrastructure.Persistence;
using Learning.Architecture.Notifications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddOptions<IdentitySettings>().BindConfiguration("Identity")
    .Validate(settings => settings.HasHttpsAuthority(), "Identity:Authority must be an absolute HTTPS URI.")
    .Validate(settings => !string.IsNullOrWhiteSpace(settings.Audience), "Identity:Audience is required.")
    .ValidateOnStart();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<IdentitySettings>>((options, settings) =>
    {
        options.Authority = settings.Value.Authority;
        options.Audience = settings.Value.Audience;
        options.MapInboundClaims = false;
        options.RequireHttpsMetadata = true;
    });
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser().Build())
    .AddPolicy("enroll-self", policy => policy.RequireAuthenticatedUser().RequireAssertion(context =>
        Guid.TryParse(context.User.FindFirst("learner_id")?.Value, out Guid learner) && learner != Guid.Empty
        && context.User.FindAll("scope").Any(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Contains("enrollment.write", StringComparer.Ordinal))));

builder.Services.AddDbContext<EnrollmentDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("Enrollment")
        ?? throw new InvalidOperationException("Enrollment connection string is required.")));
builder.Services.AddDbContextFactory<WelcomeDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("Notifications")
        ?? throw new InvalidOperationException("Notifications connection string is required.")));
builder.Services.AddScoped<EfCourseOfferingStore>();
builder.Services.AddScoped<IOfferingQueries>(services => services.GetRequiredService<EfCourseOfferingStore>());
builder.Services.AddScoped<ListOfferingsHandler>();
builder.Services.AddScoped<IEnrollmentRequests, DurableEnrollmentRequests>();
builder.Services.AddScoped<IEnrollmentEventSink, WelcomeEnrollmentConsumer>();
builder.Services.AddScoped<OutboxDispatcher>();
if (builder.Configuration.GetValue("Outbox:Enabled", true))
    builder.Services.AddHostedService<OutboxDeliveryWorker>();

var app = builder.Build();
// Automatic schema creation is ONLY a disposable local/test convenience. Real deployment must
// provision reviewed migrations separately; EnsureCreated is not an upgrade strategy.
if (app.Configuration.GetValue<bool>("Database:Initialize"))
{
    if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
        throw new InvalidOperationException("Automatic database initialization is restricted to local labs.");
    await using AsyncServiceScope scope = app.Services.CreateAsyncScope();
    EnrollmentDbContext enrollment = scope.ServiceProvider.GetRequiredService<EnrollmentDbContext>();
    await enrollment.Database.EnsureCreatedAsync();
    Guid sample = Guid.Parse("10000000-0000-0000-0000-000000000001");
    if (!await enrollment.Offerings.AnyAsync(row => row.Id == sample))
    {
        enrollment.Offerings.Add(EfCourseOfferingStore.ToRow(CourseOffering.Create(sample, "Architecture Lab", 2)));
        await enrollment.SaveChangesAsync();
    }
    WelcomeDbContext notifications = scope.ServiceProvider.GetRequiredService<WelcomeDbContext>();
    await notifications.Database.EnsureCreatedAsync();
}
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();
app.MapEnrollmentEndpoints();
app.Run();

// Public entry-point marker for WebApplicationFactory. No business behavior belongs in Program.
public partial class Program;
