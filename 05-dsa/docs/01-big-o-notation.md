# Big-O Notation

Big-O describes how resource usage grows as input size grows. It is a language for comparing algorithms when exact timings are too dependent on hardware, runtime, input distribution, and constant factors.

## Common Growth Classes

| Complexity | Name         | Example                                |
| ---------- | ------------ | -------------------------------------- |
| O(1)       | Constant     | Read `array[0]`                        |
| O(log n)   | Logarithmic  | Binary search                          |
| O(n)       | Linear       | Scan an array                          |
| O(n log n) | Linearithmic | Merge sort, typical comparison sorting |
| O(n^2)     | Quadratic    | Compare every pair                     |
| O(2^n)     | Exponential  | Enumerate subsets                      |
| O(n!)      | Factorial    | Enumerate permutations                 |

## What To Count

Usually count the operation that grows with input size:

- Loop iterations
- Comparisons
- Hash lookups
- Recursive calls
- Allocated auxiliary storage

## Time vs Space

Time complexity measures work. Space complexity measures additional memory beyond the input.

Examples:

- Reversing an array in place: O(n) time, O(1) extra space
- Building a frequency dictionary: O(n) time, O(k) extra space where `k` is the number of distinct values
- Merge sort: O(n log n) time, O(n) extra space

## Amortized Cost

Some operations are occasionally expensive but cheap on average over many calls. `List<T>.Add` is amortized O(1): most appends write directly, but sometimes the internal array resizes and copies existing elements.

## Practical Guidance

- Drop constants when comparing growth: O(2n) becomes O(n).
- Drop lower-order terms: O(n^2 + n) becomes O(n^2).
- Keep variable names meaningful: scanning `users` and `roles` is O(u + r), not always O(n).
- Worst-case, average-case, and best-case can differ.
- Big-O does not replace benchmarking for hot production paths.

## How To Analyze A Method

Use this order:

1. Identify the input size or sizes.
2. Find loops and recursive calls that grow with input.
3. Count expensive operations inside those loops.
4. Include helper calls if they are not O(1).
5. Keep separate variables separate.
6. Remove constants and lower-order terms.
7. Analyze extra memory separately from input memory.

Example:

```csharp
foreach (var user in users)
{
    foreach (var role in roles)
    {
        // compare user to role
    }
}
```

This is O(u * r), not automatically O(n^2). It becomes O(n^2) only when both inputs grow together and have roughly the same size.

## Common .NET Cost Traps

- `List<T>.Contains` is O(n); `HashSet<T>.Contains` is average O(1).
- `Enumerable.Count()` can enumerate the whole sequence when the source is not a collection.
- `OrderBy` is O(n log n) and allocates sorting buffers when materialized.
- Repeated `string += value` in a loop can become O(n^2).
- `Dictionary<TKey, TValue>` lookup is average O(1), not guaranteed O(1).
- Copying with `ToList()` or `ToArray()` is O(n) time and O(n) space.

## Complexity Review Questions

Before accepting your own answer, ask:

- What happens when input doubles?
- Did I count the cost of sorting?
- Did I count allocated helper collections?
- Is this worst case, average case, or amortized?
- Are there two independent input sizes?
- Did a LINQ call hide a loop?

## Backend Example

Imagine an API endpoint receives 10,000 product IDs and must remove duplicates.

Brute force with a list:

```text
for each id:
    if result does not contain id:
        add id
```

`result.Contains(id)` is O(n), so the whole process can become O(n^2).

Using `HashSet<int>`:

```text
for each id:
    if seen.Add(id):
        add id to result
```

The average time becomes O(n), with O(n) extra space.
