namespace CoreDotNet.Examples.ExceptionHandling
{
    /// <summary>
    /// Comprehensive examples for exception handling patterns.
    ///
    /// This lesson uses practical scenarios rather than generic samples:
    /// - Guard clauses and validation failures.
    /// - Specific catch blocks and exception filters.
    /// - Custom exceptions for domain-specific problems.
    /// - Retry logic with temporary failures.
    /// - Resource cleanup and exception translation.
    ///
    /// Best practices:
    /// - Catch specific exceptions, not generic Exception.
    /// - Use using/try-finally for cleanup.
    /// - Preserve exception context with inner exceptions.
    /// - Translate low-level errors into domain-specific errors when needed.
    /// - Keep retry loops small and deterministic.
    /// </summary>
    public static class ExceptionHandlingExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} Exception Handling Examples {new string('=', 5)}");

            PrintSection("EXCEPTION HIERARCHY");
            DemoExceptionHierarchy();

            PrintSection("TRY-CATCH PATTERNS");
            DemoTryCatchPatterns();

            PrintSection("CUSTOM EXCEPTIONS");
            DemoCustomExceptions();

            PrintSection("RETRY PATTERNS");
            DemoRetryPattern();

            PrintSection("RESOURCE CLEANUP");
            DemoResourceCleanup();

            PrintSection("EXCEPTION CONTEXT");
            DemoExceptionContext();

            Console.WriteLine();
        }

        private static void DemoExceptionHierarchy()
        {
            Console.WriteLine("Common exception types:");
            Console.WriteLine("- ArgumentException: Invalid argument values");
            Console.WriteLine("- ArgumentNullException: Null argument when not allowed");
            Console.WriteLine("- InvalidOperationException: Invalid object state");
            Console.WriteLine("- FormatException: Format parsing failed");
            Console.WriteLine("- IOException: I/O operation failed");
            Console.WriteLine("- TimeoutException: Operation timed out");

            try
            {
                int.Parse("invalid");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Caught FormatException: {ex.Message}");
            }
        }

        private static void DemoTryCatchPatterns()
        {
            // Pattern 1: Specific exception handling
            try
            {
                int result = 10 / int.Parse("0");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Format issue: {ex.Message}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Math error: {ex.Message}");
            }

            // Pattern 2: Using filter expressions
            try
            {
                throw new ArgumentException("Invalid value");
            }
            catch (ArgumentException ex) when (ex.Message.Contains("Invalid"))
            {
                Console.WriteLine($"Caught validation error: {ex.Message}");
            }

            // Pattern 3: try-finally for cleanup
            var resource = new DisposableResource();
            try
            {
                resource.DoWork();
            }
            finally
            {
                resource.Cleanup();
            }
        }

        private static void DemoCustomExceptions()
        {
            try
            {
                ValidateAge(-5);
            }
            catch (InvalidAgeException ex)
            {
                Console.WriteLine($"Domain error: {ex.Message}");
                Console.WriteLine($"  Age attempted: {ex.InvalidAge}");
            }

            try
            {
                throw new BusinessException("Order processing failed", new InvalidOperationException("Payment declined"));
            }
            catch (BusinessException ex)
            {
                Console.WriteLine($"Business error: {ex.Message}");
                Console.WriteLine($"  Inner exception: {ex.InnerException?.Message}");
            }

            try
            {
                ValidatePurchase("", 0);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Validation error: {ex.ParamName} - {ex.Message}");
            }
        }

        private static void DemoRetryPattern()
        {
            Console.WriteLine("Retry with exponential backoff:");
            int retries = 0;
            int maxRetries = 3;
            int delayMs = 100;

            while (retries < maxRetries)
            {
                try
                {
                    AttemptFlakyOperation(retries);
                    Console.WriteLine("Operation succeeded!");
                    break;
                }
                catch (TemporaryException ex)
                {
                    retries++;
                    if (retries >= maxRetries)
                    {
                        Console.WriteLine($"Failed after {maxRetries} retries: {ex.Message}");
                        break;
                    }

                    Console.WriteLine($"  Retry {retries}/{maxRetries} after {delayMs}ms: {ex.Message}");
                    System.Threading.Thread.Sleep(delayMs);
                    delayMs *= 2; // Exponential backoff
                }
            }
        }

        private static void DemoExceptionContext()
        {
            try
            {
                try
                {
                    throw new FormatException("Invalid format");
                }
                catch (FormatException ex)
                {
                    // Re-throw with context
                    throw new DataProcessingException("Failed to process data", ex);
                }
            }
            catch (DataProcessingException ex)
            {
                Console.WriteLine($"Processing failed: {ex.Message}");
                Exception? current = ex;
                int level = 0;
                while (current != null)
                {
                    Console.WriteLine($"  Level {level}: {current.GetType().Name} - {current.Message}");
                    current = current.InnerException;
                    level++;
                }
            }
        }

        private static void ValidateAge(int age)
        {
            if (age < 0 || age > 150)
            {
                throw new InvalidAgeException(age);
            }
        }

        private static void DemoResourceCleanup()
        {
            using var resource = new DisposableResource();
            resource.DoWork();
            Console.WriteLine("Resource will be disposed automatically when the scope ends.");
        }

        private static void AttemptFlakyOperation(int attempt)
        {
            // Fail twice, succeed on third attempt
            if (attempt < 2)
            {
                throw new TemporaryException("Service temporarily unavailable");
            }
        }

        private static void ValidatePurchase(string customerId, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("Customer ID is required.", nameof(customerId));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be greater than zero.");
            }
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    // Custom exception classes
    public class InvalidAgeException : Exception
    {
        public int InvalidAge { get; }

        public InvalidAgeException(int age)
            : base($"Age must be between 0 and 150, got {age}")
        {
            InvalidAge = age;
        }
    }

    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, Exception inner) : base(message, inner) { }
    }

    public class TemporaryException : Exception
    {
        public TemporaryException(string message) : base(message) { }
    }

    public class DataProcessingException : Exception
    {
        public DataProcessingException(string message) : base(message) { }
        public DataProcessingException(string message, Exception inner) : base(message, inner) { }
    }

    public class DisposableResource : IDisposable
    {
        public void DoWork()
        {
            Console.WriteLine("Resource working...");
        }

        public void Cleanup()
        {
            Console.WriteLine("Resource cleaned up");
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
