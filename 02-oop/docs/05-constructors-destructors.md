# Constructors & Destructors

## Constructors
A constructor is a special method called when an object is created. It initializes the object’s state.

```csharp
public class Person
{
	public string Name;
	public Person(string name) { Name = name; }
}
var p = new Person("Alice");
```

## Destructors (Finalizers)
A destructor cleans up resources before the object is destroyed (rarely needed in C#).
```csharp
class Resource
{
	~Resource() { /* cleanup code */ }
}
```

## Object Lifecycle
- **Constructor**: Allocates and initializes
- **Destructor**: Cleans up (called by GC, not deterministic)

## IDisposable
For deterministic cleanup (e.g., files, DB connections), implement `IDisposable`:
```csharp
public class FileManager : IDisposable
{
	public void Dispose() { /* cleanup */ }
}
```
Use with `using` statement:
```csharp
using (var fm = new FileManager())
{
	// use fm
}
```

## Practice Exercise

**Task:**
1. Create a class with a constructor and destructor.
2. Implement IDisposable for resource cleanup.

## Interview Questions
- What is a constructor? Can you overload it?
- When do you need a destructor?
- What is IDisposable and why is it important?

## Pro Tips
- Prefer IDisposable for cleanup over destructors.
- Always release unmanaged resources.
