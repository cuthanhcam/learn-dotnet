---
title: "Advanced Tree Indexes: AVL, Heaps, Tries, Fenwick, Segment, and B-Trees"
description: "A comparison of balanced search trees, priority heaps, prefix tries, range-query trees, and storage-oriented B-tree families."
slug: dsa-advanced-tree-indexes
phase: 5
order: 11
topics: [dsa, avl, heap, trie, fenwick-tree, segment-tree, b-tree]
article-type: deep-dive
estimated-reading-minutes: 22
prerequisites: [dsa-trees-graphs, dsa-sorting-searching, dsa-big-o]
difficulty: advanced
status: maintained
last-reviewed: 2026-08-15
---

# Advanced Tree Indexes

Different trees optimize different questions. “Use a tree” is not a complete design decision: ordering, update frequency, query type, key shape, memory layout, and storage medium determine the appropriate structure.

## Learning Objectives

After this article, you should be able to:

- explain why an unbalanced BST can degrade to linear height;
- compare AVL and red-black balancing priorities;
- implement heap sift-up and sift-down invariants;
- distinguish a trie prefix from a complete stored key;
- derive Fenwick tree navigation from the least-significant set bit;
- choose between Fenwick and segment trees for range problems;
- explain why database indexes favor B/B+ trees rather than pointer-heavy binary trees.

## Decision Table

| Requirement | Candidate | Typical operation cost | Main trade-off |
|---|---|---|---|
| Ordered set/map | Balanced BST | `O(log n)` search/update | Pointer and balancing overhead |
| Repeated min/max | Binary heap | `O(1)` peek, `O(log n)` update | No efficient arbitrary search |
| Prefix lookup | Trie | `O(k)` for key length `k` | High node/edge memory use |
| Prefix sums + point update | Fenwick tree | `O(log n)` | Less flexible aggregation |
| General range aggregate/update | Segment tree | `O(log n)` | Larger implementation/storage cost |
| Storage-page ordered index | B/B+ tree | Logarithmic in high branching factor | Complex split/merge logic |

## AVL Trees

An AVL node stores height or balance factor. For every node:

```text
balance = height(left) - height(right)
balance ∈ {-1, 0, 1}
```

Insertion or deletion updates heights while returning toward the root. A balance of `+2` or `-2` requires rotation:

- left-left: single right rotation;
- right-right: single left rotation;
- left-right: left rotation on the child, then right rotation;
- right-left: right rotation on the child, then left rotation.

A rotation must preserve BST ordering while changing local height. AVL trees enforce strict balance and therefore give excellent lookup height, at the cost of more update bookkeeping.

Red-black trees use coloring constraints to guarantee height at most proportional to `log n` with a looser balance. Many general ordered maps prefer this update/search compromise. In application code, use `SortedSet<T>` or `SortedDictionary<TKey,TValue>` unless implementing the tree is the learning or product requirement.

## Binary Heaps and `PriorityQueue`

A binary heap is a complete tree stored compactly in an array. For zero-based index `i`:

```text
parent = (i - 1) / 2
left   = 2i + 1
right  = 2i + 2
```

In a min-heap every parent priority is no greater than its children. Insert at the end and sift upward. Remove the root, move the last element to index zero, and sift downward. Building a heap bottom-up is `O(n)`, not `O(n log n)`.

.NET provides `PriorityQueue<TElement,TPriority>`. Equal priorities do not promise stable FIFO ordering; include a sequence value in the priority when stability is required.

The repository also implements `BinaryMinHeap<T>` to expose mechanics hidden by
`PriorityQueue`. Its collection constructor uses bottom-up heapify: leaves already satisfy the
invariant, so only internal nodes sift downward. Repeated removal is a useful executable check of
the invariant because it must produce nondecreasing output, including duplicate values.

## Prefix Tries

A trie stores a path per key prefix. Each node needs an explicit terminal marker because `car` and `cart` can both be valid keys. Search, insertion, and deletion are `O(k)` in key length, independent of the number of stored keys, assuming child lookup is effectively constant.

The included `PrefixTrie` uses a `SortedDictionary<char,Node>` so prefix results are deterministic and lexical. That choice makes each edge lookup logarithmic in a node's branching factor. A `Dictionary` is usually faster for lookup; a fixed array can be fastest for a small known alphabet but wastes space.

Deletion unmarks the terminal node and prunes an edge only when the child is neither terminal nor parent to another key. This invariant preserves longer and sibling words.

Unicode note: .NET `char` represents a UTF-16 code unit, not necessarily one Unicode scalar or user-perceived character. Production text indexes must define normalization, casing, culture, and token boundaries explicitly.

## Fenwick Trees

A Fenwick tree stores partial sums in a one-based array. The least-significant set bit, `i & -i`, describes the range length represented at index `i`.

- Prefix query moves toward zero: `i -= i & -i`.
- Point update moves to responsible ancestors: `i += i & -i`.
- Range sum `[left, right]` is `prefix(right) - prefix(left - 1)`.

Both query and update are `O(log n)` with `O(n)` storage. The implementation retains original point values so assignment can be converted into an additive delta. `checked` arithmetic makes overflow part of the visible contract.

Fenwick trees work best when the aggregate has an inverse suitable for subtracting prefixes. Segment trees are more flexible for minimum/maximum, custom associative operations, and lazy range updates.

## Segment Trees

A segment tree recursively partitions an interval and stores an aggregate for each segment. A query visits only segments that exactly cover the requested range. Point updates change one leaf and recompute ancestors.

Typical costs are `O(n)` build, `O(log n)` point update, and `O(log n)` range query. A recursive
implementation often reserves around `4n` slots for convenience. The included iterative
`SegmentTree` instead rounds leaf capacity to a power of two and uses a flat array twice that
capacity. Lazy propagation defers range updates by recording pending work at internal nodes, but
substantially increases correctness complexity.

The public API uses half-open ranges `[startInclusive, endExclusive)`, matching .NET slicing and
allowing an empty range when both boundaries are equal. During an iterative query, an odd left
boundary contributes its current node before moving right, while an odd exclusive-right boundary
moves left before contributing. Both boundaries then move to their parents.

Before implementation, define:

- whether ranges are closed, open, or half-open;
- the associative combine operation;
- its identity element;
- whether updates are point or range based;
- overflow and empty-range behavior.

## B-Trees and B+ Trees

Binary trees optimize comparisons but have poor branching for block storage. B-trees store many keys and children per node so one disk/page read eliminates a large search range. Splits and merges preserve occupancy rules.

B+ trees keep records or record pointers in leaves, while internal nodes guide navigation. Linked leaves support efficient range scans. Database index behavior also depends on page size, fill factor, clustering, concurrency control, and storage engine implementation; an in-memory sample cannot model all of those concerns honestly.

## Implementation Map

- `TreesGraphs/BinarySearchTree.cs`: unbalanced ordered-set mechanics.
- `TreesGraphs/BinaryMinHeap.cs`: bottom-up heapify, insertion, peek, and removal.
- `TreesGraphs/PrefixTrie.cs`: prefix insertion, lookup, enumeration, and safe pruning.
- `TreesGraphs/FenwickTree.cs`: point assignment and prefix/range sums.
- `TreesGraphs/SegmentTree.cs`: iterative half-open range sums and point assignment.
- `BinaryMinHeapTests.cs`: duplicates, custom comparison, empty-state, and ordering tests.
- `SegmentTreeTests.cs`: empty ranges, full/partial queries, updates, and invalid boundaries.
- `AdvancedTreeIndexTests.cs`: trie and Fenwick semantics and boundaries.

## Common Pitfalls

- Calling a plain BST operation `O(log n)` without a balance guarantee.
- Forgetting to update height before testing an AVL balance factor.
- Breaking BST order during a rotation.
- Treating a trie prefix node as a complete key without a terminal marker.
- Mixing zero-based public indexes with one-based Fenwick internals.
- Using a Fenwick tree for an aggregate that cannot be composed from prefixes.
- Assuming `PriorityQueue` is stable for equal priorities.
- Comparing B-tree algorithms without considering page I/O and branching factor.

## Exercises

1. Implement an AVL tree and assert its balance and ordering invariants after every permutation of a small input.
2. Extend `BinaryMinHeap<T>` with replace-min and a stable priority wrapper using an insertion sequence.
3. Extend `PrefixTrie` with prefix deletion and define its return contract.
4. Implement a generic segment tree over an associative combine function and identity.
5. Add a Fenwick `Add(index, delta)` operation and property-based comparisons against a plain array.

## Review Questions

1. Why is bottom-up heap construction linear?
2. Which trie deletion condition makes pruning safe?
3. What does `i & -i` represent in a Fenwick tree?
4. When is a segment tree worth its additional complexity?
5. Why do database indexes use a high branching factor?

## Navigation

[← .NET DSA reference](10-dotnet-dsa-reference.md) · [Graph algorithms →](12-advanced-graph-algorithms.md)

## References

- [.NET `PriorityQueue<TElement,TPriority>` API](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.priorityqueue-2)
- [.NET collection selection guidance](https://learn.microsoft.com/en-us/dotnet/standard/collections/selecting-a-collection-class)
