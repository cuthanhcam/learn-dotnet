using Learning.Architecture.Application.Enrollments;
using Learning.Architecture.Application.Offerings;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Learning.Architecture.Api.Features;

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints(this IEndpointRouteBuilder routes)
    {
        RouteGroupBuilder v1 = routes.MapGroup("/api/v1");
        v1.MapGet("/offerings", ListAsync).AllowAnonymous();
        v1.MapPost("/offerings/{offeringId:guid}/enrollments", EnrollAsync)
            .RequireAuthorization("enroll-self");
    }

    private static async Task<IResult> ListAsync(int? offset, int? limit, ListOfferingsHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new ListOfferingsQuery(offset ?? 0, limit ?? 20);
        if (query.Offset is < 0 or > 10_000 || query.Limit is < 1 or > 100)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["pagination"] = ["Offset must be 0..10000 and limit must be 1..100."]
            });
        return Results.Ok(await handler.HandleAsync(query, cancellationToken));
    }

    private static async Task<IResult> EnrollAsync(Guid offeringId, HttpContext context,
        IEnrollmentRequests requests, CancellationToken cancellationToken)
    {
        // Authorization has already required a valid learner_id claim and enrollment scope. Never
        // accept LearnerId from a body/header: a caller could otherwise enroll another user's identity.
        Guid learnerId = Guid.Parse(context.User.FindFirst("learner_id")!.Value);
        var keys = context.Request.Headers["Idempotency-Key"];
        if (offeringId == Guid.Empty || keys.Count != 1 || !Guid.TryParse(keys[0], out Guid requestId)
            || requestId == Guid.Empty)
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = ["A nonempty offering ID and one GUID Idempotency-Key are required."]
            });

        EnrollmentRequestResult result;
        try
        {
            result = await requests.ExecuteAsync(new EnrollmentRequest(requestId, offeringId, learnerId),
                cancellationToken);
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode is 5 or 6)
        {
            return StorageBusy(context);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException
        { SqliteErrorCode: 5 or 6 })
        {
            // SaveChanges wraps provider errors; classify only the known busy/locked codes.
            // Constraint violations and unrelated update failures must remain server failures.
            return StorageBusy(context);
        }
        if (result.Status == EnrollmentRequestStatus.KeyReused)
            return Results.Problem(statusCode: 409, title: "Idempotency key was reused for another request.");
        if (result.Status == EnrollmentRequestStatus.Conflict)
            return Results.Problem(statusCode: 409, title: "Offering changed; retry with the same request key.");

        EnrollLearnerResult enrollment = result.Enrollment!;
        return enrollment.Status switch
        {
            EnrollLearnerStatus.NotFound => Results.Problem(statusCode: 404, title: "Offering was not found."),
            EnrollLearnerStatus.Full => Results.Problem(statusCode: 409, title: "Offering has no available seats."),
            EnrollLearnerStatus.Enrolled or EnrollLearnerStatus.AlreadyEnrolled when enrollment.Version is >= 1
                => Results.Ok(new EnrollmentResponse(enrollment.Status.ToString(), enrollment.Version.Value,
                    result.Replayed)),
            _ => throw new InvalidOperationException("The enrollment workflow returned an invalid result.")
        };
    }

    private static IResult StorageBusy(HttpContext context)
    {
        // A provider lock timeout is transient infrastructure unavailability, not a full course.
        // A retry must retain the request key and use a fresh request scope/DbContext.
        context.Response.Headers.RetryAfter = "1";
        return Results.Problem(statusCode: 503, title: "Enrollment storage is temporarily busy.");
    }
}

public sealed record EnrollmentResponse(string Status, long Version, bool Replayed);
