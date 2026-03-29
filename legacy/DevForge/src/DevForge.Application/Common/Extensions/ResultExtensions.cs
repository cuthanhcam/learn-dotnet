using DevForge.Application.Common.Models;

namespace DevForge.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods for Result and Error types
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Creates a failure result from a string message (compatibility helper)
        /// </summary>
        public static Result<T> FailureFrom<T>(string message)
        {
            return Result<T>.Failure(Error.Failure("Error", message));
        }

        /// <summary>
        /// Creates a non-generic failure result from a string message (compatibility helper)
        /// </summary>
        public static Result FailureFrom(string message)
        {
            return Result.Failure(Error.Failure("Error", message));
        }
    }
}
