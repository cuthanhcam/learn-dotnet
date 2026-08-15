---
title: "Object Contracts, Equality, Records, and Immutability"
description: "Reference identity, value equality, Equals, GetHashCode, ToString, records, and immutable type design."
phase: 2
order: 8
topics: [csharp, equality, records, immutability]
---

# Object Contracts, Equality, Records, and Immutability

Every C# type ultimately inherits the public contract of `System.Object`. Understanding that contract is essential when values are compared, logged, placed in hash-based collections, or passed across an API boundary.

## Identity and value equality

Reference identity asks whether two variables refer to the same object. Value equality asks whether two objects represent the same logical value. Neither interpretation is universally correct: an `Employee` entity may be identified by an ID even when other fields change, while two independently constructed `Money(10, "USD")` values should usually compare equal.

```csharp
var first = new Customer("C-100", "Ada");
var alias = first;
var second = new Customer("C-100", "Ada");

ReferenceEquals(first, alias);  // true
ReferenceEquals(first, second); // false
```

Classes use reference equality by default unless they override equality. Structs have value-oriented default equality, although a deliberate implementation may be clearer and faster for frequently compared values. Records synthesize value equality from their declared components.

## The `Equals` contract

A correct equality relation is:

- reflexive: `x.Equals(x)` is true;
- symmetric: `x.Equals(y)` and `y.Equals(x)` agree;
- transitive: if `x == y` and `y == z`, then `x == z`;
- consistent while the compared state is unchanged;
- false when compared with `null`.

For a value object, implement `IEquatable<T>` to provide strongly typed equality and override `object.Equals` consistently:

```csharp
public sealed class Money : IEquatable<Money>
{
    public Money(decimal amount, string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public decimal Amount { get; }
    public string Currency { get; }

    public bool Equals(Money? other) =>
        other is not null &&
        Amount == other.Amount &&
        StringComparer.Ordinal.Equals(Currency, other.Currency);

    public override bool Equals(object? obj) => obj is Money other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(
        Amount,
        StringComparer.Ordinal.GetHashCode(Currency));
}
```

## `GetHashCode` is part of equality

Equal values **must** produce the same hash code during a process. Unequal values may collide. Hash codes are used to choose buckets in `Dictionary<TKey, TValue>` and `HashSet<T>`; they are not stable IDs, checksums, or persisted values.

Never mutate fields that participate in equality while an object is being used as a hash key. The collection may search a different bucket and appear to “lose” the object.

## `ToString` is diagnostic text

Override `ToString` when a concise representation helps debugging and logs. Do not expose secrets, tokens, passwords, or personal data. Do not make parsers depend on diagnostic text unless the type explicitly defines a stable serialization format.

```csharp
public override string ToString() => $"{Amount} {Currency}";
```

## Records versus classes

Use a record when the primary meaning of the type is its data and value equality is desirable. Use a class when identity and an evolving lifecycle dominate. This is a design decision, not a rule that DTOs must always be records.

```csharp
public sealed record Address(string Street, string City, string CountryCode);

var original = new Address("1 Main St", "London", "GB");
var moved = original with { Street = "2 Main St" };
```

Record properties can still reference mutable objects. A record containing a `List<T>` is not deeply immutable merely because its top-level properties are `init`-only.

## Designing immutable types

An immutable type does not expose operations that change its observable state after construction. Practical techniques include:

- validate invariants in the constructor or factory;
- expose get-only or `init`-only properties;
- avoid returning mutable internal collections;
- copy mutable input when ownership is not transferred;
- return a new value from state-changing operations.

Immutability simplifies reasoning and makes sharing safer, but copying large object graphs can be costly. Choose it where stable values and predictable behavior matter.

## Review questions

1. Why must equal objects have equal hash codes?
2. Why is a mutable dictionary key dangerous?
3. When is identity equality more appropriate than structural equality?
4. Does `init` make every referenced object immutable?
5. What information should never appear in `ToString`?

## Practice

Implement an immutable `EmailAddress` value object that normalizes its domain, rejects invalid input, provides value equality, and produces safe diagnostic text. Add tests for reflexivity, symmetry, equivalent casing rules, invalid input, and hash-set behavior.

## Navigation

[← OOP patterns](07-oop-patterns.md) · [SOLID principles →](09-solid-principles.md)
