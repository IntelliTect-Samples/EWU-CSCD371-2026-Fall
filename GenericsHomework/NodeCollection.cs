using System.Collections;

namespace GenericsHomework;

/// <summary>
/// A circular linked list implementation that maintains a homogeneous collection of values.
/// Each node points to the next node, with the last node pointing back to the first.
/// </summary>
/// <typeparam name="T">The type of values stored in the collection.</typeparam>
public class NodeCollection<T> : System.Collections.Generic.ICollection<T>, IEnumerable<T>, IEnumerable
{
    /// <summary>
    /// Gets or sets the value stored in this node.
    /// </summary>
    public T Value { get; set; }

    /// <summary>
    /// Gets the next node in the circular list.
    /// The setter is private to maintain circular structure integrity.
    /// </summary>
    public NodeCollection<T> Next { get; private set; }

    /// <summary>
    /// Gets the number of elements in the circular list.
    /// </summary>
    public int Count
    {
        get
        {
            int count = 1; // Start with 1 for the current node
            NodeCollection<T> current = this.Next;
            // Traverse the circular list until we loop back to the starting node
            while (current != this)
            {
                count++;
                current = current.Next;
            }
            return count;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the collection is read-only.
    /// Always returns false as this collection is mutable.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Initializes a new node with the specified value.
    /// The node initially points to itself, forming a circular structure.
    /// </summary>
    /// <param name="value">The value to store in this node.</param>
    public NodeCollection(T value)
    {
        Value = value;
        Next = this;
    }

    /// <summary>
    /// Returns the string representation of the value stored in this node.
    /// </summary>
    /// <returns>The string representation of the value, or an empty string if the value is null.</returns>
    public override string ToString()
    {
        return Value?.ToString() ?? String.Empty;
    }

    /// <summary>
    /// Appends a new node with the specified value to the circular list.
    /// </summary>
    /// <param name="value">The value to append.</param>
    /// <exception cref="InvalidOperationException">Thrown when the value already exists in the list.</exception>
    public void Append(T value)
    {
        // Check for duplicates before appending
        if (Exists(value))
        {
            throw new InvalidOperationException($"Value '{value}' already exists in the list.");
        }

        NodeCollection<T> newNode = new NodeCollection<T>(value);

        // Find the last node in the circular list (the one that points back to this)
        NodeCollection<T> current = this;
        while (current.Next != this)
        {
            current = current.Next;
        }

        // Insert new node between last node and this (first) node
        current.Next = newNode;
        newNode.Next = this;
    }

    /// <summary>
    /// Adds an item to the collection. This is an alias for Append.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <exception cref="ArgumentException">Thrown when the item already exists in the list.</exception>
    public void Add(T item)
    {
        Append(item);
    }

    /// <summary>
    /// Checks if a value exists anywhere in the circular list.
    /// </summary>
    /// <param name="value">The value to search for.</param>
    /// <returns>True if the value exists; otherwise, false.</returns>
    public bool Exists(T value)
    {
        NodeCollection<T> current = this;

        do
        {
            if (EqualityComparer<T>.Default.Equals(current.Value, value))
            {
                return true;
            }
            current = current.Next;
        } while (current != this);

        return false;
    }

    /// <summary>
    /// Determines whether the collection contains a specific value.
    /// </summary>
    /// <param name="item">The value to locate.</param>
    /// <returns>True if the value is found; otherwise, false.</returns>
    public bool Contains(T item)
    {
        return Exists(item);
    }

    /// <summary>
    /// Removes the first occurrence of a specific value from the collection.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <returns>True if the item was successfully removed; false if the item was not found or cannot be removed.</returns>
    /// <remarks>
    /// Cannot remove the only node in the list as it would break the circular structure.
    /// In this case, the method returns false.
    /// </remarks>
    public bool Remove(T item)
    {
        // Case 1: If the item to remove is in the first node (this)
        if (EqualityComparer<T>.Default.Equals(Value, item))
        {
            if (Next == this)
            {
                // Special case: only one node in the list
                // Cannot remove the only node as it would break the circular structure
                // Returning false to indicate the item cannot be removed
                return false;
            }

            // Copy the next node's value to this node and remove the next node instead
            Value = Next.Value;
            Next = Next.Next;
            return true;
        }

        // Case 2: Look for the item in the rest of the list
        NodeCollection<T> current = this;
        while (current.Next != this)
        {
            if (EqualityComparer<T>.Default.Equals(current.Next.Value, item))
            {
                // Remove the found node by updating the Next reference
                current.Next = current.Next.Next;
                return true;
            }
            current = current.Next;
        }

        // Item not found
        return false;
    }

    /// <summary>
    /// Copies the elements of the collection to an array, starting at a particular array index.
    /// </summary>
    /// <param name="array">The destination array.</param>
    /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
    /// <exception cref="ArgumentNullException">Thrown when array is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when arrayIndex is less than 0.</exception>
    /// <exception cref="ArgumentException">Thrown when the destination array is not large enough.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Array index cannot be negative.");
        }

        // Check if there's enough space in the array
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("The destination array is not large enough to hold the elements.", nameof(array));
        }

        NodeCollection<T> current = this;
        int index = arrayIndex;
        do
        {
            array[index] = current.Value;
            index++;
            current = current.Next;
        } while (current != this);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        NodeCollection<T> current = this;
        do
        {
            yield return current.Value;
            current = current.Next;
        } while (current != this);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator for the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Removes all nodes from the list except the current node.
    /// Sets Next to point to itself, effectively isolating this node.
    /// </summary>
    /// <remarks>
    /// Garbage Collection Note:
    /// -----------------------------------------------------------------------
    /// We only need to set Next to itself because in C#, the garbage collector
    /// can detect and collect circular references. Even though removed nodes
    /// form a circular chain pointing to each other, they will be collected
    /// once there are no external references to any node in that chain.
    /// The GC uses a mark-and-sweep algorithm that traces from GC roots,
    /// so unreachable circular structures are properly collected.
    /// Therefore, we don't need to manually break the removed nodes' circular
    /// references - the GC will handle them automatically.
    /// </remarks>
    public void Clear()
    {
        // Simply set Next to this, breaking connection to other nodes
        // The removed nodes will form their own circular chain with no external references
        Next = this;

        // No need to traverse & break chain
        // The GC will collect them as they're now unreachable
    }
}