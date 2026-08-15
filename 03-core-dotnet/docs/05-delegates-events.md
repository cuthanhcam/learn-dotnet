---
title: "Delegates and Events"
description: "Delegate types, lambdas, closures, multicast invocation, event contracts, and subscriber lifetime."
slug: csharp-delegates-events-closures
phase: 3
order: 5
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 25
topics: [csharp, delegates, events]
prerequisites: [csharp-methods-and-parameters, csharp-classes-objects-encapsulation]
status: maintained
last-reviewed: 2026-08-15
---

# 📢 Delegates & Events: Event-Driven Programming

## Overview

Delegates and events enable event-driven architecture and publish-subscribe patterns. This section covers delegate types, event patterns, and best practices.

## Table of Contents

1. [Delegates Fundamentals](#delegates-fundamentals)
2. [Action and Func](#action-and-func)
3. [Events](#events)
4. [Event Patterns](#event-patterns)
5. [Advanced Scenarios](#advanced-scenarios)
6. [Best Practices](#best-practices)
7. [Common Pitfalls](#common-pitfalls)

## Delegates Fundamentals

### What is a Delegate?

A delegate is a type-safe function pointer or callback mechanism:

```csharp
// Define a delegate type
public delegate void NotifyDelegate(string message);

// Method matching delegate signature
public static void PrintMessage(string message)
{
    Console.WriteLine(message);
}

// Use delegate
NotifyDelegate notify = PrintMessage;
notify("Hello"); // Calls PrintMessage
```

### Multicast Delegates

```csharp
public delegate void Operation(int x, int y);

public static void Add(int x, int y) => Console.WriteLine($"Sum: {x + y}");
public static void Multiply(int x, int y) => Console.WriteLine($"Product: {x * y}");

Operation op = Add;
op += Multiply; // Add another delegate

op(5, 3);
// Output:
// Sum: 8
// Product: 15

op -= Add; // Remove Add delegate
```

## Action and Func

### Action<T> - Return Void

```csharp
// Action takes parameters, returns void
Action<string> greet = name => Console.WriteLine($"Hello, {name}");
greet("Alice"); // Hello, Alice

Action<int, int> add = (x, y) => Console.WriteLine(x + y);
add(2, 3); // 5

// Action with no parameters
Action start = () => Console.WriteLine("Started");
start();
```

### Func<T, TResult> - Return Value

```csharp
// Func takes parameters, returns value (last type parameter)
Func<int, int> square = x => x * x;
int result = square(5); // 25

Func<int, int, int> multiply = (x, y) => x * y;
int product = multiply(3, 4); // 12

// Func with no parameters
Func<string> getMessage = () => "Hello";
string msg = getMessage();
```

### Predicate<T> - Boolean Return

```csharp
// Predicate returns bool
Predicate<int> isEven = x => x % 2 == 0;
bool result = isEven(4); // true

var numbers = new[] { 1, 2, 3, 4, 5 };
var evens = System.Array.FindAll(numbers, isEven); // {2, 4}
```

## Events

### Event Publisher

```csharp
public class Button
{
    // Define delegate type
    public delegate void ClickedHandler();

    // Declare event based on delegate
    public event ClickedHandler? Clicked;

    public void Click()
    {
        OnClicked();
    }

    protected virtual void OnClicked()
    {
        Clicked?.Invoke();
    }
}

// Usage
var button = new Button();
button.Clicked += () => Console.WriteLine("Button clicked!");
button.Click();
```

### Event Subscribers

```csharp
public class EventPublisher
{
    public event EventHandler? StateChanged;

    private string _state = "initial";
    public string State
    {
        get => _state;
        set
        {
            _state = value;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

// Subscribe
var publisher = new EventPublisher();
publisher.StateChanged += (sender, e) => Console.WriteLine("State changed!");
publisher.State = "new state"; // Triggers event
```

## Event Patterns

### Standard EventHandler Pattern

```csharp
// Define custom EventArgs
public class ProcessingEventArgs : EventArgs
{
    public int Progress { get; set; }
    public string Message { get; set; }
}

public class DataProcessor
{
    public event EventHandler<ProcessingEventArgs>? OnProgress;

    public void Process(int itemCount)
    {
        for (int i = 1; i <= itemCount; i++)
        {
            // Do work...

            OnProgress?.Invoke(this, new ProcessingEventArgs
            {
                Progress = (i * 100) / itemCount,
                Message = $"Processed {i} items"
            });
        }
    }
}

// Usage
var processor = new DataProcessor();
processor.OnProgress += (sender, args) =>
    Console.WriteLine($"Progress: {args.Progress}% - {args.Message}");
processor.Process(10);
```

### Action-Based Events

```csharp
public class FileWatcher
{
    public event Action<string>? FileCreated;
    public event Action<string>? FileDeleted;

    public void OnFileCreated(string fileName)
    {
        FileCreated?.Invoke(fileName);
    }

    public void OnFileDeleted(string fileName)
    {
        FileDeleted?.Invoke(fileName);
    }
}

// Usage
var watcher = new FileWatcher();
watcher.FileCreated += fileName => Console.WriteLine($"File created: {fileName}");
watcher.OnFileCreated("document.txt");
```

## Advanced Scenarios

### Weak Events (Prevent Memory Leaks)

```csharp
public class WeakEventPublisher
{
    private List<WeakReference> _subscribers = new();

    public event EventHandler? PropertyChanged;

    public void Subscribe(EventHandler handler)
    {
        PropertyChanged += handler;
        _subscribers.Add(new WeakReference(handler));
    }

    protected virtual void OnPropertyChanged()
    {
        // Remove dead references
        _subscribers.RemoveAll(wr => !wr.IsAlive);

        PropertyChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

### Event Aggregation

```csharp
public class EventAggregator
{
    private Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<T>(Action<T> handler) where T : class
    {
        var type = typeof(T);
        if (!_subscribers.ContainsKey(type))
            _subscribers[type] = new();

        _subscribers[type].Add(handler);
    }

    public void Publish<T>(T args) where T : class
    {
        var type = typeof(T);
        if (_subscribers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers.Cast<Action<T>>())
                handler(args);
        }
    }
}
```

## Best Practices

### 1. Always Check for Null

```csharp
// ✅ GOOD - Safe invocation
Clicked?.Invoke();

// ❌ BAD - Throws if null
Clicked.Invoke();
```

### 2. Use Standard Event Pattern

```csharp
// ✅ GOOD - Standard pattern
public event EventHandler<MyEventArgs>? Something;

protected virtual void OnSomething(MyEventArgs args)
{
    Something?.Invoke(this, args);
}

// ❌ BAD - Non-standard
public delegate void CustomHandler(int value);
public event CustomHandler? Something;
```

### 3. Unsubscribe to Prevent Leaks

```csharp
// ✅ GOOD
button.Clicked += OnButtonClicked;
// Later...
button.Clicked -= OnButtonClicked; // Unsubscribe

// ❌ BAD - Potential memory leak
button.Clicked += (sender, e) => { /* ... */ }; // Hard to unsubscribe
```

### 4. Use Virtual OnXxx Methods

```csharp
// ✅ GOOD - Derived classes can override
public class Button
{
    public event EventHandler? Clicked;

    protected virtual void OnClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}

// ❌ BAD - Can't override
public class Button
{
    public event EventHandler? Clicked;

    public void RaiseClicked()
    {
        Clicked?.Invoke(this, EventArgs.Empty);
    }
}
```

### 5. Immutable EventArgs

```csharp
// ✅ GOOD - Immutable
public class ButtonClickedArgs : EventArgs
{
    public DateTime ClickedAt { get; }
    public int ClickCount { get; }

    public ButtonClickedArgs(int clickCount)
    {
        ClickedAt = DateTime.Now;
        ClickCount = clickCount;
    }
}

// ❌ BAD - Mutable
public class ButtonClickedArgs : EventArgs
{
    public int ClickCount { get; set; }
}
```

## Common Pitfalls

### Pitfall 1: Memory Leak from Not Unsubscribing

```csharp
// ❌ WRONG
public class Form
{
    public Form()
    {
        button.Clicked += (s, e) => DoSomething();
        // Never unsubscribed - form held in memory
    }
}

// ✅ CORRECT
public class Form : IDisposable
{
    public Form()
    {
        button.Clicked += Button_OnClicked;
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        DoSomething();
    }

    public void Dispose()
    {
        button.Clicked -= Button_OnClicked;
    }
}
```

### Pitfall 2: Exception in Event Handler

```csharp
// ❌ BAD - First exception stops others
public void RaiseEvent()
{
    Occurred?.Invoke(this, EventArgs.Empty);
}

// ✅ GOOD - All handlers executed
public void RaiseEvent()
{
    var handlers = Occurred;
    if (handlers != null)
    {
        foreach (EventHandler handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Event handler failed");
            }
        }
    }
}
```

### Pitfall 3: Storing Delegates Permanently

```csharp
// ❌ BAD - Keeps references alive
private Action? _handler = () => Console.WriteLine("Handler");
_handler += () => Console.WriteLine("Another");
// _handler never cleared

// ✅ GOOD
EventHandler? handler = null;
handler += (s, e) => Console.WriteLine("Handler");
// Use handler, then allow it to be garbage collected
```

## Key Takeaways

- Use `Action<T>` for void returns
- Use `Func<T, TResult>` for return values
- Follow standard EventHandler pattern
- Always unsubscribe to prevent memory leaks
- Use `?.Invoke()` for safe event invocation
- Create immutable EventArgs
- Handle exceptions in event handlers
- Use virtual OnXxx methods
- Consider weak events for complex scenarios
- Document event behavior clearly
