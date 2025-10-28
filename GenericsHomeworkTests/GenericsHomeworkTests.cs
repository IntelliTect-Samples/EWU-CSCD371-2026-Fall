using GenericsHomework;

namespace GenericsHomeworkTests;

[TestClass]
public class GenericsHomeworkTests
{
    [TestMethod]
    [DataRow("hello", "hello")]
    [DataRow(null, "null")]
    public void ToString_ValueIsString_ReturnsValueToString(string input, string expected)
    {
        // Arrange
        Node<string> node = new(input);

        // Act
        string result = node.ToString();

        // Assert
        Assert.AreEqual<string>(expected, result);
    }

    [TestMethod]
    [DataRow(42, "42")]
    public void ToString_ValueIsInt_ReturnsValueToString(int input, string expected)
    {
        // Arrange
        Node<int> node = new(input);

        // Act
        string result = node.ToString();

        // Assert
        Assert.AreEqual<string>(expected, result);
    }

    [TestMethod]
    public void Constructor_InitializesNextToSelf_SelfReferenceConfirmed()
    {
        // Arrange
        Node<int> node = new(42);

        // Act
        Node<int> next = node.Next;

        // Assert
        Assert.AreEqual<Node<int>>(node, next);
    }

    [TestMethod]
    public void Append_ThreeNodesCircular_CorrectLinkingConfirmed()
    {
        // Arrange
        Node<int> first = new(1);

        // Act
        Node<int> second = first.Append(2);
        Node<int> third = second.Append(3);

        // Assert
        Assert.AreEqual<int>(2, first.Next.Value);
        Assert.AreEqual<int>(3, first.Next.Next.Value);
        Assert.AreEqual<int>(1, first.Next.Next.Next.Value); // Circular
    }


    [TestMethod]
    public void Exists_ValueIsInSingleNode_ReturnsTrue()
    {
        // Arrange
        Node<string> node = new Node<string>("hello");

        // Act
        bool result = node.Exists("hello");

        // Assert
        Assert.AreEqual<bool>(true, result);
    }

    [TestMethod]
    public void Exists_ValueIsNotInSingleNode_ReturnsFalse()
    {
        // Arrange
        Node<string> node = new Node<string>("hello");

        // Act
        bool result = node.Exists("world");

        // Assert
        Assert.AreEqual<bool>(false, result);
    }

    [TestMethod]
    public void Exists_ValueIsInMultiNodeList_ReturnsTrue()
    {
        // Arrange
        Node<int> first = new(1);
        Node<int> second = first.Append(2);
        Node<int> third = second.Append(3);

        // Act
        bool result = first.Exists(3);

        // Assert
        Assert.AreEqual<bool>(true, result);
    }

    [TestMethod]
    public void Exists_ValueIsNotInMultiNodeList_ReturnsFalse()
    {
        // Arrange
        Node<int> first = new(1);
        Node<int> second = first.Append(2);
        Node<int> third = second.Append(3);

        // Act
        bool result = first.Exists(99);

        // Assert
        Assert.AreEqual<bool>(false, result);
    }

    [TestMethod]
    public void Exists_ValueIsNullInList_ReturnsTrue()
    {
        // Arrange
        Node<string> first = new("a");
        Node<string> second = first.Append(null);
        Node<string> third = second.Append("c");

        // Act
        bool result = first.Exists(null);

        // Assert
        Assert.AreEqual<bool>(true, result);
    }

    [TestMethod]
    public void Append_DuplicateValue_ThrowsInvalidOperationException()
    {
        // Arrange
        Node<string> node = new Node<string>("hello");

        // Act
        node.Append("world");

        // Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            node.Append("hello");
        });
    }

    [TestMethod]
    public void Clear_MultiNodeList_OnlyCurrentNodeRemains()
    {
        // Arrange
        Node<int> first = new(1);
        Node<int> second = first.Append(2);
        Node<int> third = second.Append(3);

        // Act
        first.Clear();

        // Assert
        Assert.AreEqual<Node<int>>(first, first.Next);
        Assert.IsFalse(first.Exists(2));
        Assert.IsFalse(first.Exists(3));
    }

    [TestMethod]
    public void Clear_RemovedNodesStillPointToSelf()
    {
        // Arrange
        Node<string> first = new Node<string>("a");
        Node<string> second = first.Append("b");
        Node<string> third = second.Append("c");

        // Act
        first.Clear();

        // Assert
        Assert.AreEqual<Node<string>>(second, second.Next);
        Assert.AreEqual<Node<string>>(third, third.Next);
    }
}
