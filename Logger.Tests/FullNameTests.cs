using Xunit;
using Logger;

namespace Logger.Tests;

public class FullNameTests
{
    [Fact]
    public void ToString_NoMiddleNameParameter_ReturnsFirstAndLast()
    {
        // Arrange
        FullName name = new(FirstName:"Ada",LastName: "Lovelace"); 

        // Act
        string result = name.ToString();

        // Assert
        Assert.Equal("Ada Lovelace", result);
    }

    [Fact]
    public void ToString_EmptyMiddleName_ReturnsFirstAndLast()
    {
        // Arrange
        FullName name = new("Alan", "Turing", "");

        // Act
        string result = name.ToString();

        // Assert
        Assert.Equal("Alan Turing", result);
    }

    [Fact]
    public void ToString_WhitespaceMiddleName_ReturnsFirstAndLast()
    {
        // Arrange
        FullName name = new("Linus", "Torvalds", "   ");

        // Act
        string result = name.ToString();

        // Assert
        Assert.Equal("Linus Torvalds", result);
    }

    [Fact]
    public void ToString_WithMiddleName_ReturnsFirstMiddleLast()
    {
        // Arrange
        FullName name = new("Grace", "Hopper", "Brewster");

        // Act
        string result = name.ToString();

        // Assert
        Assert.Equal("Grace Brewster Hopper", result);
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        // Arrange
        FullName a = new("Tim", "Berners-Lee", "John");
        FullName b = new("Tim", "Berners-Lee", "John");

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.True(equal);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void WithExpression_UpdateMiddleName_ProducesNewInstanceWithUpdatedName()
    {
        // Arrange
        FullName original = new("Katherine", "Johnson");
        // Act
        FullName updated = original with { MiddleName = "Coleman" };

        // Assert
        Assert.Equal("Katherine Johnson", original.ToString());          
        Assert.Equal("Katherine Coleman Johnson", updated.ToString());   
        Assert.NotEqual(original, updated);
    }

}
