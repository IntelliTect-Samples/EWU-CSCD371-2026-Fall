using Xunit;

namespace Logger.Tests;

public class FullNameTests
{
    [Fact]
    public void FullName_ConstructorWithAllFields_SetsValuesCorrectly()
    {
        // Arrange
        string first = "John";
        string last = "Doe";
        string middle = "Michael";
        FullName name = new(first, last, middle);

        // Act
        string actualFirst = name.First;
        string actualLast = name.Last;
        string actualMiddle = name.Middle!;

        // Assert
        Assert.Equal(first, actualFirst);
        Assert.Equal(last, actualLast);
        Assert.Equal(middle, actualMiddle);
    }

    [Fact]
    public void FullName_Constructor_HandlesOptionalMiddle()
    {
        // Arrange
        string first = "Jane";
        string last = "Smith";
        FullName name = new(first, last, null);

        // Act
        string actualFirst = name.First;
        string actualLast = name.Last;
        string? actualMiddle = name.Middle;

        // Assert
        Assert.Equal(first, actualFirst);
        Assert.Equal(last, actualLast);
        Assert.Null(actualMiddle);
    }

    [Fact]
    public void FullName_EqualityComparison_ReturnsTrueForSameValues()
    {
        // Arrange
        FullName name1 = new("Alice", "Jones", "Marie");
        FullName name2 = new("Alice", "Jones", "Marie");

        // Act & Assert
        Assert.Equal(name1, name2);
    }

    [Fact]
    public void FullName_WithExpression_CreatesRenamedCopy()
    {
        FullName original = new("John", "Doe", "Michael");
        FullName renamed = original with { First = "Jane" };

        Assert.Equal("Jane", renamed.First);
        Assert.Equal("Doe", renamed.Last);
        Assert.Equal("Michael", renamed.Middle);
        Assert.Equal("John", original.First);
    }
}
