---
title: "File I/O and Streams"
description: "Paths, files, streams, encoding, buffering, disposal, atomicity, and safe filesystem boundaries."
slug: dotnet-file-io-streams
phase: 3
order: 6
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 26
topics: [dotnet, files, streams]
prerequisites: [csharp-construction-finalization-disposal, dotnet-exception-handling]
status: maintained
last-reviewed: 2026-08-15
---

# 💾 File I/O: Reading and Writing Files

## Overview

File I/O operations are essential for working with the file system. This section covers streams, file operations, and async patterns.

## Table of Contents

1. [Stream Basics](#stream-basics)
2. [Reading Files](#reading-files)
3. [Writing Files](#writing-files)
4. [Working with Paths](#working-with-paths)
5. [Async File Operations](#async-file-operations)
6. [Best Practices](#best-practices)
7. [Common Pitfalls](#common-pitfalls)

## Stream Basics

### Stream Hierarchy

```
Stream (abstract base)
├── FileStream - File operations
├── MemoryStream - In-memory buffer
├── NetworkStream - Network operations
└── ... (other specialized streams)

TextWriter / TextReader (text wrappers)
├── StreamWriter / StreamReader
├── StringWriter / StringReader
└── File.CreateText() / File.OpenText()
```

### Reading from Stream

```csharp
using (var stream = File.OpenRead("file.txt"))
{
    byte[] buffer = new byte[1024];
    int bytesRead = stream.Read(buffer, 0, buffer.Length);

    // Process buffer
    string content = Encoding.UTF8.GetString(buffer, 0, bytesRead);
}
```

## Reading Files

### Reading All Text

```csharp
// Read entire file as string
string content = File.ReadAllText("file.txt");
string content = File.ReadAllText("file.txt", Encoding.UTF8);

// Read all lines as array
string[] lines = File.ReadAllLines("file.txt");

// Read all bytes
byte[] bytes = File.ReadAllBytes("file.txt");
```

### Reading Line by Line

```csharp
// Efficient for large files
foreach (string line in File.ReadLines("file.txt"))
{
    ProcessLine(line);
}

// Manual reading
using (var reader = File.OpenText("file.txt"))
{
    string? line;
    while ((line = reader.ReadLine()) != null)
    {
        ProcessLine(line);
    }
}
```

### Reading Binary Data

```csharp
using (var stream = File.OpenRead("data.bin"))
using (var reader = new BinaryReader(stream))
{
    int version = reader.ReadInt32();
    string name = reader.ReadString();
    double value = reader.ReadDouble();
}
```

## Writing Files

### Writing Text

```csharp
// Write all text (overwrites)
File.WriteAllText("file.txt", "Hello, World!");

// Write lines
File.WriteAllLines("file.txt", new[] { "Line 1", "Line 2" });

// Append
File.AppendAllText("file.txt", "New line\n");
File.AppendAllLines("file.txt", new[] { "Line 3", "Line 4" });
```

### Writing with StreamWriter

```csharp
using (var writer = File.CreateText("file.txt"))
{
    writer.WriteLine("Line 1");
    writer.WriteLine("Line 2");
} // File closed and flushed automatically

// Append mode
using (var writer = File.AppendText("file.txt"))
{
    writer.WriteLine("Appended line");
}
```

### Writing Binary Data

```csharp
using (var stream = File.Create("data.bin"))
using (var writer = new BinaryWriter(stream))
{
    writer.Write(42);        // Int32
    writer.Write("Hello");   // String
    writer.Write(3.14);      // Double
}
```

## Working with Paths

### Path Manipulation

```csharp
string path = @"C:\Users\John\Documents\file.txt";

string directory = Path.GetDirectoryName(path);      // C:\Users\John\Documents
string fileName = Path.GetFileName(path);             // file.txt
string nameWithoutExt = Path.GetFileNameWithoutExtension(path); // file
string extension = Path.GetExtension(path);           // .txt

string fullPath = Path.GetFullPath("file.txt");      // Absolute path
string combined = Path.Combine(@"C:\Users", "John", "file.txt");
```

### Cross-Platform Paths

```csharp
// Use Path.Combine for cross-platform compatibility
string path = Path.Combine("data", "subfolder", "file.txt");

// DirectorySeparatorChar for current platform
string backslash = Path.DirectorySeparatorChar; // \ on Windows, / on Unix

// Use forward slash - .NET normalizes automatically
string path = "data/subfolder/file.txt";
```

### Checking Existence

```csharp
if (File.Exists("file.txt"))
{
    // File exists
}

if (Directory.Exists("folder"))
{
    // Directory exists
}
```

## Async File Operations

### Async Reading

```csharp
// Async read all text
string content = await File.ReadAllTextAsync("file.txt");

// Async read all lines
string[] lines = await File.ReadAllLinesAsync("file.txt");

// Async read bytes
byte[] bytes = await File.ReadAllBytesAsync("file.txt");
```

### Async Writing

```csharp
// Async write
await File.WriteAllTextAsync("file.txt", content);

// Async append
await File.AppendAllTextAsync("file.txt", newContent);

// Async write lines
await File.WriteAllLinesAsync("file.txt", lines);
```

### Async StreamReader/Writer

```csharp
// Async reading
using (var reader = new StreamReader("file.txt"))
{
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        await ProcessLineAsync(line);
    }
}

// Async writing
using (var writer = new StreamWriter("file.txt"))
{
    await writer.WriteLineAsync("Content");
}
```

## Best Practices

### 1. Always Use 'Using' for Streams

```csharp
// ✅ GOOD - Stream automatically closed
using (var stream = File.OpenRead("file.txt"))
{
    // Use stream
} // Disposed here

// ✅ GOOD - C# 8+ using declaration
using var stream = File.OpenRead("file.txt");
// Use stream
// Disposed when leaving scope

// ❌ BAD - No disposal
var stream = File.OpenRead("file.txt");
// Potential resource leak
```

### 2. Choose Right Encoding

```csharp
// ✅ GOOD - Specify encoding
File.WriteAllText("file.txt", content, Encoding.UTF8);

// ⚠️ WARNING - Uses default encoding
File.WriteAllText("file.txt", content);

// Handle different encodings when reading
var encoding = DetectEncoding("file.txt");
string content = File.ReadAllText("file.txt", encoding);
```

### 3. Buffer Sizes Matter

```csharp
// ✅ GOOD - Reasonable buffer
const int bufferSize = 8192;
using (var stream = File.OpenRead("large.bin"))
{
    byte[] buffer = new byte[bufferSize];
    int bytesRead;
    while ((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0)
    {
        ProcessBuffer(buffer, bytesRead);
    }
}

// ❌ BAD - Tiny buffer, inefficient
byte[] buffer = new byte[1];
```

### 4. Use Async for I/O

```csharp
// ✅ GOOD - Async prevents blocking
public async Task ProcessFileAsync(string path)
{
    string content = await File.ReadAllTextAsync(path);
    await ProcessAsync(content);
}

// ❌ BAD - Blocks thread
public void ProcessFile(string path)
{
    string content = File.ReadAllText(path);
    Process(content);
}
```

### 5. Handle Exceptions

```csharp
// ✅ GOOD - Specific exception handling
try
{
    string content = File.ReadAllText("file.txt");
}
catch (FileNotFoundException)
{
    Console.WriteLine("File not found");
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("Access denied");
}
catch (IOException ex)
{
    Console.WriteLine($"IO error: {ex.Message}");
}
```

## Common Pitfalls

### Pitfall 1: File Still Open

```csharp
// ❌ WRONG - File locked while processing
var content = File.ReadAllText("file.txt");
var lines = content.Split('\n');
// File is still locked for other processes

// ✅ CORRECT - Read into memory first
var lines = File.ReadAllLines("file.txt");
// File is now released
```

### Pitfall 2: Large File in Memory

```csharp
// ❌ BAD - Loads entire large file
string[] gigabytesOfData = File.ReadAllLines("huge.txt");

// ✅ GOOD - Stream line by line
foreach (var line in File.ReadLines("huge.txt"))
{
    ProcessLine(line);
}
```

### Pitfall 3: Not Flushing

```csharp
// ❌ WRONG - Data might not be written
using (var writer = File.CreateText("file.txt"))
{
    writer.WriteLine("Data");
    // Exit without explicit flush
}

// ✅ CORRECT - Explicit flush
using (var writer = File.CreateText("file.txt"))
{
    writer.WriteLine("Data");
    writer.Flush();
}
```

### Pitfall 4: Hard-coded Paths

```csharp
// ❌ BAD - Hard-coded, not portable
string path = @"C:\Users\John\file.txt";

// ✅ GOOD - Uses environment variables
string path = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "file.txt"
);

// ✅ GOOD - Relative to application
string path = Path.Combine(
    AppContext.BaseDirectory,
    "data",
    "file.txt"
);
```

## Key Takeaways

- Always use 'using' for streams and file operations
- Use appropriate encoding when working with text
- Stream large files instead of loading into memory
- Use async operations for I/O
- Handle specific exceptions appropriately
- Use Path.Combine for portable paths
- Be aware of file locking issues
- Flush writers to ensure data is written
- Choose appropriate buffer sizes
- Consider cross-platform compatibility
