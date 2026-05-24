using System.Reflection;
using System.Diagnostics;

namespace CoreDotNet.Examples.Attributes
{
    /// <summary>
    /// Comprehensive examples for attributes and reflection.
    ///
    /// This lesson demonstrates how metadata shapes behavior and documentation:
    /// - Built-in attributes for deprecation, flags, and conditional tracing.
    /// - Custom attributes that describe endpoints, permissions, and validation rules.
    /// - Reflection queries that read metadata from types, methods, and properties.
    /// - Attribute-driven validation and documentation extraction.
    ///
    /// Best practices:
    /// - Use AttributeUsage to restrict attribute targets.
    /// - Cache reflection results when metadata is read repeatedly.
    /// - Keep attribute behavior simple and declarative.
    /// - Avoid reflection on hot paths when a cached model will do.
    /// </summary>
    public static class AttributesExample
    {
        public static void Run()
        {
            Console.WriteLine($"{new string('=', 5)} Attributes Examples {new string('=', 5)}");

            PrintSection("BUILTIN ATTRIBUTES");
            DemoBuiltInAttributes();

            PrintSection("CONDITIONAL LOGGING");
            DemoConditionalLogging();

            PrintSection("CUSTOM ATTRIBUTES");
            DemoCustomAttributes();

            PrintSection("REFLECTION QUERYING");
            DemoReflectionQuerying();

            PrintSection("PRACTICAL APPLICATIONS");
            DemoPracticalApplications();

            Console.WriteLine();
        }

        private static void DemoBuiltInAttributes()
        {
            // Obsolete attribute
            var sample = new SampleClass();
            Console.WriteLine("Built-in attributes:");
            Console.WriteLine("- [Obsolete]: Marks members as deprecated");
            Console.WriteLine("- [Flags]: Marks enum for bitwise operations");
            Console.WriteLine("- [Conditional]: Compiles out based on symbols");

            // Flags enum example
            var permissions = Permissions.Read | Permissions.Write;
            Console.WriteLine($"Permissions: {permissions}");
            Console.WriteLine($"Has Read: {permissions.HasFlag(Permissions.Read)}");

            sample.NewMethod();
#pragma warning disable CS0618
            sample.OldMethod();
#pragma warning restore CS0618
        }

        private static void DemoConditionalLogging()
        {
            TraceMessage("Attribute-driven trace message for debugging and diagnostics.");
            Console.WriteLine("Conditional methods run only when the compilation symbol is defined.");
        }

        private static void DemoCustomAttributes()
        {
            // Query custom attributes on class
            var classAttributes = typeof(ApiEndpoint).GetCustomAttributes<EndpointAttribute>();
            foreach (var attr in classAttributes)
            {
                Console.WriteLine($"Endpoint: {attr.Route} [{attr.Method}]");
            }

            // Query custom attributes on method
            var method = typeof(ApiEndpoint).GetMethod(nameof(ApiEndpoint.GetUser));
            var methodAttrs = method?.GetCustomAttributes<RequiredPermissionAttribute>();
            if (methodAttrs != null)
            {
                foreach (var attr in methodAttrs)
                {
                    Console.WriteLine($"  Requires permission: {attr.Permission}");
                }
            }

            // Query properties with attributes
            var properties = typeof(User).GetProperties();
            foreach (var prop in properties)
            {
                var validationAttrs = prop.GetCustomAttributes<ValidationAttribute>();
                foreach (var attr in validationAttrs)
                {
                    Console.WriteLine($"Property {prop.Name}: {attr.Rule}");
                }

                var displayName = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (displayName != null)
                {
                    Console.WriteLine($"Property {prop.Name} display name: {displayName.Name}");
                }
            }
        }

        private static void DemoReflectionQuerying()
        {
            Console.WriteLine("Reflection querying examples:");

            // Get all types with custom attribute
            var assembly = typeof(AttributesExample).Assembly;
            var typesWithAttribute = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<EndpointAttribute>() != null)
                .ToList();
            Console.WriteLine($"Types with [Endpoint]: {typesWithAttribute.Count}");

            // Get methods with specific attribute
            var apiMethods = typeof(ApiEndpoint).GetMethods()
                .Where(m => m.GetCustomAttribute<RequiredPermissionAttribute>() != null)
                .ToList();
            Console.WriteLine($"Methods requiring permission: {apiMethods.Count}");

            // Get attribute instances
            var attr = typeof(ApiEndpoint).GetCustomAttribute<EndpointAttribute>();
            if (attr != null)
            {
                Console.WriteLine($"ApiEndpoint route: {attr.Route}");
            }
        }

        private static void DemoPracticalApplications()
        {
            // Validation pattern
            var user = new User { Name = "Alice", Email = "alice@example.com", Age = 30 };
            bool isValid = ValidateObject(user);
            Console.WriteLine($"User is valid: {isValid}");

            // Validation with invalid data
            var invalidUser = new User { Name = "", Email = "invalid", Age = -5 };
            isValid = ValidateObject(invalidUser);
            Console.WriteLine($"Invalid user is valid: {isValid}");

            // Metadata extraction
            ExtractMetadata(typeof(ApiEndpoint));
        }

        private static bool ValidateObject(object obj)
        {
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties)
            {
                object? value = prop.GetValue(obj);
                var validations = prop.GetCustomAttributes<ValidationAttribute>();

                foreach (var validation in validations)
                {
                    if (!validation.IsValid(value))
                    {
                        Console.WriteLine($"  Validation failed: {prop.Name} - {validation.Rule}");
                        return false;
                    }
                }
            }
            return true;
        }

        private static void ExtractMetadata(Type type)
        {
            var endpoint = type.GetCustomAttribute<EndpointAttribute>();
            if (endpoint != null)
            {
                Console.WriteLine($"API Endpoint: {endpoint.Route} [{endpoint.Method}]");
                Console.WriteLine($"Description: {endpoint.Description}");
            }
        }

        [Conditional("DEBUG")]
        private static void TraceMessage(string message)
        {
            Console.WriteLine($"TRACE: {message}");
        }

        private static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.WriteLine($"--- {title} ---");
        }
    }

    // Custom attribute definitions
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EndpointAttribute : Attribute
    {
        public string Route { get; }
        public string Method { get; }
        public string Description { get; set; } = string.Empty;

        public EndpointAttribute(string route, string method = "GET")
        {
            Route = route;
            Method = method;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class RequiredPermissionAttribute : Attribute
    {
        public string Permission { get; }

        public RequiredPermissionAttribute(string permission)
        {
            Permission = permission;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ValidationAttribute : Attribute
    {
        public string Rule { get; }

        public ValidationAttribute(string rule)
        {
            Rule = rule;
        }

        public virtual bool IsValid(object? value)
        {
            return value != null;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string Name { get; }

        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }

    public class RequiredAttribute : ValidationAttribute
    {
        public RequiredAttribute() : base("Required") { }

        public override bool IsValid(object? value)
        {
            return value != null && (value is not string s || !string.IsNullOrEmpty(s));
        }
    }

    public class MinLengthAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public MinLengthAttribute(int minLength) : base($"Min length {minLength}")
        {
            _minLength = minLength;
        }

        public override bool IsValid(object? value)
        {
            return value is string s && s.Length >= _minLength;
        }
    }

    public class RangeAttribute : ValidationAttribute
    {
        private readonly int _min;
        private readonly int _max;

        public RangeAttribute(int min, int max) : base($"Range {min}-{max}")
        {
            _min = min;
            _max = max;
        }

        public override bool IsValid(object? value)
        {
            return value is int i && i >= _min && i <= _max;
        }
    }

    // Sample classes
    [Endpoint("/api/users", "GET", Description = "User management endpoint")]
    public class ApiEndpoint
    {
        [RequiredPermission("read:users")]
        public void GetUser() { }

        [RequiredPermission("write:users")]
        public void CreateUser() { }
    }

    public class User
    {
        [DisplayName("Full name")]
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [DisplayName("Email address")]
        [Required]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Age in years")]
        [Range(0, 150)]
        public int Age { get; set; }
    }

    public class SampleClass
    {
        [Obsolete("Use NewMethod instead")]
        public void OldMethod() { }

        public void NewMethod() { }
    }

    [Flags]
    public enum Permissions
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4,
        Execute = 8,
        Admin = Read | Write | Delete | Execute
    }
}
