namespace GenericsHomework_
{
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
                if(cur.Value.Equals(value))
                    return true;
                cur = cur.Next;
            }
            while (cur != this);
            return false;
        }
        public void Append(T value)
        {
            if(Exists(value)) throw new ArgumentException("Given value is already included in this list!");

            Node<T> node = new Node<T>(value);
            node.Next = Next;
            this.Next = node;
        }
    }
}
