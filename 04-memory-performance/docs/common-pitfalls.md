# Common Pitfalls

1. Treating every allocation as a bug.
2. Forcing garbage collection in normal application code.
3. Confusing string interning with general string reuse.
4. Boxing value types in loops without noticing.
5. Returning pooled buffers instead of the result they contained.
6. Using `Span<T>` after its source has gone out of scope.
7. Optimizing before measuring the real workload.
8. Prematurely replacing readable code with low-level tricks.
9. Assuming a pooled buffer is always cheaper than a fresh array.
10. Reading one benchmark result as if it were a final answer.

## How To Avoid Them

- keep allocation changes tied to a real workload
- prefer deterministic cleanup for resource ownership
- treat spans as temporary views, not owned storage
- use benchmarks to compare alternatives, not to justify a hunch
