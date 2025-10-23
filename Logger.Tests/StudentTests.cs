using Xunit;

namespace Logger.Tests;

public class StudentTests
{
    [Fact]
    public void Student_GraduationYearProperty_IsSetCorrectly()
    {
        // Arrange
        var fullName = new FullName("Alice", "B", "Johnson");
        var graduationYear = "2025";
        var student = new entities.Student(fullName, graduationYear);
        // Act & Assert
        Assert.Equal(graduationYear, student.GraduationYear);
    }

    [Fact]
    public void Student_NameProperty_ReturnsCorrectFullName()
    {
        // Arrange
        var fullName = new FullName("Bob", "C", "Williams");
        var graduationYear = "2024";
        var student = new entities.Student(fullName, graduationYear);
        // Act
        var name = student.Name;
        // Assert
        Assert.Equal("Bob C Williams", name);
    }

    [Fact]
    public void Student_NamePropterty_ReturnsFullName_WhenMiddleNameIsNull()
    {
        // Arrange
        string? middle = null;
        var fullName = new FullName("Charlie", middle, "Brown");
        var student = new entities.Student(fullName, "2023");
        // Act
        var name = student.Name;
        // Assert
        Assert.Equal("Charlie Brown", name);
    }

    [Fact]
    public void Student_NameProperty_ReturnsFullName_WhenMiddleNameIsEmpty()
    {
        // Arrange
        var fullName = new FullName("Eva", "", "Green");
        var student = new entities.Student(fullName, "2022");
        // Act
        var name = student.Name;
        // Assert
        Assert.Equal("Eva Green", name);
    }

    [Fact]
    public void Student_Equality_ReturnsFalseForSameContentDifferentId()
    {
        // Arrange
        var fullName = new FullName("Diana", "D", "Evans");
        var graduationYear = "2026";
        var student1 = new entities.Student(fullName, graduationYear);
        var student2 = new entities.Student(fullName, graduationYear);
        // Act & Assert
        Assert.False(student1 == student2);
    }

    [Fact]
    public void Student_Equality_ReturnsTrueForSameIdAndContent()
    {
        // Arrange
        var sharedId = Guid.NewGuid();
        var fullName = new FullName("Shared", null, "Name");
        var year = "2021";

        // Act
        var student1 = new entities.Student(fullName, year) { Id = sharedId };
        var student2 = new entities.Student(fullName, year) { Id = sharedId };

        // Assert
        Assert.True(student1.Equals(student2));
        Assert.True(student1 == student2);
        Assert.Equal(student1.GetHashCode(), student2.GetHashCode());
    }

    [Fact]
    public void Student_WithExpression_CreatesNewRecordWithModifiedGraduationYear()
    {
        // Arrange
        var originalYear = "2023";
        var originalFullName = new FullName("Frank", null, "Harris");
        var originalStudent = new entities.Student(originalFullName, originalYear);

        // Act
        var updatedYear = "2024";
        var updatedStudent = originalStudent with { GraduationYear = updatedYear };
        // Assert
        Assert.Equal(originalYear, originalStudent.GraduationYear);
        Assert.Equal(updatedYear, updatedStudent.GraduationYear);
        Assert.NotSame(originalStudent, updatedStudent);
    }

    [Fact]
    public void Student_Constructor_ThrowsException_WhenGraduationYearIsNullOrEmpty()
    {
        // Arrange
        var fullName = new FullName("Grace", "I", "Johnson");
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new entities.Student(fullName, null!));
        Assert.Throws<ArgumentException>(() => new entities.Student(fullName, ""));
        Assert.Throws<ArgumentException>(() => new entities.Student(fullName, "   "));
    }
}
