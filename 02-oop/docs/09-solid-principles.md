---
title: "SOLID Principles"
description: "A practical introduction to five object-design heuristics, their trade-offs, and common misuse."
slug: solid-principles-practical-guide
phase: 2
order: 9
difficulty: intermediate
article-type: deep-dive
estimated-reading-minutes: 24
topics: [oop, solid, design]
prerequisites: [oop-composition-and-patterns, csharp-object-contracts-records-immutability]
status: maintained
last-reviewed: 2026-08-15
---

# SOLID Principles

SOLID is a set of design heuristics for controlling reasons to change and dependency direction. It is not a scoring system and does not require an interface for every class. Apply a principle when it makes a concrete change safer or a contract clearer.

## S — Single Responsibility Principle

A module should have one cohesive reason to change. “Do only one line of work” is too literal; a cohesive class may contain several methods.

If an invoice service calculates totals, formats PDFs, sends email, and writes database rows, changes from accounting, design, operations, and persistence all collide. Separate policies at boundaries that change independently.

## O — Open/Closed Principle

Software should be open to extension without requiring risky edits to stable behavior. Polymorphism, composition, delegates, and data-driven rules can all support extension.

Do not predict every possible extension. Introduce an abstraction after identifying a genuine variation point; speculative abstractions create more code without proven flexibility.

```csharp
public interface IDiscountPolicy
{
    decimal CalculateDiscount(Order order);
}
```

New policies can implement the contract without adding another branch to a central switch. The trade-off is indirection and more types.

## L — Liskov Substitution Principle

Code accepting a base type should continue to behave correctly when given any valid subtype. A subtype must preserve the base contract: it should not strengthen preconditions, weaken postconditions, or invalidate expected invariants.

The classic warning sign is inheritance chosen only for code reuse. If a subtype must throw `NotSupportedException` for a normal base operation, the hierarchy may be expressing the wrong relationship. Composition or smaller capability interfaces often model the domain better.

## I — Interface Segregation Principle

Clients should depend only on capabilities they use. A large interface forces implementations and consumers to know about unrelated operations.

Prefer cohesive capability contracts such as `IReadableStore<T>` and `IWritableStore<T>` when consumers genuinely need different subsets. Avoid fragmenting every method into its own interface; that merely moves complexity into dependency wiring.

## D — Dependency Inversion Principle

High-level policy should not depend directly on low-level details; both should depend on stable abstractions owned near the policy boundary.

Dependency injection is a mechanism that can support this principle, but constructor injection alone does not guarantee good design. The abstraction must represent what the consumer needs, not mirror every method of an infrastructure library.

```csharp
public sealed class CheckoutService
{
    private readonly IPaymentGateway _payments;

    public CheckoutService(IPaymentGateway payments)
    {
        _payments = payments ?? throw new ArgumentNullException(nameof(payments));
    }

    public Task<PaymentResult> CheckoutAsync(Order order, CancellationToken token) =>
        _payments.ChargeAsync(order.Total, token);
}
```

## Applying SOLID responsibly

Before adding an abstraction, ask:

1. Which independent change is this design isolating?
2. Which consumer owns the required contract?
3. Is composition clearer than inheritance?
4. Does the extra indirection improve testing or replacement in a meaningful way?
5. Can the design remain simpler until a second implementation or variation appears?

Warning signs include interfaces with a single implementation and no boundary value, inheritance trees built for reuse, service classes with unrelated collaborators, and mock-heavy tests that duplicate implementation details.

## Review exercise

Take an order-processing class that validates an order, calculates discounts, writes a row, and sends an email. Identify the independent policies and side effects. Refactor only the boundaries that need to vary, then explain which SOLID principle each change supports and what complexity it adds.

## Navigation

[← Object contracts and records](08-object-contracts-records.md) · [Common pitfalls →](common-pitfalls.md)
