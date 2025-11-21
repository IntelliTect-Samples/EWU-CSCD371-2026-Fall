using System.Collections;
using System.Collections.Generic;

namespace Assignment;

public class Node<T> : IEnumerable<T>
{
    public T Data { get; }
    public Node<T> Next { get; set; }

    public Node(T data)
    {
        Data = data;
        Next = this; // Circular reference by default
    }

    // Returns all items in the "circle" of items (Stopping after one full rotation)
    public IEnumerator<T> GetEnumerator()
    {
        yield return Data;
        var current = Next;
        while (current != null && current != this)
        {
            yield return current.Data;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // Returns remaining items with max count
    public IEnumerable<T> ChildItems(int maximum)
    {
        var current = Next;
        int count = 0;
        
        // FIX: Added current != this check to prevent an infinite loop in a circular list (Task 7)
        while (current != null && current != this && count < maximum)
        {
            yield return current.Data;
            current = current.Next;
            count++;
        }
    }
}