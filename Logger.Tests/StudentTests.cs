using Xunit;
using Logger;

namespace Logger.Tests;

public class StudentTests
{

    [Fact]
    public void Name_WithValidFullNameAndId_ReturnsFullNameWithIdSuffix()
    {
        // Arrange
        Student s = new()
        {
            FullName = new FullName("Luke", "Skywalker"), 
            StudentId = "JEDI-77"
        };

        // Act
        string result = s.Name;

        // Assert
        Assert.Equal("Luke Skywalker (ID: JEDI-77)", result);
    }

    [Fact]
    public void Equals_SameIdAndSameFullName_ReturnsTrue()
    {
        // Arrange
        Student a = new() { FullName = new FullName("Tony", "Stark", "Edward"), StudentId = "IRON-42" };
        Student b = new() { FullName = new FullName("Tony", "Stark", "Edward"), StudentId = "IRON-42" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_DifferentIdSameFullName_ReturnsFalse()
    {
        // Arrange
        Student a = new() { FullName = new FullName("Peter", "Parker"), StudentId = "SPDR-001" };
        Student b = new() { FullName = new FullName("Peter", "Parker"), StudentId = "SPDR-002" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_SameIdDifferentFullName_ReturnsFalse()
    {
        // Arrange
        Student a = new() { FullName = new FullName("Bruce", "Wayne"), StudentId = "BAT-1" };
        Student b = new() { FullName = new FullName("Bruce", "Banner"), StudentId = "BAT-1" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_NullComparison_ReturnsFalse()
    {
        // Arrange
        Student a = new() { FullName = new FullName("Hermione", "Granger"), StudentId = "HOG-3" };

        // Act
        bool equal = a.Equals(null);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void GetHashCode_EqualStudents_ProduceSameHashCode()
    {
        // Arrange
        Student a = new() { FullName = new FullName("James", "Bond", "Herbert"), StudentId = "007" };
        Student b = new() { FullName = new FullName("James", "Bond", "Herbert"), StudentId = "007" };

        // Act
        int hashA = a.GetHashCode();
        int hashB = b.GetHashCode();

        // Assert
        Assert.Equal(hashA, hashB);
    }

    [Fact]
    public void GetHashCode_DifferentStudents_ProduceDifferentHashCodes()
    {
        // Arrange
        Student a = new() { FullName = new FullName("Leia", "Organa"), StudentId = "REB-1" };
        Student b = new() { FullName = new FullName("Leia", "Organa"), StudentId = "REB-2" };

        // Act
        int hashA = a.GetHashCode();
        int hashB = b.GetHashCode();

        // Assert
        Assert.NotEqual(hashA, hashB);
    }

}
