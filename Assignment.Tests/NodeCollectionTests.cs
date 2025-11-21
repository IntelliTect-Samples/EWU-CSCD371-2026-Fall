using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Assignment.Tests;

[TestClass]
public class NodeCollectionTests
{
    [TestMethod]
    public void Constructor_ProperValuePassedIn_SetsPropertiesCorrectly()
    {
        // Arrange
        int expectedValue = 10;
        // Act
        Node<int> node = new Node<int>(expectedValue);
        // Assert
        Assert.AreEqual<int>(expectedValue, node.Value);
        Assert.AreEqual<Node<int>>(node, node.Next);
    }

    [TestMethod]
    public void Append_NewUniqueValue_AppendsSuccessfully()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        int newValue = 2;
        // Act
        node.Append(newValue);
        // Assert
        Assert.AreEqual<int>(newValue, node.Next.Value);
        Assert.AreEqual<Node<int>>(node, node.Next.Next);
    }

    [TestMethod]
    public void Append_DuplicateValue_ThrowsInvalidOperationException()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => node.Append(1));
    }

    [TestMethod]
    public void Exists_ValueExists_ReturnsTrue()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        // Act
        bool exists = node.Exists(2);
        // Assert
        Assert.IsTrue(exists);
    }

    [TestMethod]
    public void Exists_ValueDoesNotExist_ReturnsFalse()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        // Act
        bool exists = node.Exists(3);
        // Assert
        Assert.IsFalse(exists);
    }

    [TestMethod]
    public void ToString_ReturnsValueStringRepresentation()
    {
        // Arrange
        Node<int> node = new Node<int>(42);
        // Act
        string result = node.ToString();
        // Assert
        Assert.AreEqual<string>("42", result);
    }

    [TestMethod]
    public void ToString_NullValue_ReturnsEmptyString()
    {
        // Arrange
        Node<string> node = new Node<string>(null!);
        // Act
        string result = node.ToString();
        // Assert
        Assert.AreEqual<string>(string.Empty, result);
    }

    [TestMethod]
    public void Clear_RemovesAllNodesExceptCurrent_Success()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        node.Append(3);
        // Act
        node.Clear();
        // Assert
        Assert.AreEqual<Node<int>>(node, node.Next);
    }

    [TestMethod]
    public void GetEnumerator_IteratesThroughAllNodes_Success()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        node.Append(3);
        // Act
        List<int> values = new List<int>();
        foreach (int val in node)
        {
            values.Add(val);
        }
        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, values);
    }

    [TestMethod]
    public void ChildItems_IteratesThroughAllNodes_Success()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        node.Append(3);
        // Act
        List<int> values = new List<int>();
        foreach (int val in node.ChildItems(10))
        {
            values.Add(val);
        }
        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, values);
    }

    [TestMethod]
    public void ChildItems_LimitIterations_Success()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        node.Append(3);
        // Act
        List<int> values = new List<int>();
        foreach (int val in node.ChildItems(2))
        {
            values.Add(val);
        }
        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 2 }, values);
    }

    [TestMethod]
    public void ChildItems_ZeroLimit_NoIterations()
    {
        // Arrange
        Node<int> node = new Node<int>(1);
        node.Append(2);
        node.Append(3);
        // Act
        List<int> values = new List<int>();
        foreach (int val in node.ChildItems(0))
        {
            values.Add(val);
        }
        // Assert
        CollectionAssert.AreEqual(new List<int> { }, values);
    }
}
