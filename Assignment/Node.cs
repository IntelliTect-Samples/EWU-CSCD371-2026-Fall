using System;
using System.Collections;
using System.Collections.Generic;

namespace Assignment;

public class Node<T> : IEnumerable<T>
{
    public T Data { get; set; }
    private Node<T> _next = null!;
    public Node<T> Next
    {
        get => _next;
        private set
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value), "Next cannot be null. Use Clear() or IterativeClear() to reset links explicitly.");
            _next = value;
        }
    }

    public Node(T data)
    {
        Data = data;
        Next = this;
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
    //Some node's next can access the node cleared is called on
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
    public IEnumerator<T> GetEnumerator()
    {
        yield return Data;

        Node<T> cur = Next;
        while (cur != this)
        {
            yield return cur.Data;
            cur = cur.Next;
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerable<T> ChildItems(int maximum)
    {
        if (maximum <= 1)
        {
            yield break;
        }

        int toReturn = maximum - 1;
        int returned = 0;
        var cur = Next;

        while (returned < toReturn && cur != this)
        {
            yield return cur.Data;
            returned++;
            cur = cur.Next;
        }
    }
}
