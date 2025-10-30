namespace GenericsHomework;

public class Node<TNodeValue>
{
    public TNodeValue Value { get; set; }
    public Node<TNodeValue> Next { get; private set; }

    public Node(TNodeValue value)
    {
        Value = value;
        Next = this;
    }

    public void Append(TNodeValue value)
    {
        if (Exists(value))
        {
            throw new ArgumentException("Value already exists, no duplicates allowed.", nameof(value));
        }
        Node<TNodeValue> newNode = new(value)
        {
            Next = Next
        };
        Next = newNode;
    }

    public bool Exists(TNodeValue value)
    {
        Node<TNodeValue> current = this;
        do
        {
            if (value is null && current.Value is null)
            {
                return true;
            } else if (current.Value is not null && current.Value.Equals(value))
            {
                return true;
            }
            current = current.Next;
        } while (current != this);
        return false;
    }

    public void Clear()
    {
        // this is sufficient for garbage collection to clean up the nodes because there are no external references to them
        // the nodes do not need to have their Next pointers set to null individually as long as there are no external references to them
        Next = this;
    }

    override public string ToString()
    {
        return Value?.ToString() ?? "null";
    }
}
