using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GenericsHomework;

namespace GenericsHomework.Tests;

[TestClass]
public class NodeTests
{
    [TestMethod]
    public void Constructor_SetsValues_WhenNodeIsMade()
    {
        var node = new Node<int>(1);

        Assert.AreEqual<int>(1, node.Value);
        Assert.AreEqual<Node<int>>(node, node.Next);
    }

    [TestMethod]
    public void ToString_DisplaysValue_WhenCalled()
    {
        var node = new Node<int>(2);

        Assert.AreEqual<string>("2", node.ToString());
    }

    [TestMethod]
    public void Append_AddsNewNode_AfterCurrent()
    {
        var first = new Node<int>(1);
        var second = first.Append(2);
        var third = first.Append(3);

        Assert.AreEqual<Node<int>>(third, first.Next);
        Assert.AreEqual<Node<int>>(second, third.Next);
        Assert.AreEqual<Node<int>>(first, second.Next);
    }

    [TestMethod]
    public void Append_ThrowsException_WhenDuplicateValueAdded()
    {
        var node = new Node<int>(4);

        Assert.Throws<ArgumentException>(() => node.Append(4));
    }

    [TestMethod]
    public void Clear_SetsNextToItself_WhenCalled()
    {
        var first = new Node<int>(5);
        var second = first.Append(-1);

        first.Clear();

        Assert.AreEqual<Node<int>>(first, first.Next);
    }

    [TestMethod]
    public void Clear_DoesNothing_WhenCalledOnSingleNode()
    {
        var node = new Node<int>(6);

        node.Clear();

        Assert.AreEqual<Node<int>>(node, node.Next);
    }

    [TestMethod]
    public void Exists_ReturnsTrue_WhenIntValueIsInList()
    {
        var first = new Node<int>(7);
        first.Append(-1);
        first.Append(-2);

        Assert.IsTrue(first.Exists(7));
    }

    [TestMethod]
    public void Exists_ReturnsTrue_WhenStringValueIsInList()
    {
        var first = new Node<string>("seven");
        first.Append("negative one");
        first.Append("negative two");

        Assert.IsTrue(first.Exists("seven"));
    }

    [TestMethod]
    public void Exists_ReturnsFalse_WhenIntValueIsNotInList()
    {
        var first = new Node<int>(8);
        first.Append(-1);
        first.Append(-2);

        Assert.IsFalse(first.Exists(0));
    }

    [TestMethod]
    public void Exists_ReturnsFalse_WhenStringValueIsNotInList()
    {
        var first = new Node<string>("eight");
        first.Append("negative one");
        first.Append("negative two");

        Assert.IsFalse(first.Exists("zero"));
    }
}
