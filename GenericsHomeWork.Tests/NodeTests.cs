namespace GenericsHomeWork.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using GenericsHomeWork;
using System;
using System.Globalization;

[TestClass]
public class NodeTests
{
    [TestMethod]
    public void Node_SetsValue_PointsToSelf()
    {
        // Arrange
        int expectedValue = 67;

        // Act
        NodeCollection<int> node = new NodeCollection<int>(expectedValue);

        // Assert
        Assert.AreEqual(expectedValue, node.Value);
        Assert.AreEqual(node, node.Next);
    }

    [TestMethod]

    public void Node_IsNeverNull_ReturnsNotNull()
    {
        // Arrange & Act
        var node = new NodeCollection<object>(new object());

        // Assert
        Assert.IsNotNull(node, "Node instance shouldnt be null after its creation.");
        Assert.IsNotNull(node.Next, "Next should point to itself and not be null");
    }

    [TestMethod]

    public void Node_AfterAppending_IsNotNull()
    {
        // Arrange
        var head = new NodeCollection<string>("head");

        // Act
        head.Append("newNode");

        // Assert
        Assert.IsNotNull(head.Next, "Next node after appending should not be null.");
        Assert.AreNotEqual(head, head.Next, "Next node should not be the same as head after appending.");
    }


    [TestMethod]
    public void Append_InsertsNewNode_AfterCurrentNode()
    {
        // Arrange
        var head = new NodeCollection<string>("first");
        // Act
        head.Append("second");
        // Assert
        Assert.AreEqual("second", head.Next.Value, "String 'second' should be inserted after 'first'");
        Assert.AreEqual(head, head.Next.Next, "Next of 'second' should point back to head node 'first'.");

        // Act
        head.Append("third");
        // Assert
        Assert.AreEqual("third", head.Next.Value, "String 'third' should be inserted after 'second'.");
        Assert.AreEqual("second", head.Next.Next.Value, "String 'second' should still be after 'first'.");
        Assert.AreEqual(head, head.Next.Next.Next, "Next of 'third' should point back to head node 'first'.");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Append_ThrowsException_OnDuplicateValue()
    {
        // Arrange
        var head = new NodeCollection<int>(10);
        head.Append(20);
        // Act
        head.Append(10);
    }

    [TestMethod]
    public void Exists_ReturnsTrue_IfValueExists()
    {
        // Arrange
        var head = new NodeCollection<double>(1.1);
        head.Append(2.2);
        head.Append(3.3);
        // Act & Assert
        Assert.IsTrue(head.Exists(1.1), "Value 1.1 should exist in the list.");
        Assert.IsTrue(head.Exists(2.2), "Value 2.2 should exist in the list.");
        Assert.IsTrue(head.Exists(3.3), "Value 3.3 should exist in the list.");
    }

    [TestMethod]
    public void Exists_ReturnsFalse_IfValueDoesNotExist()
    {
        // Arrange
        var head = new NodeCollection<char>('A');
        head.Append('B');
        head.Append('C');
        // Act & Assert
        Assert.IsFalse(head.Exists('D'), "Value 'D' should not exist in the list.");
        Assert.IsFalse(head.Exists('E'), "Value 'E' should not exist in the list.");
    }

    [TestMethod]
    public void Clear_RemovesAllNodes_ExceptHead()
    {
        // Arrange
        var head = new NodeCollection<string>("root");
        head.Append("child1");
        head.Append("child2");
        head.Append("child3");
        // Act
        head.Clear();
        // Assert
        Assert.AreEqual(head, head.Next, "After Clear(), head's Next should point to itself.");
        Assert.IsFalse(head.Exists("child1"), "Value 'child1' should not exist after Clear().");
        Assert.IsFalse(head.Exists("child2"), "Value 'child2' should not exist after Clear().");
        Assert.IsFalse(head.Exists("child3"), "Value 'child3' should not exist after Clear().");
    }

    [TestMethod]
    public void Clear_OnSingleNodeList_DoesNothing()
    {
        // Arrange
        var head = new NodeCollection<int>(42);
        // Act
        head.Clear();
        // Assert
        Assert.AreEqual(head, head.Next, "After Clear() on single node list, head's Next should still point to itself.");
        Assert.IsTrue(head.Exists(42), "Value 42 should still exist after Clear() on single node list.");
    }

    [TestMethod]
    public void Clear_IsololatesCurrentNode()
    {
        // Arrange
        var head = new NodeCollection<int>(1);
        head.Append(2);
        head.Append(3);

        // Act
        head.Clear();
        // Assert
        Assert.AreEqual(head, head.Next, "After Clear(), head's Next should point to itself.");
        Assert.AreEqual(1, head.Value, "Head node's value should remain unchanged after Clear().");
    }

    [TestMethod]
    public void ToString_ReturnsValueStringRepresentation()
    {
        // Arrange
        string value = "TestNode";
        var node = new NodeCollection<string>(value);
        // Act
        string result = node.ToString();
        // Assert
        Assert.AreEqual(value, result, "ToString() should return the string representation of the node's value.");
    }

    [TestMethod]
    public void ToString_ReturnsValueIntegerRepresentation()
    {
        // Arrange
        int value = 12345;
        var node = new NodeCollection<int>(value);
        // Act
        string result = node.ToString();
        // Assert
        Assert.AreEqual(value.ToString(CultureInfo.InvariantCulture), result, "ToString() should return the string representation of the integer value.");
    }

    [TestMethod]
    public void ToString_ReturnsEmptyString_ForNullValue()
    {
        // Arrange
        string? value = null;
        var node = new NodeCollection<string>(value!);
        // Act
        string result = node.ToString();
        // Assert
        Assert.AreEqual(string.Empty, result, "ToString() should return an empty string for null value.");
    }

    [TestMethod]
    public void Add_FunctionallyEquivalentToAppend()
    {
        // Arrange
        var head = new NodeCollection<string>("A");
        // Act
        ((ICollection<string>)head).Add("B");
        // Assert
        Assert.AreEqual(1, head.Count, "Add() should increase the count.");
        Assert.AreEqual("B", head.Next!.Value, "Add() should insert 'B' after 'A'.");
    }

    [TestMethod]
    public void Count_CalculatesCorrectNumberOfNodes()
    {
        // Arrange 
        var head = new NodeCollection<int>(100);
        Assert.AreEqual(0, head.Count, "Initial count should be 0.");
        // Act
        head.Add(200);
        head.Add(300);
        // Assert
        Assert.AreEqual(2, head.Count, "Count should be 2 after adding two nodes.");
        // Act
        head.Clear();
        // Assert
        Assert.AreEqual(0, head.Count, "Count should be 0 after clearing the list.");
    }

    [TestMethod]
    public void Remove_RemovesExistingNode_ReturnsTrue()
    {
        // Arrange
        var head = new NodeCollection<int>(1);
        head.Add(2);
        head.Add(3);
        // Act
        bool result = head.Remove(2);
        // Assert
        Assert.IsTrue(result, "Remove should return true for an existing item.");
        Assert.AreEqual(1, head.Count, "Count should decrease after removal.");
        Assert.IsFalse(head.Contains(2), "The removed item should no longer exist.");
        Assert.AreEqual(3, head.Next!.Value, "List structure should be preserved: 1 -> 3 -> 1.");
        // Act
        head.Remove(3);
        // Assert
        Assert.AreEqual(0, head.Count, "Count should be 0 after removing all nodes.");
        Assert.AreEqual(head, head.Next, "Next should point to head after all nodes are removed.");
    }

    [TestMethod]
    public void Remove_NonExistingNode_ReturnsFalse()
    {
        // Arrange
        var head = new NodeCollection<string>("Start");
        head.Add("One");
        // Act
        bool result = head.Remove("Missing");
        // Assert
        Assert.IsFalse(result, "Remove should return false for a non-existing item.");
        Assert.AreEqual(1, head.Count, "Count should remain unchanged after attempting to remove a non-existing item.");
    }

    [TestMethod]
    public void Remove_HeadNodeValue_ReturnsFalse()
    {
        // Arrange
        var head = new NodeCollection<int>(10);
        head.Add(20);
        // Act
        bool result = head.Remove(10);
        // Assert
        Assert.IsFalse(result, "Remove should return false when trying to remove the head node's value.");
        Assert.AreEqual(1, head.Count, "Count should remain unchanged when attempting to remove the head node's value.");
    }

    [TestMethod]
    public void CopyTo_CopiesElementsToCorrectArray()
    {
        // Arrange
        var head = new NodeCollection<double>(0.0);
        head.Add(1.1);
        head.Add(2.2);
        head.Add(3.3);
        double[] array = new double[5];
        int index = 1;
        // Act
        head.CopyTo(array, index);
        // Assert
        Assert.AreEqual(0.0, array[0], "Index 0 should be untouched.");
        Assert.AreEqual(3.3, array[1], "First copied element should be at index 1.");
        Assert.AreEqual(2.2, array[2]);
        Assert.AreEqual(1.1, array[3], "Last copied element should be at index 3.");
        Assert.AreEqual(0.0, array[4], "Index 4 should be untouched.");
    }

    [TestMethod]
    public void GetEnumerator_AllowsForeachTraversal()
    {
        // Arrange
        var head = new NodeCollection<string>("Start");
        head.Add("Last");
        head.Add("Middle");
        var expected = new List<string> { "Middle", "Last" };
        var actual = new List<string>();
        // Act
        foreach (var item in head)
        {
            actual.Add(item);
        }
        // Assert
        CollectionAssert.AreEqual(expected, actual, "Enumerator should allow traversal of all nodes except head.");
    }

    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        // Arrange
        var head = new NodeCollection<int>(5);
        // Act
        bool isReadOnly = ((ICollection<int>)head).IsReadOnly;
        // Assert
        Assert.IsFalse(isReadOnly, "IsReadOnly should return false for Node<T>.");
    }
}