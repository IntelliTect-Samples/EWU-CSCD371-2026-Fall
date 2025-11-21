#pragma warning disable MSTEST0037

using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AssignmentTests;

[TestClass]
public class NodeTests
{
    [TestMethod]
    public void NodeEnum_SingleNode_EnumeratesSingleValue()
    {
        var root = new Node<int>(42);

        List<int> values = root.ToList();

        Assert.AreEqual<int>(1, values.Count);
        Assert.AreEqual<int>(42, values[0]);
    }

    [TestMethod]
    public void NodeEnum_MultipleNodes_EnumeratesFullCircleOnce()
    {
        var root = new Node<int>(1);
        var n2 = root.Append(2);
        var n3 = n2.Append(3);

        List<int> values = root.ToList();

        CollectionAssert.AreEqual(new List<int> { 1, 2, 3 }, values);
    }

    [TestMethod]
    public void ChildItems_SmallMax_returnsEmpty()
    {
        var root = new Node<int>(1);
        root.Append(2);

        var result = root.ChildItems(0).ToList();

        Assert.AreEqual<int>(0, result.Count);
    }

    [TestMethod]
    public void ChildItems_ValidInputs_ReturnsChildrenUpToMax()
    {
        var root = new Node<int>(1);
        var n2 = root.Append(2);
        var n3 = n2.Append(3);
        var n4 = n3.Append(4);

        List<int> children = root.ChildItems(2).ToList();

        CollectionAssert.AreEqual(new List<int> { 2, 3 }, children);
    }
}