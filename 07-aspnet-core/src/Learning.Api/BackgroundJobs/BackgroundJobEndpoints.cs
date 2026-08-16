namespace Learning.Api.BackgroundJobs;

public static class BackgroundJobEndpoints
{
    public static IEndpointRouteBuilder MapBackgroundJobEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/background-jobs")
            .WithTags("Background jobs");

        group.MapPost("/", Submit)
            .WithName("SubmitBackgroundJob")
            .Produces<BackgroundJobState>(StatusCodes.Status202Accepted)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);
        group.MapGet("/{id:guid}", Get)
            .WithName("GetBackgroundJob")
            .Produces<BackgroundJobState>()
            .ProducesProblem(StatusCodes.Status404NotFound);
        return endpoints;
    }

    private static IResult Submit(
        SubmitBackgroundJobRequest request,
        BackgroundJobQueue queue,
        BackgroundJobStore store)
    {
        int descriptionLength = request.Description?.Trim().Length ?? 0;
        if (descriptionLength is < 3 or > 200)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Description)] = ["Description must contain between 3 and 200 characters."]
            });
        }

        BackgroundJob job = store.Create(request.Description!);
        if (!queue.TryEnqueue(job))
        {
            store.Remove(job.Id);
            return Results.Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "The background job queue is full.",
                detail: "Retry later; the service does not accept unbounded queued work.");
        }

        return Results.AcceptedAtRoute("GetBackgroundJob", new { id = job.Id }, store.Find(job.Id));
    }

    private static IResult Get(Guid id, BackgroundJobStore store) =>
        store.Find(id) is { } state
            ? Results.Ok(state)
            : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Background job not found.");
}
