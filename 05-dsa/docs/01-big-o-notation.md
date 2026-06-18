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
