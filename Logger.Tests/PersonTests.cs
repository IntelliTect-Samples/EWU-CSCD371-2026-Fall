using Xunit;

namespace Logger.Tests;

public class PersonTests
{
    [Fact]
    public void Person_Constructor_SetsFullName()
    {
        // Arrange
        var fullName = new FullName("Alice", "B", "Johnson");

        // Act
        var person = new TestPerson(fullName);

        // Assert
        Assert.Equal(fullName, person.FullName);
    }

    [Fact]
    public void Person_NameProperty_ReturnsCorrectFullName()
    {
        // Arrange
        var fullName = new FullName("Bob", "C", "Williams");
        var person = new TestPerson(fullName);
        // Act
        var name = person.Name;
        // Assert
        Assert.Equal("Bob C Williams", name);
    }

    [Fact]
    public void Person_ToString_IncludesFullName()
    {
        // Arrange
        var fullName = new FullName("Diana", "E", "Prince");

        // Act
        var person = new TestPerson(fullName);
        var str = person.ToString();

        // Assert
        Assert.Contains("Diana", str);
        Assert.Contains("Prince", str);
    }

    [Fact]
    public void Person_Id_IsGeneratedByDefault()
    {
        // Arrange & Act
        var person = new TestPerson(new FullName("Charlie", "D", "Brown"));

        // Assert
        Assert.NotEqual(Guid.Empty, person.Id);
    }


    [Fact]
    public void Person_EqualityOnNameAndId_ReturnsTrue()
    {
        // Arrange
        var name1 = new FullName("Alice", "B.", "Cooper");
        var name2 = new FullName("Alice", "B.", "Cooper");
        var id = Guid.NewGuid();

        // Act
        var person1 = new TestPerson(name1, id);
        var person2 = new TestPerson(name2, id);

        // Assert
        Assert.Equal(person1, person2);
        Assert.Equal(person1.GetHashCode(), person2.GetHashCode());
    }

    [Fact]
    public void Person_Equality_ReturnsFalseForDifferentValues()
    {
        // Arrange
        var name1 = new FullName("Alice", "B.", "Cooper");
        var name2 = new FullName("Bob", "B.", "Cooper");

        // Act
        var person1 = new TestPerson(name1);
        var person2 = new TestPerson(name2);

        // Assert
        Assert.NotEqual(person1, person2);
        Assert.NotEqual(person1.GetHashCode(), person2.GetHashCode());
    }

    [Fact]
    public void Person_Equality_FailsWithDifferentId()
    {
        // Arrange
        var fullName = new FullName("Alice", "B", "Cooper");
        
        // Act
        var p1 = new TestPerson(fullName, Guid.NewGuid());
        var p2 = new TestPerson(fullName, Guid.NewGuid());

        // Assert
        Assert.NotEqual(p1, p2);
    }

    private sealed record class TestPerson : Person
    {
        public TestPerson(FullName fullName) : base(fullName) { }
        public TestPerson(FullName fullName, Guid id) : base(fullName)
        {
            Id = id;
        }
    }
}
