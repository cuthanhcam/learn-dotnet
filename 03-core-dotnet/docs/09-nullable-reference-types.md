---
title: "Nullable Reference Types"
description: "Nullable annotations, flow analysis, API contracts, migration, and suppression hazards."
phase: 3
order: 9
topics: [csharp, nullability, api-design]
---

# ✅ Nullable Reference Types: Null Safety

## Overview

Nullable Reference Types (NRTs) provide compile-time null-safety checks. This section covers nullable annotations and best practices.

## Table of Contents

1. [NRT Basics](#nrt-basics)
2. [Annotations](#annotations)
3. [Null-Coalescing Operators](#null-coalescing-operators)
4. [Migration Path](#migration-path)
5. [Best Practices](#best-practices)
6. [Common Pitfalls](#common-pitfalls)

## NRT Basics

### Enabling NRTs

```csharp
// File-level enable
#nullable enable

public class User
{
    public string Name { get; set; } // Cannot be null
    public string? Email { get; set; } // Can be null
}

#nullable disable // Disable for rest of file
```

### Project-Level Enable

```xml
<!-- In .csproj file -->
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

### What Changes

```csharp
// Without NRT enabled
string? email; // Reference type, could be null

// With NRT enabled
string name;  // Cannot be null (must initialize)
string? email; // Can be null

public void ProcessUser(string name, string? email)
{
    // name: guaranteed not null
    // email: might be null
}
```

## Annotations

### Non-Nullable Reference (default with NRT)

```csharp
#nullable enable

public class User
{
    public string Name { get; set; } // Cannot be null

    public User(string name)
    {
        Name = name; // Must assign (or use init)
    }
}

var user = new User("Alice");
// user.Name is definitely not null
```

### Nullable Reference Type

```csharp
public class Profile
{
    public string? Bio { get; set; }  // Can be null
    public string? PhotoUrl { get; set; }
}

var profile = new Profile();
if (profile.Bio != null)
{
    Console.WriteLine(profile.Bio.Length); // Safe
}

// Without null check - compiler warning
Console.WriteLine(profile.Bio.Length); // Warning!
```

### Nullable Value Types

```csharp
#nullable enable

public struct Values
{
    public int Id { get; set; }      // Cannot be null
    public int? OptionalId { get; set; } // Can be null
}

// Usage
var val = new Values { Id = 1, OptionalId = null };
if (val.OptionalId.HasValue)
{
    int id = val.OptionalId.Value;
}
```

## Null-Coalescing Operators

### Null-Coalescing Operator (??)

```csharp
string? email = null;

// If email is null, use "unknown@example.com"
string result = email ?? "unknown@example.com";

// Chaining
string? primary = null;
string? secondary = null;
string result = primary ?? secondary ?? "default";
```

### Null-Conditional Operator (?.)

```csharp
public class User
{
    public Profile? Profile { get; set; }
}

public class Profile
{
    public string? Bio { get; set; }
}

User? user = null;

// Returns null if user is null
int? bioLength = user?.Profile?.Bio?.Length;

// Safe collection access
var firstName = user?.Name?[0];
```

### Combining Operators

```csharp
User? user = GetUser();

// Null-conditional + null-coalescing
string displayName = user?.Name ?? "Unknown";

// Null-conditional + method call
var emails = user?.GetEmails()?.ToList();

// Short-circuit evaluation
int length = user?.Profile?.Bio?.Length ?? 0;
```

### Null-Forgiving Operator (!)

```csharp
string? nullableName = "Alice";

// Tells compiler: "I know this is nullable, but trust me it's not null"
string name = nullableName!;

// Use sparingly - signals to code reviewer
if (user.Profile != null)
{
    var bio = user.Profile!.Bio; // Profile is definitely not null here
}
```

## Migration Path

### Step 1: Enable NRT Project-Wide

```xml
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>
```

### Step 2: Address Compiler Warnings

```csharp
// Before
public class User
{
    public string Name { get; set; }
}

// After - Initialize property
public class User
{
    public string Name { get; set; } = string.Empty;
}

// Or use required keyword (C# 11+)
public class User
{
    public required string Name { get; set; }
}
```

### Step 3: Mark Nullable Parameters/Returns

```csharp
// Before (ambiguous)
public User? GetUser(string id)
{
    return _users.FirstOrDefault(u => u.Id == id);
}

// After (clear)
public User? GetUser(string id)
{
    return _users.FirstOrDefault(u => u.Id == id);
}

// Nullable parameter
public void UpdateUser(User user, string? nickname)
{
    user.Nickname = nickname ?? user.FirstName;
}
```

### Step 4: Handle External Libraries

```csharp
#nullable enable

// External library returns nullable
var user = _externalApi.GetUser(id); // Might be null

// Compiler warns - handle appropriately
if (user != null)
{
    Process(user);
}
else
{
    LogWarning("User not found");
}

#nullable disable // Back to old behavior if needed
```

## Best Practices

### 1. Enable NRT Project-Wide

```xml
<!-- ✅ GOOD -->
<PropertyGroup>
    <Nullable>enable</Nullable>
</PropertyGroup>

<!-- ❌ BAD - Inconsistent null checking -->
<!-- File-by-file #nullable enable/disable -->
```

### 2. Clear Parameter Contracts

```csharp
// ✅ GOOD - Clear what can be null
public void UpdateUser(User user, string? nickname)
{
    ArgumentNullException.ThrowIfNull(user);
    // nickname can be null
}

// ❌ BAD - Ambiguous
public void UpdateUser(User user, string nickname)
{
    // Can nickname be null? Unknown!
}
```

### 3. Initialize Non-Nullable Properties

```csharp
// ✅ GOOD - Always initialized
public class User
{
    public string Name { get; set; } = string.Empty;
    public List<Email> Emails { get; set; } = new();
}

// ✅ GOOD - Required (C# 11+)
public class User
{
    public required string Name { get; set; }
}

// ❌ BAD - Not initialized, compiler warning
public class User
{
    public string Name { get; set; }
}
```

### 4. Use Null-Coalescing in APIs

```csharp
// ✅ GOOD - Defensive coding
public class UserService
{
    public string GetDisplayName(User? user)
    {
        return user?.Name ?? "Guest";
    }
}

// ❌ BAD - Will crash if user is null
public string GetDisplayName(User user)
{
    return user.Name;
}
```

### 5. Document Nullable Intent

```csharp
/// <summary>
/// Gets a user by ID.
/// </summary>
/// <param name="id">The user ID</param>
/// <returns>The user if found; null otherwise</returns>
public User? GetUser(string id)
{
    return _users.FirstOrDefault(u => u.Id == id);
}
```

## Common Pitfalls

### Pitfall 1: Ignoring Compiler Warnings

```csharp
// ❌ WRONG - Uninitialized non-nullable
public class User
{
    public string Name { get; set; }
    // Compiler warning: non-nullable property not initialized
}

// ✅ CORRECT - Initialize or allow null
public class User
{
    public string Name { get; set; } = string.Empty;
    // OR
    public string? Name { get; set; }
}
```

### Pitfall 2: Overusing Null-Forgiving Operator

```csharp
// ❌ BAD - Too many ! operators
string name = GetName()!.Trim()!.ToUpper()!;

// ✅ GOOD - Check once
var tempName = GetName();
if (tempName != null)
{
    string name = tempName.Trim().ToUpper();
}
```

### Pitfall 3: Unsafe Null Checks

```csharp
// ❌ BAD - Still could be null
string? email = GetEmail();
if (email != string.Empty) // Checks for empty, not null!
{
    Console.WriteLine(email.Length); // Warning!
}

// ✅ GOOD
if (!string.IsNullOrEmpty(email))
{
    Console.WriteLine(email.Length);
}
```

### Pitfall 4: Mixed NRT and Non-NRT Code

```csharp
// ❌ BAD - Mixing confuses intent
#nullable enable
public class Repository
{
    public User GetUser(string id) { } // Non-nullable return
}

public class Controller
{
    #nullable disable
    var user = _repo.GetUser(id); // Ambiguous intent
}

// ✅ GOOD - Consistent throughout
// Always enable at project level
```

### Pitfall 5: Trusting External Libraries

```csharp
// ❌ BAD - Trust library's annotations
var result = _apiClient.GetData(); // Might return null despite annotations
ProcessData(result!); // Dangerous!

// ✅ GOOD - Verify before using
var result = _apiClient.GetData();
if (result != null)
{
    ProcessData(result);
}
```

## Key Takeaways

- Enable NRT at project level
- Use `?` for potentially null reference types
- Initialize non-nullable properties
- Use `??` and `?.` operators
- Use `!` sparingly and carefully
- Document nullable intent in XML comments
- Check for null before using
- Provide default values with `??`
- Use safe method call patterns
- Migrate incrementally from unmanaged to managed nullability
