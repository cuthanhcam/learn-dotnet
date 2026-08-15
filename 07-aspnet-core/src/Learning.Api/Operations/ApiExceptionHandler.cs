using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Learning.Api.Operations;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        (int status, string title) = exception switch
        {
            CatalogUnavailableException =>
                (StatusCodes.Status503ServiceUnavailable, "The product catalog is temporarily unavailable."),
            _ =>
                (StatusCodes.Status500InternalServerError, "An unexpected server error occurred.")
        };

        // Log the full exception internally, but return a stable public description. Logging the
        // exception through structured APIs preserves its stack trace and searchable properties.
        logger.LogError(exception, "Request failed with mapped status code {StatusCode}", status);
        httpContext.Response.StatusCode = status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title
            }
        });
    }
}

public sealed class CatalogUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);
