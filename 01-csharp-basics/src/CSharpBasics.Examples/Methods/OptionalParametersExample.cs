using System;
using System.Collections.Generic;

namespace CSharpBasics.Examples.Methods
{
    /// <summary>
    /// Comprehensive lesson for optional parameters and named arguments.
    /// 
    /// This example is designed for learning purposes but follows
    /// real-world best practices used in .NET backend development.
    /// 
    /// Key topics:
    /// - Optional parameters with default values
    /// - Named arguments for clarity
    /// - Method overloading vs optional parameters
    /// - Parameter order and flexibility
    /// - Nullable parameters
    /// 
    /// Best practices:
    /// - Optional parameters must come after required parameters
    /// - Use named arguments with optional parameters for clarity
    /// - Avoid excessive optional parameters (max 2-3)
    /// - Use meaningful default values
    /// - Document what default value means
    /// - Consider using objects for multiple optional parameters
    /// 
    /// When to use optional parameters:
    /// - Adding new features without breaking existing calls
    /// - Methods with reasonable defaults
    /// - Improving API usability
    /// 
    /// When NOT to use optional parameters:
    /// - When overloading makes meaning clearer
    /// - When logical grouping requires method variants
    /// - When too many combinations exist
    /// </summary>
    public static class OptionalParametersExample
    {
        /// <summary>
        /// Entry point to run all demos.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} OptionalParametersExample {new string('=', 5)}");

            PrintSection("OPTIONAL PARAMETERS");
            DemoOptionalParameters();

            PrintSection("NAMED ARGUMENTS");
            DemoNamedArguments();

            PrintSection("OPTIONAL DATE PARAMETERS");
            DemoOptionalDateParameters();

            PrintSection("OPTIONAL NULLABLE PARAMETERS");
            DemoOptionalNullableParameters();

            PrintSection("WHEN OPTIONALS BECOME TOO MANY");
            DemoParameterObjectPattern();

            Console.WriteLine();
        }

        // PUBLIC METHODS

        /// <summary>
        /// Creates a user label with optional role and status.
        /// Demonstrates optional parameters with defaults.
        /// </summary>
        public static string CreateUserLabel(string name, string role = "Student", bool isActive = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            string normalizedRole = string.IsNullOrWhiteSpace(role) ? "Student" : role.Trim();
            string status = isActive ? "Active" : "Inactive";
            return $"{name.Trim()} ({normalizedRole}) - {status}";
        }

        /// <summary>
        /// Formats an audit message with optional timestamp.
        /// Uses DateTime.UtcNow as default when not specified.
        /// </summary>
        public static string FormatAuditMessage(string action, string entityId, DateTime? timestampUtc = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action is required.", nameof(action));

            if (string.IsNullOrWhiteSpace(entityId))
                throw new ArgumentException("Entity ID is required.", nameof(entityId));

            DateTime timestamp = timestampUtc ?? DateTime.UtcNow;
            return $"{action.Trim()} [{entityId.Trim()}] at {timestamp:O}";
        }

        /// <summary>
        /// Builds a query string with optional filters.
        /// Demonstrates multiple optional parameters.
        /// </summary>
        public static string BuildQueryString(string baseUrl, int? pageNumber = null, int? pageSize = null, string? sortBy = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL is required.", nameof(baseUrl));

            if (pageNumber.HasValue && pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

            if (pageSize.HasValue && (pageSize <= 0 || pageSize > 500))
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 500.");

            var parameters = new List<string>();

            if (pageNumber.HasValue)
                parameters.Add($"page={pageNumber}");

            if (pageSize.HasValue)
                parameters.Add($"pageSize={pageSize}");

            if (!string.IsNullOrWhiteSpace(sortBy))
                parameters.Add($"sortBy={Uri.EscapeDataString(sortBy.Trim())}");

            return parameters.Count > 0
                ? $"{baseUrl}?{string.Join("&", parameters)}"
                : baseUrl;
        }

        /// <summary>
        /// Sends a notification with optional delay and retry settings.
        /// Good example of when optional parameters improve usability.
        /// </summary>
        public static string SendNotification(
            string recipient,
            string message,
            int delayMs = 0,
            int maxRetries = 3,
            bool sendAsync = false)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("Recipient is required.", nameof(recipient));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.", nameof(message));

            if (delayMs < 0)
                throw new ArgumentOutOfRangeException(nameof(delayMs), "Delay cannot be negative.");

            if (delayMs > 60_000)
                throw new ArgumentOutOfRangeException(nameof(delayMs), "Delay cannot exceed 60,000 ms for this demo.");

            if (maxRetries is < 0 or > 10)
                throw new ArgumentOutOfRangeException(nameof(maxRetries), "Retry count must be between 0 and 10.");

            var settings = new List<string>();
            if (delayMs > 0)
                settings.Add($"delay={delayMs}ms");
            settings.Add($"retries={maxRetries}");
            if (sendAsync)
                settings.Add("async");

            return $"Notification to {recipient}: '{message}' [{string.Join(", ", settings)}]";
        }

        /// <summary>
        /// Parameter object approach for APIs that would otherwise require too many optional parameters.
        /// </summary>
        public static string SendNotificationWithOptions(string recipient, string message, NotificationOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(recipient))
                throw new ArgumentException("Recipient is required.", nameof(recipient));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.", nameof(message));

            options ??= new NotificationOptions();

            if (options.DelayMs is < 0 or > 60_000)
                throw new ArgumentOutOfRangeException(nameof(options.DelayMs), "Delay must be between 0 and 60,000 ms.");

            if (options.MaxRetries is < 0 or > 10)
                throw new ArgumentOutOfRangeException(nameof(options.MaxRetries), "Max retries must be between 0 and 10.");

            string priority = options.IsHighPriority ? "high-priority" : "normal";
            string channel = options.Channel;

            return $"Notification[{priority}] via {channel} to {recipient} (delay={options.DelayMs}ms, retries={options.MaxRetries}): '{message}'";
        }

        /// <summary>
        /// Logs a message with optional level and category.
        /// </summary>
        public static string LogMessage(
            string message,
            string level = "Info",
            string category = "General",
            bool includeTimestamp = true)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message is required.", nameof(message));

            string timestamp = includeTimestamp ? $" [{DateTime.Now:HH:mm:ss}]" : "";
            return $"[{level}] [{category}]{timestamp} {message}";
        }

        public sealed record NotificationOptions
        {
            public int DelayMs { get; init; } = 0;
            public int MaxRetries { get; init; } = 3;
            public bool IsHighPriority { get; init; }
            public string Channel { get; init; } = "email";
        }

        // PRIVATE DEMO METHODS

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }

        /// <summary>
        /// Demonstrates calling methods with optional parameters using default values.
        /// </summary>
        private static void DemoOptionalParameters()
        {
            // Using all defaults
            Console.WriteLine(CreateUserLabel("Cam"));

            // Override some defaults
            Console.WriteLine(CreateUserLabel("Alex", role: "Mentor"));

            // Override all parameters
            Console.WriteLine(CreateUserLabel("An", "Reviewer", false));

            Console.WriteLine();
            Console.WriteLine("All calls use same method with different defaults");
        }

        /// <summary>
        /// Demonstrates clarity of named arguments.
        /// </summary>
        private static void DemoNamedArguments()
        {
            // Without named arguments: unclear what values mean
            Console.WriteLine(CreateUserLabel("Name1", "Admin", true));

            // With named arguments: crystal clear
            Console.WriteLine(CreateUserLabel(name: "Name2", role: "Moderator", isActive: true));

            // Mix: required positional, optional named
            Console.WriteLine(CreateUserLabel("Name3", isActive: false, role: "Viewer"));

            Console.WriteLine();
            Console.WriteLine("Named arguments improve code clarity and self-documentation");
        }

        /// <summary>
        /// Demonstrates optional date/time parameters.
        /// </summary>
        private static void DemoOptionalDateParameters()
        {
            // Without timestamp (uses current time)
            Console.WriteLine(FormatAuditMessage("created", "doc-001"));

            // With specific timestamp
            var specificTime = new DateTime(2026, 3, 31, 14, 30, 0, DateTimeKind.Utc);
            Console.WriteLine(FormatAuditMessage("updated", "doc-001", specificTime));

            Console.WriteLine();
            Console.WriteLine("Optional nullable parameters enable sensible defaults");
        }

        /// <summary>
        /// Demonstrates complex optional parameter usage.
        /// </summary>
        private static void DemoOptionalNullableParameters()
        {
            // Minimal URL
            Console.WriteLine(BuildQueryString("https://api.example.com/users"));

            // With paging
            Console.WriteLine(BuildQueryString("https://api.example.com/users", pageNumber: 1, pageSize: 10));

            // With sorting
            Console.WriteLine(BuildQueryString("https://api.example.com/users", sortBy: "name"));

            // All parameters
            Console.WriteLine(BuildQueryString(
                "https://api.example.com/users",
                pageNumber: 2,
                pageSize: 20,
                sortBy: "createdDate"));

            Console.WriteLine();
            Console.WriteLine("Notification settings:");
            Console.WriteLine(SendNotification("user@example.com", "Hello"));
            Console.WriteLine(SendNotification("user@example.com", "Urgent", delayMs: 1000, maxRetries: 5, sendAsync: true));

            Console.WriteLine();
            Console.WriteLine("Logging with defaults:");
            Console.WriteLine(LogMessage("Operation started"));
            Console.WriteLine(LogMessage("Error occurred", level: "Error", category: "Database"));
        }

        /// <summary>
        /// Demonstrates the switch from many optional parameters to a parameter object.
        /// </summary>
        private static void DemoParameterObjectPattern()
        {
            Console.WriteLine("As options grow, parameter object keeps call sites readable:");

            Console.WriteLine(SendNotificationWithOptions("owner@example.com", "Daily digest"));

            var options = new NotificationOptions
            {
                DelayMs = 1_500,
                MaxRetries = 5,
                IsHighPriority = true,
                Channel = "sms"
            };

            Console.WriteLine(SendNotificationWithOptions("oncall@example.com", "Service latency high", options));
        }
    }
}
