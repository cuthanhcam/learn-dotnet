namespace DevForge.Application.Common.Models
{
    /// <summary>
    /// Represents the result of an operation with typed error handling
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T? Data { get; }
        public Error Error { get; }
        public Dictionary<string, string[]>? ValidationErrors { get; }

        private Result(bool isSuccess, T? data, Error error, Dictionary<string, string[]>? validationErrors = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Error = error;
            ValidationErrors = validationErrors;
        }

        public static Result<T> Success(T data) => new(true, data, Error.None);
        
        public static Result<T> Failure(Error error) => new(false, default, error);
        
        public static Result<T> ValidationFailure(Dictionary<string, string[]> errors) => 
            new(false, default, Error.Validation("Validation.Failed", "One or more validation errors occurred"), errors);
    }

    /// <summary>
    /// Represents the result of an operation without return data
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        public Dictionary<string, string[]>? ValidationErrors { get; }

        private Result(bool isSuccess, Error error, Dictionary<string, string[]>? validationErrors = null)
        {
            IsSuccess = isSuccess;
            Error = error;
            ValidationErrors = validationErrors;
        }

        public static Result Success() => new(true, Error.None);
        
        public static Result Failure(Error error) => new(false, error);
        
        public static Result ValidationFailure(Dictionary<string, string[]> errors) => 
            new(false, Error.Validation("Validation.Failed", "One or more validation errors occurred"), errors);
    }
}
