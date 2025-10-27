using GenericsHomework;
using Xunit;

namespace GenericHomework.Tests
{
    public class NodeTests
    {
        [Fact]
        public void Node_ToString_ReturnsValueString()
        {
            // Arrange
            var node = new Node<int>(42);
            // Act
            var result = node.ToString();
            // Assert
            Assert.Equal("42", result);
        }


        [Fact]
        public void Node_AppendValueAlreadyExists_ThrowsException()
        {
            // Arrange
            var node = new Node<int>(42);
            node.Append(43);
            // Act & Assert
            Assert.Throws<ArgumentException>(() => node.Append(42));

        }
    }
}