using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GenericsHomework.Tests;

[TestClass]
public class NodeICollectionTests
{

    private static readonly int[] ExpectedCopyTo = new int[] { 0, 5, 6, 7, 0 };
    private static readonly List<string> ExpectedEnumeratedItems = new List<string> { "x", "y", "z" };

    [TestMethod]
    public void ICollectionCountSingleAndMultiSuccess()
    {
        var n = new Node<int>(1);
        Assert.AreEqual<int>(1, ((ICollection<int>)n).Count);

        n.Append(2);
        n.Append(3);
        Assert.AreEqual<int>(3, ((ICollection<int>)n).Count);
    }

    [TestMethod]
    public void ICollectionAddIntoEmptyAfterClearSuccess()
    {
        var n = new Node<string>("first");
        n.Clear();
        ((ICollection<string>)n).Add("x");

        Assert.AreEqual<int>(1, ((ICollection<string>)n).Count);
        Assert.IsTrue(n.Exists("x"));
        Assert.AreSame(n, n.Next);
    }

    [TestMethod]
    public void ICollectionRemoveHeadOnlyItemAndEmptySuccess()
    {
        var n = new Node<int>(42);
        bool removed = ((ICollection<int>)n).Remove(42);

        Assert.IsTrue(removed);
        Assert.AreEqual<int>(0, ((ICollection<int>)n).Count);
        Assert.IsFalse(n.Exists(42));
        Assert.AreSame(n, n.Next);
    }

    [TestMethod]
    public void ICollectionRemoveHeadInMultiListShiftsHeadSuccess()
    {
        var n = new Node<string>("a");
        n.Append("b");
        n.Append("c");

        bool removed = ((ICollection<string>)n).Remove("a");
        Assert.IsTrue(removed);

        Assert.AreEqual<int>(2, ((ICollection<string>)n).Count);
        Assert.IsFalse(n.Exists("a"));
        Assert.IsTrue(n.Exists("b"));
        Assert.IsTrue(n.Exists("c"));
        Assert.AreSame(n, n.Next.Next);
    }

    [TestMethod]
    public void ICollectionRemoveMiddleNodeSuccess()
    {
        var n = new Node<int>(1);
        n.Append(2);
        n.Append(3);

        bool removed = ((ICollection<int>)n).Remove(2);
        Assert.IsTrue(removed);
        Assert.AreEqual<int>(2, ((ICollection<int>)n).Count);
        Assert.IsFalse(n.Exists(2));
        Assert.IsTrue(n.Exists(1));
        Assert.IsTrue(n.Exists(3));
    }

    [TestMethod]
    public void ICollectionCopyToWritesSequentiallySuccess()
    {
        var n = new Node<int>(5);
        n.Append(6);
        n.Append(7);

        var arr = new int[5];
        ((ICollection<int>)n).CopyTo(arr, 1);

        CollectionAssert.AreEqual(ExpectedCopyTo, arr);
    }

    [TestMethod]
    public void ICollectionEnumeratesAllItemsOnceSuccess()
    {
        var n = new Node<string>("x");
        n.Append("y");
        n.Append("z");

        var items = ((IEnumerable<string>)n).ToList();
        CollectionAssert.AreEquivalent(ExpectedEnumeratedItems, items);
        Assert.AreEqual<int>(3, items.Count);
    }

    [TestMethod]
    public void ICollectionIsReadOnlyIsFalseSuccess()
    {
        var n = new Node<int>(1);
        Assert.IsFalse(((ICollection<int>)n).IsReadOnly);
    }

}
