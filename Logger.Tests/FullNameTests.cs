using System;
using System.Collections.Generic;
using Xunit;

namespace Logger.Tests;

public class FullNameTests
{
    [Fact]
    public void Constructor_WithValidInputs_SetsProperties()
    {
        //Arrange
        string firstName = "John";
        string middleName = "Doe";
        string lastName = "Smith";
        
        //Act
        FullName testName = new FullName(firstName, lastName, middleName);

        //Assert
        Assert.Equal(firstName, testName.First);
        Assert.Equal(middleName, testName.Middle);
        Assert.Equal(lastName, testName.Last);
    }
    [Fact]
    public void Constructor_MiddleWithSurroundingSpaces_TrimsMiddleName()
    {
        // Arrange
        var middle = "  Paul ";

        // Act
        var name = new FullName("John", "Doe", middle);

        // Assert
        Assert.Equal("Paul", name.Middle);
    }
    [Fact]
    public void Constructor_NullOrWhitespaceFirst_ThrowsArgumentException()
    {
        // Arrange
        string? nullFirst = null;
        string whitespaceFirst = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FullName(nullFirst!, "Doe", null));
        Assert.Throws<ArgumentException>(() => new FullName(whitespaceFirst, "Doe", null));
    }
    [Fact]
    public void Constructor_NullOrWhitespaceLast_ThrowsArgumentException()
    {
        // Arrange
        string? nullLast = null;
        string whitespaceLast = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FullName("John", nullLast!, null));
        Assert.Throws<ArgumentException>(() => new FullName("John", whitespaceLast, null));
    }
    [Fact]
    public void Constructor_NullOrWhitespaceMiddle_DefaultsToEmptyString()
    {
        // Arrange
        string? nullMiddle = null;
        string whitespaceMiddle = "   ";

        // Act
        var name1 = new FullName("John", "Doe", nullMiddle);
        var name2 = new FullName("John", "Doe", whitespaceMiddle);

        // Assert
        Assert.Equal(string.Empty, name1.Middle);
        Assert.Equal(string.Empty, name2.Middle);
    }
    [Fact]
    public void WithExpression_ModifiesCopy_DoesNotAffectOriginal()
    {
        // Arrange
        FullName original = new FullName("John", "Doe", "Paul");

        // Act
        FullName copy = original with { First = "Jane" };

        // Assert
        Assert.NotSame(original, copy);
        Assert.Equal("John", original.First);
        Assert.Equal("Jane", copy.First);
    }

    [Fact]
    public void GetHashCode_SameObject_StableHashCode()
    {
        // Arrange
        FullName name = new FullName("Alice", "Smith", "R");

        // Act
        int hash1 = name.GetHashCode();
        int hash2 = name.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }
    [Fact]
    public void Equals_ObjectsWithSameValues_ReturnsTrue()
    {
        // Arrange
        FullName name1 = new FullName("John", "Doe", "Paul");
        FullName name2 = new FullName("John", "Doe", "Paul");

        // Act
        bool areEqual = name1 == name2;
        bool areEqualObject = name1.Equals(name2);

        // Assert
        Assert.True(areEqual);
        Assert.True(areEqualObject);
    }

    [Fact]
    public void Equals_ObjectsWithDifferentValues_ReturnsFalse()
    {
        // Arrange
        FullName name1 = new FullName("John", "Doe", "Paul");
        FullName name2 = new FullName("Jane", "Doe", "Paul");

        // Act
        bool areEqual = name1 == name2;
        bool areEqualObject = name1.Equals(name2);

        // Assert
        Assert.False(areEqual);
        Assert.False(areEqualObject);
    }

    [Fact]
    public void Equals_ObjectsWithSameValues_BehaveCorrectlyInHashSet()
    {
        // Arrange
        HashSet<FullName> set = new HashSet<FullName>();

        // Act
        set.Add(new FullName("John", "Doe", "Paul"));
        set.Add(new FullName("John", "Doe", "Paul")); // duplicate

        // Assert
        Assert.Single(set);
    }
}
