---
title: "Exception Handling in .NET"
description: "Exception taxonomy, propagation, filters, cleanup, custom exceptions, and resilient error boundaries."
slug: dotnet-exception-handling
phase: 3
order: 3
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 25
topics: [dotnet, exceptions, error-handling]
prerequisites: [csharp-methods-and-parameters, csharp-construction-finalization-disposal]
status: maintained
last-reviewed: 2026-08-15
---

# ⚠️ Exception Handling: Building Robust Applications

## Overview

Exceptions are .NET's mechanism for signaling and handling error conditions. This section covers exception hierarchy, patterns, and best practices for building resilient applications.

## Table of Contents

1. [Exception Hierarchy](#exception-hierarchy)
2. [Basic Exception Handling](#basic-exception-handling)
3. [Custom Exceptions](#custom-exceptions)
4. [Advanced Patterns](#advanced-patterns)
5. [Best Practices](#best-practices)
6. [Common Pitfalls](#common-pitfalls)

## Exception Hierarchy

### Built-in Exception Classes

```
System.Object
└── System.Exception
    ├── System.ApplicationException (for application errors)
    ├── System.SystemException (for system errors)
    │   ├── System.ArgumentException
    │   ├── System.FormatException
    │   ├── System.InvalidOperationException
    │   ├── System.NullReferenceException
    │   ├── System.OutOfMemoryException
    │   └── ... (many others)
    └── ... (other base exceptions)
```

### Most Common Exceptions

```csharp
// ArgumentException - Invalid argument passed
throw new ArgumentException("Value must be positive", nameof(value));

// ArgumentNullException - Null reference for non-nullable parameter
throw new ArgumentNullException(nameof(user));

// InvalidOperationException - Object in invalid state
throw new InvalidOperationException("Collection was modified");

// FormatException - Format invalid
throw new FormatException("Invalid date format");

// NotImplementedException - Feature not implemented
throw new NotImplementedException();

// TimeoutException - Operation timed out
throw new TimeoutException("Request exceeded timeout");
```

## Basic Exception Handling

### Try-Catch-Finally

```csharp
try
{
    var result = ParseInteger("42");
}
catch (FormatException ex)
{
    Console.WriteLine($"Invalid format: {ex.Message}");
}
catch (OverflowException ex)
{
    Console.WriteLine($"Number too large: {ex.Message}");
}
catch (Exception ex)
{
    // Catch-all (should be specific when possible)
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
finally
{
    Console.WriteLine("Cleanup code here");
}
```

### Finally Block

```csharp
// Finally always executes
StreamReader? reader = null;
try
{
    reader = new StreamReader("file.txt");
    var content = reader.ReadToEnd();
}
finally
{
    reader?.Dispose(); // Cleanup
}

// Better: Use 'using' statement (see below)
```

### Using Statement (Resource Management)

```csharp
// Automatically disposes IDisposable
using (var reader = new StreamReader("file.txt"))
{
    var content = reader.ReadToEnd();
} // Dispose called automatically

// C# 8+ declaration style
using var reader = new StreamReader("file.txt");
var content = reader.ReadToEnd();
// Dispose called when leaving scope
```

### Exception Filters (C# 6+)

```csharp
try
{
    SomeOperation();
}
catch (HttpRequestException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("Resource not found");
}
catch (HttpRequestException ex) when (ex.StatusCode == 500)
{
    Console.WriteLine("Server error");
}
catch (HttpRequestException)
{
    Console.WriteLine("Other HTTP error");
}
```

## Custom Exceptions

### Creating Custom Exception

```csharp
public class InvalidAgeException : Exception
{
    public InvalidAgeException()
        : base("Age must be between 0 and 150") { }

    public InvalidAgeException(int age)
        : base($"Age {age} is invalid. Must be between 0 and 150") { }

    public InvalidAgeException(string message)
        : base(message) { }

    public InvalidAgeException(string message, Exception innerException)
        : base(message, innerException) { }
}

// Usage
if (age < 0 || age > 150)
    throw new InvalidAgeException(age);
```

### Exception with Additional Context

```csharp
public class DataAccessException : Exception
{
    public string? Query { get; }
    public int? RetryCount { get; }

    public DataAccessException(string message, string? query = null)
        : base(message)
    {
        Query = query;
    }

    public DataAccessException(
        string message,
        Exception innerException,
        string? query = null,
        int retryCount = 0)
        : base(message, innerException)
    {
        Query = query;
        RetryCount = retryCount;
    }
}

// Usage
try
{
    ExecuteQuery(sql);
}
catch (SqlException ex)
{
    throw new DataAccessException(
        "Failed to execute database query",
        ex,
        query: sql
    );
}
```

## Advanced Patterns

### Exception Re-throwing with Context

```csharp
try
{
    ProcessData(data);
}
catch (InvalidOperationException ex)
{
    // Add context and re-throw
    throw new InvalidOperationException(
        $"Failed to process data: {ex.Message}",
        ex
    );
}

// Better: Keep original stack trace
try
{
    ProcessData(data);
}
catch (InvalidOperationException ex) when (ShouldRetry(ex))
{
    throw; // Preserves stack trace
}
```

### Retry Pattern with Exponential Backoff

```csharp
public async Task<T> ExecuteWithRetry<T>(
    Func<Task<T>> operation,
    int maxRetries = 3,
    int initialDelayMs = 100)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await operation();
        }
        catch (TimeoutException) when (attempt < maxRetries)
        {
            var delay = initialDelayMs * (int)Math.Pow(2, attempt - 1);
            await Task.Delay(delay);
        }
    }

    throw new InvalidOperationException("Max retries exceeded");
}
```

### Exception Aggregation

```csharp
var exceptions = new List<Exception>();

foreach (var item in items)
{
    try
    {
        ProcessItem(item);
    }
    catch (Exception ex)
    {
        exceptions.Add(ex);
    }
}

if (exceptions.Count > 0)
    throw new AggregateException("Multiple errors occurred", exceptions);
```

### Logging and Reporting

```csharp
public static class ExceptionHandler
{
    public static void Handle(Exception ex, ILogger logger)
    {
        logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

        if (ex.InnerException != null)
            Handle(ex.InnerException, logger);
    }
}

// Usage
try
{
    PerformOperation();
}
catch (Exception ex)
{
    ExceptionHandler.Handle(ex, logger);
    // Optionally re-throw or return error response
}
```

## Best Practices

### 1. Use Specific Exception Types

```csharp
// ✅ GOOD - Specific exception
if (user == null)
    throw new ArgumentNullException(nameof(user));

// ❌ BAD - Generic exception
if (user == null)
    throw new Exception("User is null");
```

### 2. Catch Only What You Can Handle

```csharp
// ✅ GOOD
try
{
    connection.Open();
}
catch (SqlException ex)
{
    logger.LogError(ex, "Database connection failed");
    throw;
}
catch (OutOfMemoryException)
{
    // Re-throw - can't handle out of memory
    throw;
}

// ❌ BAD
try
{
    SomeOperation();
}
catch (Exception)
{
    // Silently swallow all exceptions
}
```

### 3. Add Context Information

```csharp
// ✅ GOOD
try
{
    user.SetAge(age);
}
catch (ArgumentOutOfRangeException ex)
{
    throw new InvalidAgeException(
        $"Invalid age '{age}' for user '{user.Name}'",
        ex
    );
}

// ❌ BAD
throw new Exception("Error");
```

### 4. Use Using for Resource Cleanup

```csharp
// ✅ GOOD
using (var file = File.OpenRead("path.txt"))
{
    // File auto-closed on exit
}

// ❌ BAD
var file = File.OpenRead("path.txt");
// File might not be closed if exception occurs
```

### 5. Don't Use Exceptions for Control Flow

```csharp
// ❌ BAD - Performance issue, poor design
try
{
    int id = int.Parse(input);
    user = GetUserById(id);
}
catch (FormatException)
{
    user = null;
}

// ✅ GOOD - Control flow with return values
if (int.TryParse(input, out int id))
{
    user = GetUserById(id);
}
else
{
    user = null;
}
```

### 6. Preserve Stack Trace When Re-throwing

```csharp
// ✅ GOOD - Preserves original stack trace
try
{
    SomeOperation();
}
catch (ArgumentException) when (ShouldLogAndRethrow())
{
    throw; // Preserves stack trace
}

// ❌ BAD - Loses original stack trace
try
{
    SomeOperation();
}
catch (ArgumentException ex)
{
    throw ex; // Stack trace starts here
}
```

## Common Pitfalls

### Pitfall 1: Silent Exception Swallowing

```csharp
// ❌ BAD
try
{
    ImportantOperation();
}
catch (Exception)
{
    // Silent failure - hard to debug!
}

// ✅ GOOD
try
{
    ImportantOperation();
}
catch (Exception ex)
{
    logger.LogError(ex, "Operation failed");
    throw;
}
```

### Pitfall 2: Generic Catch-All

```csharp
// ❌ BAD
try
{
    Process();
}
catch (Exception ex)
{
    return new ErrorResult(ex.Message);
}

// ✅ GOOD
try
{
    Process();
}
catch (ValidationException ex)
{
    return new ErrorResult(ex.Message);
}
catch (DataAccessException ex)
{
    logger.LogError(ex, "Database error");
    throw;
}
```

### Pitfall 3: Missing Inner Exception

```csharp
// ❌ BAD - Loses original context
catch (Exception ex)
{
    throw new ApplicationException(ex.Message);
}

// ✅ GOOD - Preserves original exception
catch (Exception ex)
{
    throw new ApplicationException("Operation failed", ex);
}
```

### Pitfall 4: Using Exceptions for Normal Flow

```csharp
// ❌ SLOW and WRONG
public User GetUser(int id)
{
    try
    {
        return _users[id];
    }
    catch (KeyNotFoundException)
    {
        return null;
    }
}

// ✅ FAST and CORRECT
public User? GetUser(int id)
{
    return _users.TryGetValue(id, out var user) ? user : null;
}
```

## Key Takeaways

- Use specific exception types for different errors
- Only catch exceptions you can handle
- Add context information when throwing
- Use 'using' for resource cleanup
- Avoid using exceptions for control flow
- Preserve stack traces when re-throwing
- Log exceptions appropriately
- Create custom exceptions for domain errors
- Aggregate exceptions when appropriate
- Never silently swallow exceptions without logging
