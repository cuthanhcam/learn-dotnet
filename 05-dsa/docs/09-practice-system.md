---
title: "DSA Practice System"
description: "A repeatable problem-solving, review, spaced-repetition, and mistake-tracking workflow."
slug: dsa-practice-system
phase: 5
order: 9
difficulty: reference
article-type: reference
estimated-reading-minutes: 20
topics: [dsa, practice, problem-solving]
prerequisites: [dsa-recursion-backtracking]
status: maintained
last-reviewed: 2026-08-15
---

# DSA Practice System

DSA improves through repetition with feedback. Reading explanations helps, but fluency comes from solving, testing, forgetting a little, and solving again.

## The Practice Contract

For every problem, produce four things:

1. A working C# solution.
2. A short explanation of the chosen data structure.
3. Time and space complexity.
4. Tests for normal and edge cases.

If one of those is missing, the problem is not finished yet.

## Daily Session

Use a 45- to 75-minute session:

| Time | Activity |
| ---- | -------- |
| 5 min | Restate the problem and list constraints |
| 10 min | Write brute-force idea and edge cases |
| 20-35 min | Implement the best solution you can defend |
| 10 min | Add tests and fix mistakes |
| 5 min | Write complexity and one lesson learned |

When stuck, do not stare at the screen forever. Write the operation you need most: lookup, ordering, nearest item, previous item, all combinations, shortest path, or range sum. That often points to the structure.

## Review Cadence

| When | What to review |
| ---- | -------------- |
| Same day | Fix syntax and test failures |
| Next day | Re-implement without looking |
| 3 days later | Explain the invariant from memory |
| 1 week later | Solve a variant |
| 1 month later | Mix with other topics |

Spaced repetition matters because algorithm patterns are easy to recognize immediately after reading and much harder to recall later.

## Difficulty Ladder

1. **Trace**: read a solution and trace it by hand.
2. **Complete**: fill missing lines in a familiar template.
3. **Rebuild**: implement from memory.
4. **Adapt**: solve a variant with one changed constraint.
5. **Compare**: explain two approaches and their trade-offs.
6. **Design**: use the pattern inside a small backend-like feature.

## Mistake Log

Keep a tiny mistake log. Each entry should be specific:

```text
Problem:
Bug:
Why it happened:
How to catch it next time:
Pattern:
```

Good examples:

- "Moved `left` only once when the sliding window needed a `while` loop."
- "Marked graph node visited after dequeue, causing duplicates in the queue."
- "Returned exact binary-search match but the problem needed lower bound."

## Test Checklist

Use this checklist before calling a solution complete:

- Empty input
- Single item
- Two items
- Duplicate values
- Already sorted input
- Reverse sorted input
- Negative numbers when numeric input allows them
- Missing target
- Target at first or last position
- Very large input shape, even if the test uses smaller data

Not every problem needs every case, but every problem deserves an intentional choice.

## Explanation Checklist

A strong explanation includes:

- The main data structure.
- The invariant that stays true.
- Why the algorithm terminates.
- Why the result is correct.
- Time complexity.
- Space complexity.
- Any mutation of input.

Example:

```text
I use a HashSet<int> to remember values already seen.
Before checking value x, the set contains exactly the values from earlier indexes.
If target - x is present, those two indexes form a valid pair.
The loop terminates after scanning the array once.
Time is O(n); extra space is O(n).
```

## Anti-Rushing Rules

- Do not jump to the optimized solution before writing the brute-force idea.
- Do not submit code without edge-case tests.
- Do not say "O(1)" for dictionary operations without remembering it is average case.
- Do not use LINQ to hide the algorithm while practicing fundamentals.
- Do not memorize problem names; memorize operations and invariants.

## Suggested Weekly Mix

After finishing the topic docs, keep a weekly mix:

| Topic type | Count |
| ---------- | ----- |
| Arrays/strings | 2 |
| Hash tables | 2 |
| Stack/queue | 1 |
| Trees/graphs | 2 |
| Sorting/searching | 1 |
| Recursion/backtracking | 1 |
| Review old mistakes | 1 session |

The mix keeps older topics warm while newer topics grow.
