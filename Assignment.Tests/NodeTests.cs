using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class NodeTests
{
    private static Node<int> CreateSampleList()
    {
        // Due to Append implementation, order from head is:
        // head = 1
        // head.Append(2) => 1, 2
        // head.Append(3) => 1, 3, 2
        var head = new Node<int>(1);
        head.Append(2);
        head.Append(3);
        return head;
    }

    [TestMethod]
    public void GetEnumerator_Generic_ReturnsAllValuesOnceInExpectedOrder()
    {
        // Arrange
        var head = CreateSampleList();

        // Act
        var values = head.ToList();

        // Assert
        CollectionAssert.AreEqual(new List<int> { 1, 3, 2 }, values);
    }

    [TestMethod]
    public void GetEnumerator_GenericSingleNode_ReturnsSingleValue()
    {
        // Arrange
        var node = new Node<string>("only");

        // Act
        var values = node.ToList();

        // Assert
        CollectionAssert.AreEqual(new List<string> { "only" }, values);
    }

    [TestMethod]
    public void ChildItems_MaximumGreaterThanCount_ReturnsAllItems()
    {
        // Arrange
        var head = CreateSampleList();

        // Act
        var values = head.ChildItems(10).ToList();

        // Assert
        CollectionAssert.AreEqual(new List<int> { 3, 2 }, values);
    }

    [TestMethod]
    public void ChildItems_MaximumLessThanCount_TruncatesSequence()
    {
        // Arrange
        var head = CreateSampleList();

        // Act
        var values = head.ChildItems(1).ToList();

        // Assert
        CollectionAssert.AreEqual(new List<int> { 3 }, values);
    }

    [TestMethod]
    public void ChildItems_MaximumZero_ReturnsEmptySequence()
    {
        // Arrange
        var head = CreateSampleList();

        // Act
        var values = head.ChildItems(0).ToList();

        // Assert
        Assert.IsEmpty(values);
    }

    [TestMethod]
    public void ChildItems_NegativeMaximum_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var head = CreateSampleList();

        // Act + Assert
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => head.ChildItems(-1).ToList());
    }
}
