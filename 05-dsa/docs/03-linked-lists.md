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

## Why Learn Them In C#

Linked lists are less common in everyday .NET application code than arrays, lists, dictionaries, and queues. They are still worth learning because they force you to understand references, mutation, ownership, and edge cases. Those skills transfer directly to object graphs, trees, EF navigation properties, and in-memory state machines.

## Reversal

Iterative reversal uses three references:

- `previous`
- `current`
- `next`

The key invariant is: everything before `current` has already been reversed and is reachable through `previous`.

Safe reversal order:

```text
next = current.Next
current.Next = previous
previous = current
current = next
```

Store `next` before changing `current.Next`; otherwise the rest of the list can become unreachable.

## Slow/Fast Pointers

Slow/fast pointers solve:

- Middle node
- Cycle detection
- Kth from end

For cycle detection, if a cycle exists, the fast pointer eventually catches the slow pointer.

## Merging Sorted Lists

Use a sentinel node to avoid special-casing the first appended node. Move the tail forward after each attachment. This preserves O(n + m) time and O(1) extra node allocation.

## Dummy/Sentinel Nodes

A dummy node is a temporary node before the real head. It simplifies algorithms that build or filter lists because every append has a previous node.

Use it for:

- Merging sorted lists
- Removing nodes by value
- Partitioning a list
- Building a result from existing nodes

The return value is usually `dummy.Next`.

## Edge Cases

- Empty list
- One-node list
- Two-node list
- Removing the head
- Removing the tail
- Cycle exists
- Lists have different lengths
- All values are equal

## Pitfalls

- Losing the rest of the list before storing `next`.
- Forgetting to move the tail pointer.
- Comparing node values when you need reference equality for cycle detection.
- Creating new nodes when the algorithm should relink existing nodes.
- Returning the dummy node instead of `dummy.Next`.

