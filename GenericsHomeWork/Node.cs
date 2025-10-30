using System.Collections;
using System.Collections.Generic;

namespace GenericsHomeWork;

public class NodeCollection<T> : ICollection<T>
{
    public T Value { get; set; }
    public NodeCollection<T> Next { get; private set; }

    public NodeCollection(T value)
    {
        Value = value;
        Next = this;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    public void Add(T item)
    {
        Append(item);
    }

    public bool Contains(T item)
    {
        return Exists(item);
    }

    public void Append(T value)
    {
        if (Exists(value))
        {
            throw new ArgumentException($"No duplicate values allowed in the list. The value '{value}' already exists.", nameof(value));
        }
        NodeCollection<T> newNode = new NodeCollection<T>(value);
        newNode.Next = this.Next;
        this.Next = newNode;
    }

    public bool Exists(T value)
    {
        NodeCollection<T> currentNode = this;
        do
        {
            if (EqualityComparer<T>.Default.Equals(currentNode.Value, value))
            {
                return true;
            }
            currentNode = currentNode.Next;
        } while (currentNode != this);
        return false;
    }

    // The Clear method breaks the circular loop of the removed nodes by setting the last node's Next
    // to itself and isolates the removed nodes for garbage collection

    public void Clear()
    {
        if (this.Next == this)
        {
            return;
        }
        this.Next = this;
    }

    public int Count
    {
        get
        {
            int count = 0;
            NodeCollection<T>? currNode = this.Next;
            while (currNode != null && currNode != this)
            {
                count++;
                currNode = currNode.Next;
            }
            return count;
        }
    }

    public bool IsReadOnly => false;

    public bool Remove(T value)
    {
        NodeCollection<T> prevValue = this;
        NodeCollection<T> currValue = this.Next;

        while (currValue != this)
        {
            if (EqualityComparer<T>.Default.Equals(currValue.Value, value))
            {
                prevValue.Next = currValue.Next;
                return true;
            }
            prevValue = currValue;
            currValue = currValue.Next;
        }
        return false;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), "arrayIndex cannot be negative.");
        } 

        if (Count > array.Length - arrayIndex)
        {
            throw new ArgumentException("Destination array is too small.");
        }

        NodeCollection<T>? currentValue = this.Next;
        while (currentValue != null && currentValue != this)
        {
            array[arrayIndex++] = currentValue.Value;
            currentValue = currentValue.Next;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        NodeCollection<T>? currentValue = this.Next;
        while (currentValue != null && currentValue != this)
        {
            yield return currentValue.Value;
            currentValue = currentValue.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
