using Xunit;
using Logger;

namespace Logger.Tests;

public class EmployeeTests
{

    [Fact]
    public void Name_WithValidFullNameAndPosition_ReturnsFullNameWithPositionSuffix()
    {
        // Arrange
        Employee e = new()
        {
            FullName = new FullName("Michael", "Myers"),
            Position = "Janitor"
        };

        // Act
        string result = e.Name;

        // Assert
        Assert.Equal("Michael Myers, Janitor", result);
    }

    [Fact]
    public void Equals_SamePositionAndSameFullName_ReturnsTrue()
    {
        // Arrange
        Employee a = new() { FullName = new FullName("Laurie", "Strode"), Position = "Survivor" };
        Employee b = new() { FullName = new FullName("Laurie", "Strode"), Position = "Survivor" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.True(equal);
    }

    [Fact]
    public void Equals_DifferentPositionSameFullName_ReturnsFalse()
    {
        // Arrange
        Employee a = new() { FullName = new FullName("Freddy", "Krueger"), Position = "Custodian" };
        Employee b = new() { FullName = new FullName("Freddy", "Krueger"), Position = "Dream Stalker" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_SamePositionDifferentFullName_ReturnsFalse()
    {
        // Arrange
        Employee a = new() { FullName = new FullName("Billy", "Loomis"), Position = "Slasher" };
        Employee b = new() { FullName = new FullName("Stu", "Macher"), Position = "Slasher" };

        // Act
        bool equal = a.Equals(b);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void Equals_NullComparison_ReturnsFalse()
    {
        // Arrange
        Employee e = new() { FullName = new FullName("Ellen", "Ripley"), Position = "Warrant Officer" };

        // Act
        bool equal = e.Equals(null);

        // Assert
        Assert.False(equal);
    }

    [Fact]
    public void GetHashCode_EqualEmployees_ProduceSameHashCode()
    {
        // Arrange
        Employee a = new() { FullName = new FullName("Regan", "MacNeil"), Position = "Possessed" };
        Employee b = new() { FullName = new FullName("Regan", "MacNeil"), Position = "Possessed" };

        // Act
        int hashA = a.GetHashCode();
        int hashB = b.GetHashCode();

        // Assert
        Assert.Equal(hashA, hashB);
    }

    [Fact]
    public void GetHashCode_DifferentEmployees_ProduceDifferentHashCodes()
    {
        // Arrange
        Employee a = new() { FullName = new FullName("Samara", "Morgan"), Position = "Well Resident" };
        Employee b = new() { FullName = new FullName("Samara", "Morgan"), Position = "Caller" };

        // Act
        int hashA = a.GetHashCode();
        int hashB = b.GetHashCode();

        // Assert
        Assert.NotEqual(hashA, hashB);
    }


}
