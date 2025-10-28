using System;

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

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

        public void Append(T value)
        {
            Node<T> current = this;
            do
            {
                if (object.Equals(current.Value, value))
                    throw new InvalidOperationException("Duplicate value detected");
                current = current.Next;
            }while (current != this);

            var newNode = new Node<T>(value) { Next = this.Next };
            this.Next = newNode;
        }

        /// <summary>
        /// Removes all nodes from the circular list except the current node.
        /// After this call the current node will point to itself (a single-node circular list).
        /// </summary>

        //Note: Garbage Collection in .NET is automatic and can detect, clean up cyclic references
        //Therefore, although the nodes form a circular list,
        //we don't need to manually break the loop for memory management

        public void Clear()
        {
            //if this is already a single-node list nothing to do
            if (this.Next == this)
                return;

            //first node to be removed
            Node<T> firstRemoved = this.Next;

            //find the last node in the removed segment (node whose Next points to this)
            Node<T> lastRemoved = firstRemoved;
            while (lastRemoved.Next != this)
            {
                lastRemoved = lastRemoved.Next;
            }

            //close the loop of removed nodes so they form their own circular list
            lastRemoved.Next = firstRemoved;

            //isolate this node
            this.Next = this;
        }

        /// <summary>
        /// Returns true if the provided value exists in the circular list (compares with object.Equals).
        /// </summary>
        public bool Exists(T value)
        {
            Node<T> current = this;
            do
            {
                if (object.Equals(current.Value, value))
                    return true;
                current = current.Next;
            } while (current != this);

            return false;
        }
    }

