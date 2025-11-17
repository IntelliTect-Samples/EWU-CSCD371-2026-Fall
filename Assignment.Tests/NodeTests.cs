using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class NodeTests
{
    private NodeCollection<int> MakeCircularlyLinkedList(params int[] values)
    {
        if (values == null || values.Length == 0)
        {
            throw new ArgumentException("Cannot create an empty circular list for this test setup.");
        }

        var head = new NodeCollection<int>(values[0]);

        for (int i = 1; i < values.Length; i++)
        {
            head.Append(values[i]);
        }

        return head;
    }

    [TestMethod]
    public void Values_EnumerateInCircle()
    {
        // Arrange
        var head = MakeCircularlyLinkedList(15, 22, 26, 33);
        var expectedOrder = new List<int> { 15, 33, 26, 22 };

        // Act
        var actualOrder = head.Values().ToList();

        // Assert
        Assert.HasCount(4, actualOrder, "Incorrect count, method did not enumerate correctly.");
        bool areEqual = expectedOrder.Zip(actualOrder, (expected, actual) => expected.Equals(actual)).All(b => b);
        Assert.IsTrue(areEqual, "Values are not in correct order.");
        var singleValueList = new NodeCollection<int>(10);
        CollectionAssert.AreEqual(new List<int> { 10 }, singleValueList.Values().ToList(), "Single value list failed.");
    }

    [TestMethod]
    public void Values_ShouldHandleSingleNodeCorrectly()
    {
        // Arrange
        var singleNode = new NodeCollection<int>(99);
        var expected = new List<int> { 99 };

        // Act
        var actual = singleNode.Values().ToList();

        // Assert
        Assert.HasCount(1, actual, "Single node list count failed.");
        CollectionAssert.AreEqual(expected, actual, "Single node list item failed.");
    }

    [TestMethod]
    public void ChildItems_ReturnLessThanMaxSTartingFromChild()
    {
        // Arrange
        var head = MakeCircularlyLinkedList(1, 2, 3, 4);
        int maxLimit1 = 3;
        var expected1 = new List<int> { 4, 3 };

        // Act actual1
        var actual1 = head.ChildItems(maxLimit1).ToList();

        // Assert actual1
        Assert.HasCount(maxLimit1 - 1, actual1, "Test Case 1: Count failed (Expected max - 1).");
        CollectionAssert.AreEqual(expected1, actual1, "Test Case 1: Items failed (Did not return the first 2 children).");
        int maxLimit2 = 1;
        var actual2 = head.ChildItems(maxLimit2).ToList();

        // Assert 2
        Assert.HasCount(0, actual2, "Test Case 2: Count failed (Expected 0).");

        // Test Case 3: max=10 (More than available children). Should return all 3 children.
        int maxLimit3 = 10;
        var expected3 = new List<int> { 4, 3, 2 };
        var actual3 = head.ChildItems(maxLimit3).ToList();

        // Assert 3
        Assert.HasCount(3, actual3, "Test Case 3: Count failed (Should have returned all children).");
        CollectionAssert.AreEqual(expected3, actual3, "Test Case 3: Items failed (Did not return all children).");
    }

    [TestMethod]
    public void ChildItems_MaxZeroReturnsEmptyCollection()
    {
        // Arrange
        var head = MakeCircularlyLinkedList(1, 2, 3);
        int maxLimit = 0;

        // Act
        var actual = head.ChildItems(maxLimit).ToList();

        // Assert
        Assert.HasCount(0, actual, "ChildItems failed to return an empty collection for Max=0.");
    }

    [TestMethod]
    public void ChildItems_MaxTwoReturnsOneItem()
    {
        // Arrange
        var head = MakeCircularlyLinkedList(1, 2, 3, 4);
        int maxLimit = 2;
        var expected = new List<int> { 4 };

        // Act
        var actual = head.ChildItems(maxLimit).ToList();

        // Assert
        Assert.HasCount(1, actual, "ChildItems failed to return exactly 1 item for Max = 2.");
        CollectionAssert.AreEqual(expected, actual, "ChildItems did not return the correct single item.");
    }

    [TestMethod]
    public void ChildItems_MaxExceedsChildrenStopsAtEnd()
    {
        // Arrange
        var head = MakeCircularlyLinkedList(1, 2, 3);
        int maxLimit = 10;
        var expected = new List<int> { 3, 2 };

        // Act
        var actual = head.ChildItems(maxLimit).ToList();

        // Assert
        Assert.HasCount(2, actual, "ChildItems failed to stop at the end of the circle.");
        CollectionAssert.AreEqual(expected, actual, "ChildItems returned incorrect items when max was high.");
    }
}
