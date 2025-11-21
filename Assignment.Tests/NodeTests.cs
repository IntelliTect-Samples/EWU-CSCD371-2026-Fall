using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;
using Assignment;

namespace Assignment.Tests;

[TestClass]
public class NodeTests
{
    // -------------------------------------------------------------------------
    // ENUMERATOR TESTS
    // -------------------------------------------------------------------------

    [TestMethod]
    public void GetEnumerator_IteratesCircleOnce_IncludesSelf()
    {
        var node1 = new Node<int>(1);
        var node2 = new Node<int>(2);
        var node3 = new Node<int>(3);

        node1.Next = node2;
        node2.Next = node3;
        node3.Next = node1; 

        var result = node1.ToList();

        // Uses the helper at bottom
        AssertExtensions.HasCount(3, result);
        
        // Explicit <int> removes ambiguity errors
        Assert.AreEqual<int>(1, result[0]); 
        Assert.AreEqual<int>(2, result[1]);
        Assert.AreEqual<int>(3, result[2]);
    }

    [TestMethod]
    public void GetEnumerator_HandlesLinearList_EndsAtNull()
    {
        var node1 = new Node<int>(10);
        var node2 = new Node<int>(20);
        var node3 = new Node<int>(30);

        node1.Next = node2;
        node2.Next = node3;
        node3.Next = null!; 

        var result = node1.ToList();

        AssertExtensions.HasCount(3, result);
        Assert.AreEqual<int>(30, result[2]);
    }

    [TestMethod]
    public void GetEnumerator_SingleNode_ReturnsSelf()
    {
        var node = new Node<int>(99);
        var result = node.ToList();

        AssertExtensions.HasCount(1, result);
        Assert.AreEqual<int>(99, result[0]);
    }

    [TestMethod]
    public void GetEnumerator_WorksWithStrings_GenericsCheck()
    {
        var n1 = new Node<string>("Hello");
        var n2 = new Node<string>("World");
        n1.Next = n2;
        n2.Next = n1;

        var result = n1.ToList();

        AssertExtensions.HasCount(2, result);
        Assert.AreEqual("Hello", result[0]);
        Assert.AreEqual("World", result[1]);
    }

    // -------------------------------------------------------------------------
    // CHILD ITEMS TESTS
    // -------------------------------------------------------------------------

    [TestMethod]
    public void ChildItems_SkipsParent_And_ReturnsChildren()
    {
        var nodeA = new Node<string>("A");
        var nodeB = new Node<string>("B");
        var nodeC = new Node<string>("C");

        nodeA.Next = nodeB;
        nodeB.Next = nodeC;
        nodeC.Next = nodeA;

        var children = nodeA.ChildItems(5).ToList();

        AssertExtensions.HasCount(2, children); 
        Assert.AreEqual("B", children[0]);
        Assert.AreEqual("C", children[1]);
    }

    [TestMethod]
    public void ChildItems_RespectsMaximumLimit()
    {
        var nodeA = new Node<int>(1);
        var nodeB = new Node<int>(2);
        var nodeC = new Node<int>(3);
        var nodeD = new Node<int>(4);

        nodeA.Next = nodeB;
        nodeB.Next = nodeC;
        nodeC.Next = nodeD;
        nodeD.Next = nodeA;

        var children = nodeA.ChildItems(2).ToList();

        AssertExtensions.HasCount(2, children);
        Assert.AreEqual<int>(2, children[0]);
        Assert.AreEqual<int>(3, children[1]);
    }

    [TestMethod]
    public void ChildItems_SingleNode_ReturnsEmpty()
    {
        var node = new Node<int>(55);
        var children = node.ChildItems(10).ToList();

        AssertExtensions.HasCount(0, children);
    }
}

// ============================================================================
// HELPER CLASS
// Fixed to handle any Enumerable without overload ambiguity
// ============================================================================
public static class AssertExtensions
{
    public static void HasCount<T>(int expectedCount, IEnumerable<T> collection)
    {
        // 1. Convert to list to safely count without side effects
        var list = collection.ToList();
        
        // 2. Explicitly check int vs int to avoid compilation errors
        if (list.Count != expectedCount)
        {
            // We use 'Assert.Fail' which is cleaner than 'AreEqual' for custom messages
            Assert.Fail($"Expected count {expectedCount} but found {list.Count}.");
        }
    }
}