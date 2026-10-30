using Learning.Testing.Api;
using Learning.Testing.Examples.Reminders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ILoanReader, OfflineLoanReader>();
builder.Services.AddSingleton<IReminderSender, LoggingReminderSender>();
builder.Services.AddScoped<LoanReminderService>();

var app = builder.Build();
app.UseExceptionHandler();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapPost("/loans/{loanId:guid}/reminders", async (Guid loanId, LoanReminderService reminders, CancellationToken token) =>
{
    if (loanId == Guid.Empty)
        return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "A nonempty loan ID is required.");

    ReminderOutcome outcome = await reminders.SendAsync(loanId, token);
    return outcome == ReminderOutcome.NotFound
        ? Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Loan not found.")
        : Results.Ok(new ReminderResponse(loanId, outcome.ToString()));
});

app.Run();

// WebApplicationFactory needs a public entry point; the application remains a normal runnable API.
public partial class Program { }
