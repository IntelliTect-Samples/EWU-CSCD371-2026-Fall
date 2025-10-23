using Xunit;

namespace Logger.Tests;

public class StudentTests
{
    [Fact]
    public void Constructor_Initializes_Properly()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("John", "Doe");
        // Act
        Student student = new(id, name);
        // Assert
        Assert.NotNull(student);
        Assert.Equal(id, student.Id);
        Assert.Equal(name, student.FullName);
    }

    [Fact]
    public void Constructor_Throws_On_Null_FullName()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Student(id, null!));
    }

    [Fact]
    public void NameProperty_FullName_ReturnsCorrectValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("Jane", "Smith", "A.");
        Student student = new(id, name);
        // Act
        var studentName = student.Name;
        // Assert
        Assert.Equal("Jane A. Smith", studentName);
    }

    [Fact]
    public void NameProperty_NullMiddle_ReturnsCorrectValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("Jane", "Smith");
        Student student = new(id, name);
        // Act
        var studentName = student.Name;
        // Assert
        Assert.Equal("Jane Smith", studentName);
    }

    [Fact]
    public void Student_Equality_BehavesNormal()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("Alice", "Johnson");
        Student student1 = new(id, name);
        Student student2 = new(id, name);
        // Act & Assert
        Assert.Equal(student1, student2);
    }

    [Fact]
    public void Student_Inequality_BehavesNormal()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        FullName name1 = new("Bob", "Brown");
        Student student1 = new(id1, name1);
        Student student2 = new(id2, name1);
        // Act & Assert
        // Really just testing that inequality works with different IDs since students can potentially have the same name.
        Assert.NotEqual(student1, student2);
    }
}