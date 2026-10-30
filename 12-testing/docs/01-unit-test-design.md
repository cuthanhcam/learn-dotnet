---
title: "Unit Tests as Observable Contracts"
description: "Design focused xUnit facts and theories that expose boundary mistakes without coupling tests to private implementation details."
slug: testing-unit-contracts
phase: 12
order: 1
difficulty: intermediate
article-type: tutorial
estimated-reading-minutes: 15
topics: [testing, xunit, boundaries, assertions]
prerequisites: [testing-learning-roadmap]
status: maintained
last-reviewed: 2026-10-25
---

# Unit Tests as Observable Contracts

## Requirement before implementation

An unreturned library loan needs a reminder at or after its deadline. A returned loan never needs one.
An absent loan is a lookup outcome, not an invalid identifier. Empty identifiers are invalid input.
These requirements determine the tests; the current `if` statement does not define the requirement.

The [policy](../src/Learning.Testing.Examples/Loans/ReminderPolicy.cs) accepts the loan and decision
instant explicitly. There is no clock lookup, database, HTTP pipeline, or mutable global state in it.
Testing this function directly provides fast evidence about the rule, not about framework wiring.

## Arrange, act, assert

Open [ReminderPolicyTests](../tests/Learning.Testing.UnitTests/ReminderPolicyTests.cs).
Each case arranges a valid loan, acts by evaluating the policy, and asserts the externally observable
answer. The name `IsDue_AtDeadlineBoundary_UsesInclusiveInstantComparison` states the trigger and result.

| Decision instant | Unreturned | Returned |
|---|---|---|
| One tick before deadline | No reminder | No reminder |
| Exactly deadline | Reminder | No reminder |
| One tick after deadline | Reminder | No reminder |

The first theory covers all three adjacent instants, so changing `>=` to `>` breaks a meaningful
business-boundary case. A second theory covers returned loans. An offset-equivalence fact proves that
07:00 at UTC+07:00 and 00:00 UTC represent the same instant. Comparing `DateTimeOffset.DateTime`
instead would lose that contract.

Use a `[Fact]` for a specific scenario and a `[Theory]` for one rule with several input/output pairs.
Avoid tables that mix unrelated rules merely to reduce method count. Multiple assertions are useful
when they describe one coherent outcome; “one assertion per test” is not a requirement.

## Assert the right evidence

For invalid identifiers, assert the exception type and `ParamName`; exact exception prose is not the
public behavior this lesson promises. For reminder creation, assert the payload and outcome. Assert
the absence of sender calls when “must not send” is the requirement. Do not assert private helper names,
class layout, or incidental method ordering that could change in an equivalent refactor.

No test computes the expected answer by calling the same production helper a second time. That would
repeat a bug rather than provide an independent specification. Random GUIDs here provide unrelated
nonempty identity values; no assertion depends on their byte values or random ordering.

## Run a focused test

From the repository root:

```powershell
dotnet test 12-testing/tests/Learning.Testing.UnitTests --configuration Release --filter "FullyQualifiedName~ReminderPolicyTests"
```

Read the failing test name, expected/actual values, and source line. A failing assertion is useful
information. Distinguish it from a build failure, missing runtime, or unavailable package source.
Run the phase suite after a focused fix so related contracts are checked too.

## Exercises

Work in your own branch. The tracked source and tests are reference solutions.

1. Change `>=` to `>` locally. Run the policy tests and identify the exact-boundary failure. Restore
   the correct comparison before committing. Acceptance: explain why a test only one day overdue
   would miss the defect.
2. Add a deadline example with a negative UTC offset. Acceptance: compare the same instant and one
   tick before it, without depending on the machine's local time zone.
3. Propose a grace-period requirement before implementing it. Acceptance: state whether equality at
   the end of the grace interval is included and add tests for each adjacent instant. Explain how
   overflow near `DateTimeOffset.MaxValue` should be handled rather than guessing.

## Limits and references

These tests do not prove a query, HTTP route, or external sender works. Keep those integration
contracts in their own suites; duplicating every unit case through HTTP increases cost without
necessarily adding independent evidence.

- [Microsoft: unit testing best practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [xUnit v2 getting started](https://xunit.net/docs/getting-started/v2/getting-started)
- [DateTimeOffset comparison](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset.compare)

## Navigation

- Previous: [Roadmap](00-roadmap.md)
- Next: [Time, doubles, and asynchronous behavior](02-time-doubles-async.md)
