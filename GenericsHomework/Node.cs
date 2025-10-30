using System;

namespace GenericsHomework;

public class Node<T>
{
    public T Value { get; set; }
    public Node<T> Next { get; private set; }

    public Node(T value)
    {
        Value = value;
        Next = this;
    }
    public override string ToString()
    {
        return Value?.ToString() ?? "null";
    }
    public bool Exists(T value)
    {
        Node<T> cur = this;
        do
        {
            if(Equals(value, cur.Value)) return true;
            cur = cur.Next;
        }
        while (cur != this);
        return false;
    }
    /// <summary>
    /// Inserts a new node immediately after this node in the list.
    /// </summary>
    /// <remarks>
    /// This method does <b>not</b> append to the end of the list.
    /// Instead, it places the new node directly in front of this node’s current successor.
    /// </remarks>
    public void Append(T value)
    {
        if(Exists(value)) throw new ArgumentException("Given value is already included in this list!");

        Node<T> node = new Node<T>(value);
        node.Next = Next;
        this.Next = node;
    }
    public void Clear()
    {
        //It is sufficent to set Next to 'this'
        //Because GC is a tree structure, anything not connected to the 'root node' is eligible for cleanup.
        //Therefore the rest of the list, now disconnected from the 'root node', will be cleaned up at some point in time.
        Next = this;
    }
}
