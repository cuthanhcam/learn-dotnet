---
title: "Core .NET Learning Roadmap"
description: "The ordered study path, checkpoints, and practice workflow for Phase 03."
slug: core-dotnet-roadmap
phase: 3
order: 0
difficulty: intermediate
article-type: roadmap
estimated-reading-minutes: 10
topics: [dotnet, standard-library, roadmap]
prerequisites: [oop-common-pitfalls]
status: maintained
last-reviewed: 2026-08-15
---

# 📋 Roadmap: Core .NET & Standard Library Learning Path

## Overview

This section builds on OOP fundamentals to explore the essential libraries and patterns that make .NET powerful. We'll progress from foundational collection concepts to advanced patterns for null safety and metadata.

## Learning Path

### Phase 1: Collections & Data Structures (Topics 1)

**Foundation**: Understand how .NET manages collections and when to use each type

1. **Collections** - `List<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`
   - Collection interfaces: `IEnumerable<T>`, `ICollection<T>`, `IList<T>`, `IDictionary<TKey, TValue>`
   - Performance characteristics (O(n) operations, memory overhead)
   - Collection initialization and LINQ integration

**Expected Competencies:**

- Choose appropriate collection types for scenarios
- Understand collection performance characteristics
- Use collection interfaces correctly
- Initialize and manipulate collections efficiently

---

### Phase 2: Generic Programming (Topic 2)

**Foundation**: Master type safety and reusability through generics

2. **Generics** - Generic types, constraints, variance
   - Generic class and method declarations
   - Type parameters and constraints (`where` clause)
   - Covariance (`out`) and contravariance (`in`)
   - Runtime behavior and type erasure

**Expected Competencies:**

- Write generic classes and methods
- Apply appropriate generic constraints
- Understand variance in generic types
- Design reusable generic solutions

---

### Phase 3: Error Handling (Topic 3)

**Foundation**: Build robust applications with proper exception handling

3. **Exception Handling** - try-catch-finally, custom exceptions
   - Exception hierarchy and built-in exceptions
   - try-catch-finally patterns
   - Custom exception creation
   - Exception propagation and re-throwing
   - Filter expressions (C# 6+)

**Expected Competencies:**

- Write defensive code with proper error handling
- Create custom exceptions for domain errors
- Use exceptions appropriately (not for control flow)
- Implement exception propagation strategies

---

### Phase 4: LINQ - Query Language (Topic 4)

**Foundation**: Master data querying and transformation with LINQ

4. **LINQ** - Query syntax, method syntax, operators, performance
   - LINQ to Objects fundamentals
   - Query vs method syntax
   - Common operators: Select, Where, GroupBy, Join, OrderBy, etc.
   - Deferred execution and lazy evaluation
   - Materialization and immediate operators

**Expected Competencies:**

- Write complex LINQ queries
- Understand deferred vs immediate execution
- Use advanced LINQ operators effectively
- Optimize LINQ queries for performance
- Handle null values in LINQ chains

---

### Phase 5: Event-Driven Architecture (Topic 5)

**Foundation**: Implement publish-subscribe patterns with delegates and events

5. **Delegates & Events** - Functional programming patterns
   - Delegate types and declarations
   - Action<T>, Func<T, TResult> delegates
   - Event publishers and subscribers
   - Event handler patterns
   - Custom event arguments

**Expected Competencies:**

- Create custom delegates
- Implement event publishers
- Subscribe to and unsubscribe from events
- Use Lambda expressions with delegates
- Design event-driven systems

---

### Phase 6: File System Operations (Topic 6)

**Foundation**: Handle file I/O safely and efficiently

6. **File I/O** - Streams, file operations, async operations
   - Stream abstractions and classes
   - FileStream, StreamReader, StreamWriter
   - Text vs binary file operations
   - Path manipulation and file system navigation
   - Async file operations (async/await)

**Expected Competencies:**

- Read and write files efficiently
- Use appropriate stream types
- Handle file system operations safely
- Implement async file operations
- Manipulate file paths correctly

---

### Phase 7: Date & Time Handling (Topic 7)

**Foundation**: Master temporal operations and timezone awareness

7. **DateTime & TimeZone** - Date operations, timezone handling
   - DateTime, DateTimeOffset, TimeSpan
   - Timezone conversions
   - DateTime formatting and parsing
   - Duration calculations
   - UTC vs local time

**Expected Competencies:**

- Work with dates and times correctly
- Handle timezones appropriately
- Parse and format dates
- Calculate durations and intervals
- Avoid common timezone pitfalls

---

### Phase 8: Metadata with Attributes (Topic 8)

**Foundation**: Leverage reflection and metadata for advanced scenarios

8. **Attributes** - Custom attributes, reflection-based usage
   - Built-in attributes
   - Custom attribute creation
   - Attribute targets and usage
   - Reflection-based attribute querying
   - Serialization and data attributes

**Expected Competencies:**

- Create custom attributes
- Query attributes via reflection
- Use attributes for code generation
- Implement attribute-driven behavior
- Apply attributes to enable frameworks

---

### Phase 9: Null Safety (Topic 9)

**Foundation**: Write code with explicit null handling

9. **Nullable Reference Types** - Null safety annotations
   - Nullable reference type annotations
   - Null-forgiving operator (!)
   - Nullable contexts
   - Null-coalescing (??) and null-conditional (?.) operators
   - Handling nullable APIs

**Expected Competencies:**

- Enable and understand nullable annotations
- Write null-safe code
- Handle nullable types appropriately
- Migrate existing code to nullable
- Debug null reference exceptions

---

## 📊 Difficulty Progression

```
Beginner          Intermediate              Advanced
    ↓                  ↓                        ↓
Collections    Exception Handling      Generics
DateTime       File I/O                LINQ
Nullable       Delegates/Events        Attributes
                                       Custom Patterns
```

## 🎯 Capstone Project Ideas

After completing this section, you should be able to build:

1. **Data Processing Pipeline**
   - Read CSV file → Parse with generics → LINQ queries → Export results
   - Topics: Collections, Generics, File I/O, LINQ

2. **Event-Driven Logger**
   - Log system events using events and delegates
   - Write logs to file with different formatters
   - Topics: Delegates/Events, File I/O, Attributes

3. **DateTime Query Tool**
   - Parse dates, handle timezones, calculate durations
   - Generate reports for different time periods
   - Topics: DateTime, LINQ, File I/O

4. **Configuration Framework**
   - Custom attributes for configuration
   - Reflection-based attribute processing
   - Type-safe configuration with generics
   - Topics: Attributes, Generics, Reflection

## 📚 Common Pitfalls to Avoid

1. **Collections**: Using wrong collection type for scenario
2. **Generics**: Over-constraining or under-constraining types
3. **Exceptions**: Using for control flow
4. **LINQ**: Creating unnecessary materializations
5. **Events**: Memory leaks from not unsubscribing
6. **File I/O**: Not disposing streams
7. **DateTime**: Ignoring timezones
8. **Attributes**: Excessive reflection overhead
9. **Nullable**: Ignoring null possibilities
