namespace GenericsHomework;

public class Node<T>
{
    public T Value { get; }
    public Node<T>? Next { get; private set; }

    public Node(T value)
    {
        Value = value;
        Next = this;
    }

    public Node<T> Append(T value)
    {
        var newNode = new Node<T>(value);
        newNode.Next = this.Next;
        this.Next = newNode;
        return newNode;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? "null";
    }
}

