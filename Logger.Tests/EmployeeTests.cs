using Xunit;

namespace Logger.Tests;

public class EmployeeTests
{
    [Fact]
    public void Constructor_Initializes_Properly()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("John", "Doe");
        // Act
        Employee employee = new(id, name);
        // Assert
        Assert.NotNull(employee);
        Assert.Equal(id, employee.Id);
        Assert.Equal(name, employee.FullName);
    }

    [Fact]
    public void Constructor_Throws_On_Null_FullName()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Employee(id, null!));
    }

    [Fact]
    public void NameProperty_FullName_ReturnsCorrectValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("Jane", "Smith", "A.");
        Employee employee = new(id, name);
        // Act
        var employeeName = employee.Name;
        // Assert
        Assert.Equal("Jane A. Smith", employeeName);
    }

    [Fact]
    public void NameProperty_NullMiddle_ReturnsCorrectValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("Jane", "Smith");
        Employee employee = new(id, name);
        // Act
        var employeeName = employee.Name;
        // Assert
        Assert.Equal("Jane Smith", employeeName);
    }

    [Fact]
    public void Employee_Equality_BehavesNormal()
    {
        // Arrange
        var id = Guid.NewGuid();
        FullName name = new("Alice", "Johnson");
        Employee employee1 = new(id, name);
        Employee employee2 = new(id, name);
        // Act & Assert
        Assert.Equal(employee1, employee2);
    }

    [Fact]
    public void Employee_Inequality_BehavesNormal()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        FullName name1 = new("Bob", "Brown");
        Employee employee1 = new(id1, name1);
        Employee employee2 = new(id2, name1);
        // Act & Assert
        Assert.NotEqual(employee1, employee2);
    }
}