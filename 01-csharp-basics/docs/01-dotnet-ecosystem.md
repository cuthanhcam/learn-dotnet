# 🌍 .NET Ecosystem Fundamentals

> Understand how C# and .NET work together, and why this matters

---

## What is .NET?

**.NET** is a **software platform** that runs C# (and other languages).

Think of it like:
- **Language**: C# (what you write)
- **Platform**: .NET (how it runs)
- **Relationship**: Like JavaScript & Node.js, Java & JVM

---

## Different .NET Versions

### ❌ .NET Framework (Legacy)
- Old version (Windows only, 2002-2023)
- Don't learn this

### ✅ .NET Core (Modern, 2016+)
- Cross-platform (Windows, Mac, Linux)
- Fast, lightweight
- Version 3.1, 5, 6, 7, 8...

### ✅ .NET (Current Name, 2020+)
- Renamed from ".NET Core"
- Latest: **.NET 8 LTS** (Long-Term Support)
- What you should use

### 📦 What We Use
- **.NET 8 LTS** - Stable, supported until 2026

---

## How .NET Works

```
Your C# Code
    ↓
C# Compiler (csc.exe)
    ↓
IL Code (Intermediate Language)  ← Platform independent
    ↓
Common Language Runtime (CLR)     ← .NET engine
    ↓
JIT Compiler
    ↓
Machine Code (CPU executes)
    ↓
Program Output
```

Let's break this down:

---

## 1️⃣ C# Compiler

**What it does**: Converts C# → **IL (Intermediate Language)**

```
C# Code:
--------
int x = 10;
int y = x + 5;
Console.WriteLine(y);

↓ (Compiler converts to IL)

IL Code:
--------
ldc.i4.s  10     // Load constant 10
stloc.0          // Store in variable x
ldloc.0          // Load x
ldc.i4.s  5      // Load constant 5
add              // Add them
stloc.1          // Store in y
...
```

**Key point**: C# code doesn't run directly — it compiles to IL first.

---

## 2️⃣ IL (Intermediate Language)

**What is it?**
- Platform-independent bytecode
- Can run on Windows, Mac, Linux
- Can be read by debuggers
- Can be reverse-engineered (beware!)

**Why IL exists:**
- ✅ Platform independence
- ✅ JIT can optimize at runtime
- ✅ Multiple languages can compile to IL (C#, VB.NET, F#)

---

## 3️⃣ Common Language Runtime (CLR)

**What is it?** The **engine** that runs .NET programs.

**What CLR does:**
1. Loads IL code
2. Manages memory (Garbage Collection)
3. Enforces type safety
4. Handles exceptions
5. Manages threading
6. Provides security

**Like:** The JVM for Java, or V8 for JavaScript

---

## 4️⃣ JIT (Just-In-Time) Compilation

**What it does**: Converts IL → Machine Code at runtime

```
IL Code (when program starts)
    ↓
JIT Compiler (in CLR)
    ↓
Optimized Machine Code
    ↓
CPU executes
    ↓
Results
```

### Why JIT Exists

✅ **Runtime optimization**
- JIT sees actual data flow
- Can make better optimizations
- Faster than ahead-of-time compilation

✅ **Platform independence**
- Same IL runs on Windows, Mac, Linux
- JIT compiles to native code per platform

---

## 5️⃣ Assemblies

**What is an Assembly?**
- A compiled .NET program or library
- File extension: `.dll` (library) or `.exe` (executable)
- Contains: IL code + metadata + resources

**Structure of an Assembly:**
```
MyAssembly.dll
├── IL Code (the actual program)
├── Metadata (types, methods, properties info)
└── Resources (images, strings, etc.)
```

### Assembly Names

```bash
LearnDotnet.CSharpBasics.dll
    ↑
    └─ Namespace.ProjectName.FileType
```

### Where are assemblies?

```
bin/
├── Debug/  or Release/
    ├── net8.0/
        ├── CSharpBasics.ConsoleApp.dll
        ├── CSharpBasics.Examples.dll
        └── ... (all dependencies)
```

---

## 6️⃣ Namespaces

**What is a Namespace?**
- Logical grouping of code
- Prevents name conflicts
- Like folders for classes

### Example

```csharp
namespace LearnDotnet.CSharpBasics
{
    class Variables
    {
        // Code here
    }
}
```

### Using Namespaces

```csharp
// Option 1: Full name
LearnDotnet.CSharpBasics.Variables.Run();

// Option 2: Import with 'using'
using LearnDotnet.CSharpBasics;

Variables.Run();  // Shorter
```

### Common Namespaces

```csharp
using System;              // Console, string, int
using System.Collections.Generic;  // List<T>, Dictionary<K,V>
using System.Linq;         // LINQ queries
using System.Text;         // StringBuilder
```

---

## 7️⃣ Base Class Library (BCL)

**What is it?** Pre-built classes that come with .NET

```csharp
Examples:
- System.String
- System.List<T>
- System.Dictionary<K,V>
- System.DateTime
- System.IO.File
- ... thousands more
```

---

## Complete Flow Diagram

```
┌─────────────────────────────────────────────────────┐
│                C# Source Code                        │
│  (Variables.cs, ControlFlow.cs, etc.)               │
└────────────────────┬────────────────────────────────┘
                     │ C# Compiler
                     ↓
┌─────────────────────────────────────────────────────┐
│                IL Code                               │
│  (MyProgram.dll)                                    │
│  - Platform independent                             │
│  - Metadata included                                │
└────────────────────┬────────────────────────────────┘
                     │ Common Language Runtime (CLR)
                     │ - Loads assembly
                     │ - Manages memory
                     ↓
┌─────────────────────────────────────────────────────┐
│           JIT Compiler (runtime)                    │
│  Converts IL → Optimized Machine Code              │
└────────────────────┬────────────────────────────────┘
                     │
                     ↓
        ┌─────────────────────────────┐
        │  Native Machine Code        │
        │  (Windows/Mac/Linux specific)      │
        └─────────────────────────────┘
                     │
                     ↓
┌─────────────────────────────────────────────────────┐
│           CPU Executes Code                         │
│           → Program Output                          │
└─────────────────────────────────────────────────────┘
```

---

## Key Takeaways

### ✅ C# vs .NET

| Aspect | C# | .NET |
|--------|------|-----------|
| **What is it?** | Programming language | Software platform |
| **Role** | Write code | Run code |
| **Compile? Yes** | .cs files | → .dll files |

### ✅ Compilation Model

1. **C# Compiler** → converts to IL
2. **CLR** → loads IL + manages runtime
3. **JIT** → converts IL to native code
4. **CPU** → executes

### ✅ Why This Matters

| Benefit | Impact |
|---------|--------|
| Platform-independent IL | Run on Windows/Mac/Linux |
| Runtime JIT compilation | Better performance than ahead-of-time |
| Managed memory (GC) | No manual memory management |
| Type safety (CLR enforces) | Fewer runtime errors |

---

## Quick Questions

### Q1: What happens when I run a program?

```
1. CLR loads your .dll
2. CLR discovers your Main() method
3. JIT compiles Main() to machine code
4. CPU executes machine code
5. If Main calls another method, JIT compiles that too
```

### Q2: Can I run the same .dll on Windows and Mac?

✅ **Yes!** The IL is platform-independent. CLR on each OS handles the JIT compilation to platform-specific code.

```
myprogram.dll (on Windows)
    ↓ CLR + JIT (window-specific)
    ↓ Native Windows Code
    ↓ Run

myprogram.dll (on Mac)
    ↓ CLR + JIT (Mac-specific)
    ↓ Native Mac Code
    ↓ Run
```

### Q3: How do I see IL code?

Use **ILDasm.exe** (IL Disassembler):

```bash
# Windows
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\ildasm.exe" MyAssembly.dll

# macOS/Linux
dotnet ildasm MyAssembly.dll
```

But don't worry about this yet!

---

## Next Steps

- ✅ Understand this document
- ⬜ Move to `02-variables-types.md`
- ⬜ Study examples: `src/CSharpBasics.Examples/Variables/`

---

## References

- [Official .NET Docs](https://learn.microsoft.com/en-us/dotnet/)
- [Common Language Runtime (CLR)](https://learn.microsoft.com/en-us/dotnet/standard/clr)
- [IL and .NET](https://learn.microsoft.com/en-us/dotnet/standard/managed-code)
