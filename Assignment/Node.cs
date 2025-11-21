using System;
using System.Collections.Generic;
using System.Collections;

namespace Assignment;

public class Node<T> : IEnumerable<T>
{
    private Node<T> _next;
    public T Value { get; }

    public Node<T> Next
    {
        get => _next;
        private set => _next = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Node(T value)
    {
        Value = value;
        _next = this;
    }

    public override string ToString() => Value?.ToString() ?? string.Empty;

    public Node<T> Append(T value)
    {
        if (Exists(value))
            throw new ArgumentException("No duplicate values.", nameof(value));

        var newNode = new Node<T>(value);
        newNode.Next = this.Next;
        this.Next = newNode;

        return newNode;
    }

    public void Clear()
    {
        // C# garbage collector handles cyclic memory deallocation.
        // There is no way to access the rest of the list, so they 
        // will be deallocated without memory leaks.
        this.Next = this;
    }

    public bool Exists(T value)
    {
        var comp = EqualityComparer<T>.Default;
        var head = this;
        var cur = head;

        do
        {
            if (comp.Equals(cur.Value, value))
            {
                return true;
            }

            cur = cur.Next;
        } while (!ReferenceEquals(cur, head));

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        Node<T>? curr = this;

        do
        {
            yield return curr!.Value;
            curr = curr.Next;
        } while (curr != this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<T> ChildItems(int maximum)
    {
        if (maximum <=0)
            yield break;

        Node<T>? curr = this.Next;
        int count = 0;

        while (curr != this & count < maximum)
        {
            yield return curr!.Value;
            curr = curr.Next;
            count++;
        }
    }
}