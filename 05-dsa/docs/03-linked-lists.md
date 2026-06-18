# Linked Lists

A linked list stores values in nodes. Each node points to the next node. Unlike arrays, nodes do not need to live beside each other in memory.

## Singly Linked List

```text
head -> [1|next] -> [2|next] -> [3|null]
```

Common operations:

| Operation | Complexity |
| --- | --- |
| Read head | O(1) |
| Insert at head | O(1) |
| Search by value | O(n) |
| Read by index | O(n) |
| Insert after known node | O(1) |
| Delete after known node | O(1) |

Linked lists are useful for learning reference manipulation. In production C#, `List<T>`, `Queue<T>`, `LinkedList<T>`, and domain-specific structures are usually preferred unless linked-list behavior is specifically needed.

## Reversal

Iterative reversal uses three references:

- `previous`
- `current`
- `next`

The key invariant is: everything before `current` has already been reversed and is reachable through `previous`.

## Slow/Fast Pointers

Slow/fast pointers solve:

- Middle node
- Cycle detection
- Kth from end

For cycle detection, if a cycle exists, the fast pointer eventually catches the slow pointer.

## Merging Sorted Lists

Use a sentinel node to avoid special-casing the first appended node. Move the tail forward after each attachment. This preserves O(n + m) time and O(1) extra node allocation.

## Pitfalls

- Losing the rest of the list before storing `next`.
- Forgetting to move the tail pointer.
- Comparing node values when you need reference equality for cycle detection.
- Creating new nodes when the algorithm should relink existing nodes.

