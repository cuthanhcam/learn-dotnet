using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace DevForge.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            var (statusCode, title, errors) = exception switch
            {
                ValidationException validationException => HandleValidationException(validationException),
                UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Unauthorized", new Dictionary<string, string[]>()),
                _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error", new Dictionary<string, string[]>())
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var problemDetails = new
            {
                type = $"https://httpstatuses.com/{statusCode}",
                title = title,
                status = statusCode,
                detail = exception.Message,
                errors = errors.Any() ? errors : null,
                traceId = httpContext.TraceIdentifier
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails), cancellationToken);

            return true;
        }

        private (int statusCode, string title, Dictionary<string, string[]> errors) HandleValidationException(ValidationException exception)
        {
            var errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return ((int)HttpStatusCode.BadRequest, "Validation Error", errors);
        }
    }
}
