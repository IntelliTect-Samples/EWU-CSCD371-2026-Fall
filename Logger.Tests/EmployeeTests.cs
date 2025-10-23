using Xunit;

namespace Logger.Tests;

public class EmployeeTests
{

    [Fact]
    public void Employee_PositionProperty_IsSetCorrectly()
    {
        // Arrange
        var fullName = new FullName("John", "H", "Doe");
        var position = "Software Engineer";
        var employee = new entities.Employee(fullName, position);
        // Act & Assert
        Assert.Equal(position, employee.Position);
    }

    [Fact]
    public void Employee_NameProperty_ReturnsFullName_WithMiddleName()
    {
        // Arrange
        var first = "Eric";
        var middle = "B";
        var last = "Williams";
        var fullName = new Logger.FullName(first, middle, last);
        var employee = new Logger.entities.Employee(fullName, "Director of Operations");
        // Assert
        Assert.Equal("Eric B Williams", employee.Name);
    }

    [Fact]
    public void Employee_NameProperty_ReturnsFullName_WhenMiddleNameIsNull()
    {
        // Arrange
        string? middle = null;
        var fullName = new Logger.FullName("Tara", middle, "Jones");
        var employee = new Logger.entities.Employee(fullName, "Marketing");
        // Assert
        Assert.Equal("Tara Jones", employee.Name);
    }

    [Fact]
    public void Employee_NameProperty_ReturnsFullName_WhenMiddleNameIsEmpty()
    {
        // Arrange
        var fullName = new Logger.FullName("Sam", "", "Lee");
        var employee = new Logger.entities.Employee(fullName, "IT");
        // Assert
        Assert.Equal("Sam Lee", employee.Name);
    }

    [Fact]
    public void Employee_Equality_ReturnsFalseForSameContentDifferentId()
    {
        // Arrange
        var fullName = new Logger.FullName("John", "G", "Jones");
        var position = "Shift Lead";
        var employee1 = new Logger.entities.Employee(fullName, position);
        var employee2 = new Logger.entities.Employee(fullName, position);
        // Assert
        Assert.NotEqual(employee1.Id, employee2.Id);
        Assert.False(employee1.Equals(employee2));
    }

    [Fact]
    public void Employee_Equality_ReturnsTrueForSameIdAndContent()
    {
        // Arrange
        var sharedId = Guid.NewGuid();
        var fullName = new Logger.FullName("Same", null, "Guy");
        var position = "Same Pos";
        // Act
        var employee1 = new Logger.entities.Employee(fullName, position) { Id = sharedId };
        var employee2 = new Logger.entities.Employee(fullName, position) { Id = sharedId };
        // Assert
        Assert.True(employee1.Equals(employee2));
        Assert.True(employee1 == employee2);
        Assert.Equal(employee1.GetHashCode(), employee2.GetHashCode());
    }

    [Fact]
    public void Employee_WithExpression_CreatesNewRecordWithModifiedPosition()
    {
        // Arrange
        var originalPosition = "Department Lead";
        var originalFullName = new Logger.FullName("Derrick", "M", "Smith");
        var originalEmployee = new Logger.entities.Employee(originalFullName, originalPosition);

        // Act
        var newPosition = "Department Manager";
        var newEmployee = originalEmployee with { Position = newPosition };

        // Assert
        Assert.Equal(originalPosition, originalEmployee.Position);
        Assert.Equal(newPosition, newEmployee.Position);
        Assert.NotSame(originalEmployee, newEmployee);
    }

    [Fact]
    public void Employee_Constructor_ThrowsException_WhenPositionIsNullOrEmpty()
    {
        // Arrange
        var fullName = new Logger.FullName("Linda", "K", "Brown");
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Logger.entities.Employee(fullName, null!));
        Assert.Throws<ArgumentException>(() => new Logger.entities.Employee(fullName, ""));
        Assert.Throws<ArgumentException>(() => new Logger.entities.Employee(fullName, "   "));
    }
}
