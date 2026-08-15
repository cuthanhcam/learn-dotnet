---
title: ".NET Memory Model"
description: "Stack frames, managed objects, value/reference semantics, copying, and object lifetime."
phase: 4
order: 1
topics: [dotnet, memory, stack, heap]
---

# Memory Model

## The Core Question

When you write C#, you usually think in variables and objects. The runtime thinks in stack frames, references, object headers, arrays, fields, and reachable graphs. A good .NET developer does not need to memorize every runtime detail, but does need a practical model for copies, lifetime, and identity.

This chapter answers:

- What lives in a stack frame?
- What lives on the managed heap?
- What does assignment copy?
- Why can value types still be inside heap objects?
- Why is lifetime based on reachability rather than lexical scope alone?

## Stack Frames

Each active method call has a frame containing temporary method state: parameters, local variables, return addresses, and bookkeeping data. Stack allocation is fast because frames are pushed and popped in a strict order.

```csharp
public static int Add(int left, int right)
{
    int result = left + right;
    return result;
}
```

The local `result` has a very predictable lifetime. When the method returns, the frame is gone.

Important nuance: saying "local variable" does not always mean "stack only". A local can be captured by a lambda or async state machine, and then the compiler may move the required state into a heap object.

## Managed Heap

The managed heap stores objects controlled by the CLR garbage collector. Classes, arrays, strings, delegates, and boxed values are heap objects. Heap objects can outlive the method that created them as long as something can still reach them.

```csharp
var customer = new Customer("Mina", 10);
```

The variable `customer` contains a reference. The object itself is on the managed heap.

## Value Semantics

Value types are copied by value. Common value types include:

- primitive numeric types such as `int`, `double`, and `decimal`
- `bool`, `char`, `DateTime`, `Guid`
- `struct` and `record struct`

```csharp
var original = new Point(2, 4);
var copy = original;
copy = copy with { X = 99 };
```

`original` and `copy` are independent values. Mutating or replacing one does not change the other.

The example `MemoryModelExample.ValueTypeCopyExample()` demonstrates this behavior.

## Reference Semantics

Reference types are copied by reference. The variable is copied, but the object is not duplicated.

```csharp
var first = new Customer("Mina", 10);
var second = first;
second.Name = "Updated";
```

Both variables point to the same heap object. Mutating through `second` is visible through `first`.

The example `MemoryModelExample.ReferenceAliasExample()` demonstrates this behavior.

## Value Type Does Not Mean Stack

This is one of the most common .NET misconceptions.

More accurate rules:

- Value types are copied by value.
- Reference types are copied by reference.
- Storage location depends on where the value is contained.

Examples:

```csharp
int local = 42;                 // usually in the current frame or register
int[] values = [1, 2, 3];       // array object on heap, ints inside array storage
var customer = new Customer();  // customer object on heap, value-type fields inside it
object boxed = 42;              // boxed int is a heap object
```

The important habit is to ask: "When this value is assigned, passed, returned, captured, or boxed, what gets copied?"

## Passing Values

By default, method arguments are passed by value.

```csharp
void Move(Point point)
{
    point = point with { X = point.X + 1 };
}
```

The method receives a copy of `Point`.

For large structs, copying can become measurable. C# offers `in`, `ref`, and `out` modifiers:

- `in` passes by readonly reference
- `ref` passes by writable reference
- `out` is used for values assigned by the callee

Use these carefully. They improve performance only when the copy cost matters and the API remains understandable.

## Object Lifetime

An object is alive while reachable from a root. Common GC roots include:

- active stack references
- static fields
- CPU registers tracked by the runtime
- pending finalizer references
- handles used by interop or runtime infrastructure

When an object becomes unreachable, it is eligible for collection. Eligible does not mean immediately collected.

```csharp
Customer Create()
{
    return new Customer("Ava", 5);
}
```

If the caller stores the returned customer, it remains reachable. If nobody stores it, it can be collected later.

## Why This Matters

Memory performance problems often start as semantic misunderstandings:

- accidental aliasing causes shared mutation bugs
- boxing creates hidden heap objects
- large structs are copied more than expected
- captured locals survive longer than expected
- arrays and strings allocate even when their syntax is compact

The rest of this module builds on this model.

## Practice

1. Run `MemoryModelExample.Run()`.
2. Predict which values are independent and which share identity.
3. Change `Point` from `record struct` to class locally and observe how the mental model changes.
4. Add a lambda that captures a local variable, then inspect allocation behavior in a benchmark.
