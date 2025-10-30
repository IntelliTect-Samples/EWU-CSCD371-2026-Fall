using GenericsHomework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericsHomeworkTests;
[TestClass]
public class NodeTests
{
    [TestMethod]
    public void Constructor_SingleIntNode_PointsToItself()
    {
        // Arrange & Act
        Node<int> node = new(0);

        // Assert
        Assert.AreEqual<int>(0, node.Value);
        Assert.AreEqual<Node<int>>(node, node.Next);
    }

    [TestMethod]
    public void ToString_NullValue_ReturnsNullString()
    {
        // Arrange
        Node<string> node = new(null!);

        // Act
        string result = node.ToString();

        // Assert
        Assert.AreEqual<string>("null", result);
    }

    [TestMethod]
    public void ToString_IntValue_ReturnsIntString()
    {
        // Arrange
        Node<int> node = new(100);

        // Act
        string result = node.ToString();

        // Assert
        Assert.AreEqual<string>("100", result);
    }

    [TestMethod]
    public void Exists_NodeWithSameIntValue_ReturnsTrue()
    {
        // Arrange
        Node<string> node = new("test");

        // Act
        bool exists = node.Exists("test");

        // Assert
        Assert.AreEqual<bool>(true, exists);
    }

    [TestMethod]
    public void Exists_NodeWithSameNullValue_ReturnsTrue()
    {
        // Arrange
        Node<string> node = new(null!);

        // Act
        bool exists = node.Exists(null!);

        // Assert
        Assert.AreEqual<bool>(true, exists);
    }

    [TestMethod]    
    public void Exists_NodeWithDifferentValue_ReturnsFalse()
    {
        // Arrange
        Node<int> node = new(10);

        // Act
        bool exists = node.Exists(100);

        // Assert
        Assert.AreEqual<bool>(false, exists);
    }

    [TestMethod]
    public void Clear_NodeWithMultipleValues_ClearsAllButHead()
    {
        // Arrange
        Node<int> node = new(1);
        node.Append(2);
        node.Append(3);

        // Act
        node.Clear();

        // Assert
        Assert.AreEqual<bool>(false, node.Exists(2));
        Assert.AreEqual<bool>(false, node.Exists(3));
        Assert.AreEqual<bool>(true, node.Exists(1));
    }

    [TestMethod]
    public void Append_NewValue_AppendsNodeSuccessfully()
    {
        // Arrange
        Node<int> node = new(1);

        // Act
        node.Append(2);

        // Assert
        Assert.AreEqual<int>(2, node.Next.Value);
        Assert.AreEqual<Node<int>>(node, node.Next.Next);
    }

    [TestMethod]
    public void Append_DuplicateValue_ThrowsArgumentException()
    {
        // Arrange
        Node<int> node = new(1);

        // Act & Assert
        Assert.ThrowsException<ArgumentException>(() => node.Append(1));
    }
}
