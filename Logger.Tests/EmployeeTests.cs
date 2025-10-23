using System;
using Xunit;

namespace Logger.Tests;

public class EmployeeTests
{

    [Fact]
    public void Employee_Create_InitializesProperties()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var employee = new Employee { Id = id, FullName = new ("Alan", "Turing"), Department = "Research" };

        // Act
        var department = employee.Department;
        Guid employeeId = employee.Id;
        var name = employee.FullName;

        // Assert
        Assert.Equal("Research", department);
        Assert.Equal("Alan", name.First);
        Assert.Equal("Turing", name.Last);
        Assert.Equal("Turing, Alan", name.ToString());
        Assert.Equal(id, employeeId);
    }

    [Fact]
    public void Employee_NullDepartment_ReturnsNull()
    {
        // Arrange
        var employee = new Employee { FullName = new ("Uli", "Aguilar") };

        // Act
        var department = employee.Department;

        // Assert
        Assert.Null(department);
    }
}