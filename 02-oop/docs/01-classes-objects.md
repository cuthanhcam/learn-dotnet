# Classes & Objects in C#

## What is a Class?
A **class** is a blueprint for creating objects. It defines the structure (fields/properties) and behavior (methods) that its objects will have.

**Analogy:**
- Class = Blueprint for a house
- Object = Actual house built from the blueprint

## What is an Object?
An **object** is an instance of a class. It has its own state (data) and can perform actions (methods).

## Declaring a Class
```csharp
public class Person
{
	public string Name;
	public int Age;
}
```

## Creating Objects
```csharp
var person = new Person();
person.Name = "Alice";
person.Age = 30;
```

## Fields and Properties
- **Fields** store data directly.
- **Properties** provide controlled access to fields (encapsulation).

```csharp
public class Car
{
	private string _model;
	public string Model
	{
		get { return _model; }
		set { _model = value; }
	}
}
```

## Methods
Methods define actions/behavior:
```csharp
public void Drive() {
	Console.WriteLine($"{Name} is driving.");
}
```

## Object Initializers
```csharp
var book = new Book { Title = "C# in Depth", Author = "Jon Skeet" };
```

## Encapsulation
Encapsulation means hiding internal details and exposing only what’s necessary. Use private fields and public properties.

## Real-World Example
```csharp
public class BankAccount
{
	public string Owner { get; set; }
	private decimal _balance;
	public decimal Balance => _balance;

	public BankAccount(string owner, decimal initial)
	{
		Owner = owner;
		_balance = initial;
	}

	public void Deposit(decimal amount)
	{
		if (amount > 0) _balance += amount;
	}
}
```

## Practice Exercise

**Task:** Implement a `Student` class with properties for `Name`, `Id`, and a method `PrintInfo()` that prints the student’s info.

**Bonus:** Add a private field for GPA and a public property to get/set it with validation (0.0–4.0).

---

## Interview Questions
- What is the difference between a class and an object?
- Why use properties instead of public fields?
- How does encapsulation improve code quality?

---

## Pro Tips
- Always use properties for public data.
- Use object initializers for clarity.
- Encapsulate fields to protect data integrity.
