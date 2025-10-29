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

    [TestMethod]
    public void Node_Tostring_PrintsNullWhenNull()
    {
        // Arrange
        var node = new Node<string>(null!);
        // Act
        var result = node.ToString();
        // Assert
        Assert.AreEqual<string>("null", result);
    }

    [TestMethod]
    public void Node_Next_SingleNodeIsNext()
    {
        // Arrange
        var node = new Node<string>("test");
        // Act
        // Assert
        Assert.AreEqual<Node<string>>(node.Next, node);
    }

    [TestMethod]
    public void Node_Append_AppendsNext()
    {
        // Arrange
        var node = new Node<string>("test1");
        string test2 = "test2";
        // Act
        node.Append(test2);
        // Assert
        Assert.AreEqual<string>(node.Next.Value, test2);
    }
    [TestMethod]
    public void Node_Append_AppendsNextNext()
    {
        // Arrange
        var node = new Node<string>("test1");
        string test2 = "test2";
        string test3 = "test3";
        // Act
        node.Append(test2);
        node.Append(test3);

        // Assert
        Assert.AreEqual<string>(node.Next.Next.Value, test2);
        Assert.AreEqual<string>(node.Next.Value, test3);
    }
}