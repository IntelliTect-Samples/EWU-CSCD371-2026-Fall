using GenericsHomework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericHomework.Tests;

    [TestClass]
public class NodeTests
{
    [TestMethod]
    public void Node_ToString_ReturnsValueString()
    {
        // Arrange
        var node = new Node<int>(42);
        // Act
        var result = node.ToString();
        // Assert
        Assert.AreEqual<string>("42", result);
    }

    [TestMethod]
    public void Node_AppendValueAlreadyExists_ThrowsException()
    {
        // Arrange
        var node = new Node<int>(42);
        node.Append(43);
        // Act & Assert
        Assert.Throws<ArgumentException>(() => node.Append(42));
    }

    [TestMethod]
    public void Node_Clear_RemovesAllElements()
    {
        // Arrange
        var node = new Node<int>(1);
        node.Append(2);
        node.Append(3);
        // Act
        node.Clear();
        // Assert
        Assert.IsFalse(node.Exists(2));
        Assert.IsFalse(node.Exists(3));
        Assert.IsTrue(node.Exists(1));
    }
}