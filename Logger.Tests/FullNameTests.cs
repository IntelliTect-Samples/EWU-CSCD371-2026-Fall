using Xunit;
using System.Linq;

namespace Logger.Tests;

public class FullNameTests
{
    [Fact]
    public void FullName_ToString_ReturnsCorrectFormat_WithMiddleName()
    {
        // Arrange
        var first = "John";
        var middle = "A";
        var last = "Smith";
        var fullName = new Logger.FullName(first, middle, last);
        // Act
        var result = fullName.ToString();
        // Assert
        Assert.Equal("John A Smith", result);
    }
    [Fact]
    public void FullName_ToString_ReturnsCorrectFormat_WithoutMiddleName()
    {
        // Arrange
        var first = "Jane";
        string? middle = null;
        var last = "Doe";
        var fullName = new Logger.FullName(first, middle, last);
        // Act
        var result = fullName.ToString();
        // Assert
        Assert.Equal("Jane Doe", result);
    }
    [Fact]
    public void FullName_Equality_ReturnsTrueForSomeValues()
    {
        // Arrange
        var fullName1 = new Logger.FullName("Alice", "B", "Johnson");
        var fullName2 = new Logger.FullName("Alice", "B", "Johnson");
        // Act & Assert
        Assert.True(fullName1.Equals(fullName2));
        Assert.True(fullName1 == fullName2);
        Assert.Equal(fullName1.GetHashCode(), fullName2.GetHashCode());
    }

    [Fact]
    public void FullName_Equality_ReturnsFalseForDifferentValues()
    {
        // Arrange
        var fullName1 = new Logger.FullName("Charlie", null, "Brown");
        var fullName2 = new Logger.FullName("Charlie", "D", "Brown");
        // Act & Assert
        Assert.False(fullName1.Equals(fullName2));
        Assert.False(fullName1 == fullName2);
        Assert.NotEqual(fullName1.GetHashCode(), fullName2.GetHashCode());
    }

    [Fact]
    public void FullName_WithExpression_CreatesNewRecordAndPreservesOriginal()
    {
        var originalFirst = "Sam";
        var originalName = new FullName(originalFirst, "L", "Wilson");
        // Act
        var newFirstName = originalName with { First = "Henry" };
        // Assert
        Assert.Equal(originalFirst, originalName.First);
        Assert.NotSame(originalName, newFirstName);
        Assert.Equal("Henry", newFirstName.First);
    }

    [Fact]
    public void FullName_ToString_ReturnsCorrectFormat_WhenMiddleNameIsEmptyString()
    {
        // Arrange
        var fullName = new Logger.FullName("Mark", "", "Foster");
        // Act
        var result = fullName.ToString();
        // Assert
        Assert.Equal("Mark Foster", result);
    }

    [Fact]
    public void FullName_ToString_ReturnsCorrectFormat_WhenMiddleNameIsWhitespace()
    {
        // Arrange
        var fullName = new Logger.FullName("Anna", "   ", "Brown");
        // Act
        var result = fullName.ToString();
        // Assert
        Assert.Equal("Anna Brown", result);
    }

    [Fact]
    public void FullName_ToString_ReturnsCorrectFormat_WhenMiddleNameIsNull()
    {
        // Arrange
        var fullName = new Logger.FullName("Lucy", null, "Adams");
        // Act
        var result = fullName.ToString();
        // Assert
        Assert.Equal("Lucy Adams", result);
    }



    [Fact]
    public void FullName_Equality_TreatsNullAndEmptyMiddleAsUnEqual()
    {
        // Arrange
        var nameWithNull = new Logger.FullName("Case", null, "Study");
        var nameWithEmpty = new Logger.FullName("Case", "", "Study");

        // Assert
        Assert.False(nameWithNull.Equals(nameWithEmpty));
        Assert.False(nameWithNull == nameWithEmpty);
        Assert.NotEqual(nameWithNull.GetHashCode(), nameWithEmpty.GetHashCode());
    }
}
