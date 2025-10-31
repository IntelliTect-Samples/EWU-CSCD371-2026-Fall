namespace GenericsHomework;

public class Node<T>
{
    public T Data { get; set; }
    private Node<T> _next;
    public Node<T> Next
    {
        get => _next;
        private set => _next = value ?? this;
    }

    public Node(T data)
    {
        Data = data;
        _next = this;
    }

    public void Append(T value)
    {
        if (this.Exists(value))
        {
            throw new InvalidOperationException("Cannot Have Duplicates Appended");
        }
        Node<T> nextNode = new(value)
        {
            Next = this.Next
        };
        Next = nextNode;
    }
    //This Clear() is not as good as itterative clear because the garbage collector wont collect
    //any of the nodes that point to the node clear is called on. It is also bad practice because
    //Some node's next can access the node cleared is called on.
    public void Clear()
    {
        Next = this;
    }
    public void IterativeClear()
    {
        Node<T> cur = this.Next;
        while (cur != this)
        {
            Node<T> next = cur.Next;
            cur.Next = cur;
            cur = next;
        }
        this.Next = this;
    }

    public bool Exists(T value)
    {
        Node<T> cur = this;
        do
        {
            if (Equals(cur.Data, value))
            {
                return true;
            }
            cur = cur.Next;
        } while (cur.Next != this);
        return false;
    }
    public override string ToString()
    {
        if (Data is null)
        {
            return "null";
        }
        else
        {
            return Data.ToString() ?? "null";
        }
    }
}
