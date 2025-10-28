namespace GenericsHomework;

public class Node<T>
{
    public T Value { get; }
    public Node<T> Next { get; private set; }

    public Node(T value)
    {
        Value = value;
        Next = this;
    }

    public Node<T> Append(T value)
    {
        if (Exists(value))
        {
            throw new InvalidOperationException($"Value '{value}' already exists in the list.");
        }

        Node<T> newNode = new(value)
        {
            Next = this.Next
        };
        this.Next = newNode;
        return newNode;
    }

    public bool Exists(T value)
    {
        Node<T> current = this;
        do
        {
            if (Equals(current.Value, value))
            {
                return true;
            }
            current = current.Next;
        }
        while (current != this);

        return false;
    }

    public void Clear()
    {
        Node<T> current = this.Next;

        if (ReferenceEquals(current, this))
        {
            return;
        }

        while (!ReferenceEquals(current.Next, this))
        {
            Node<T> next = current.Next;
            current.Next = current; // Close the loop on the removed node
            current = next;
        }

        current.Next = current;
        this.Next = this;
    }


    public override string ToString()
    {
        return Value?.ToString() ?? "null";
    }
}

/*
 * Garbage Colection Notes:
 * As nodes are removed from the circular linked list using the Clear method, 
 * their Next references are set to point to themselves. 
 * This breaks the circular reference, 
 * allowing the garbage collector to reclaim the memory used by these nodes 
 * if there are no other references to them in the program.
 * 
 * Garbage Collection as I've understood it, as it pertains to this topic: 
 * If an outside source still holds a reference to a node that has been "removed"
 * it should be safer.
 * but a removed node "pointing to itself" does not count,
 * and therefore it could be collected.
 * 
 * See: https://essentialcsharp.com/runtime#garbage-collection
 */