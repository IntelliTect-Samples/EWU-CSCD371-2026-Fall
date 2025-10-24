using Xunit;
using Logger;

namespace Logger.Tests;

public class BookTests
{

    [Fact]
    public void Name_WithTitle_ReturnsTitle()
    {
        // Arrange
        Book book = new() { Title = "The Hobbit" };

        // Act
        string name = book.Name;

        // Assert
        Assert.Equal("The Hobbit", name);
    }

    [Fact]
    public void Equals_SameTitle_ReturnsTrue()
    {
        // Arrange
        Book a = new() { Title = "Dune" };
        Book b = new() { Title = "Dune" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_DifferentTitle_ReturnsFalse()
    {
        // Arrange
        Book a = new() { Title = "1984" };
        Book b = new() { Title = "Brave New World" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_NullComparison_ReturnsFalse()
    {
        // Arrange
        Book a = new() { Title = "The Name of the Wind" };

        // Act
        bool equal = a.Equals(null);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_SameLettersDifferentCasing_ReturnsFalse()
    {
        // Arrange
        Book a = new() { Title = "Frankenstein" };
        Book b = new() { Title = "frankenstein" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.False(equal); // equality is case-sensitive by Title ==
    }

    [Fact]
    public void GetHashCode_EqualBooks_ProduceSameHashCode()
    {
        // Arrange
        Book a = new() { Title = "The Lord of the Rings" };
        Book b = new() { Title = "The Lord of the Rings" };

        // Act
        int hashA = a.GetHashCode();
        int hashB = b.GetHashCode();

        // Assert
        Assert.Equal(hashA, hashB);
    }

    [Fact]
    public void GetHashCode_DifferentTitles_ProduceDifferentHashCodes()
    {
        // Arrange
        Book a = new() { Title = "The Shining" };
        Book b = new() { Title = "Doctor Sleep" };

        // Act
        int hashA = a.GetHashCode();
        int hashB = b.GetHashCode();

        // Assert
        Assert.NotEqual(hashA, hashB);
    }

}
