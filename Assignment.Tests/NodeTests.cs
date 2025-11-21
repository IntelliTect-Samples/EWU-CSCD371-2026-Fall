using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class NodeTests
{
    [TestMethod]
    public void IEnumerator_SingleNode_EnumerationContainsOnlySelf()
    {
        // Arrange
        Node<string> node = new("A");

        // Act
        List<string> items = node.ToList();

        // Assert
        Assert.HasCount(1, items);
        Assert.AreEqual("A", items[0]);
    }
    [TestMethod]
    public void Append_AppendAndEnumerate_ReturnsExpectedOrder()
    {
        // Arrange
        Node<string> node = new("A");
        node.Append("B");
        node.Append("C");
        node.Append("D");

        // Act
        List<string> actual = node.ToList();
        List<string> expected = new() { "A", "D", "C", "B" };

        // Assert
        Assert.IsTrue(actual.SequenceEqual(expected));
    }
    [TestMethod]
    public void GetEnumerator_MultipleEnumeration_ReturnsSameSequence()
    {
        // Arrange
        Node<int> node = new(1);
        node.Append(2);
        node.Append(3);

        // Act
        List<int> first = node.ToList();
        List<int> second = node.ToList();

        // Assert
        Assert.IsTrue(first.SequenceEqual(second));
    }
    [TestMethod]
    public void LinqOperations_CountAndElmentAt_WorkOnNodeEnumeration()
    {
        // Arrange
        Node<char> node = new('x');
        node.Append('y');
        node.Append('z');

        // Act / Assert
        Assert.AreEqual<int>(3, node.Count());
        Assert.AreEqual('z', node.ElementAt(1));
    }
    [TestMethod]
    public void ChildItems_AccessChildItems_ReturnsExpectedValues()
    {
        // Arrange
        Node<string> node = new("A");
        node.Append("B");
        node.Append("C");
        node.Append("D"); // append order yields: A -> D -> C -> B -> A

        // Act
        List<string> twoChildren = node.ChildItems(3).ToList();
        List<string> noneWhenOne = node.ChildItems(1).ToList(); 
        List<string> allChildrenWhenLarge = node.ChildItems(10).ToList();

        // Assert
        CollectionAssert.AreEqual(new List<string> { "D", "C" }, twoChildren);
        Assert.IsEmpty(noneWhenOne);
        CollectionAssert.AreEqual(new List<string> { "D", "C", "B" }, allChildrenWhenLarge);
    }
}
