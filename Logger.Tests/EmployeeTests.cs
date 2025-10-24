using System;
using System.Collections.Generic;
using Xunit;

namespace Logger.Tests;

public class EmployeeTests
{
    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        FullName fullName = new FullName("Biscuit", "Gravy", "the");

        // Act
        Employee employee = new Employee(expectedId, fullName);

        // Assert
        Assert.Equal(expectedId, employee.Id);
        Assert.Equal(fullName, employee.fullName);
    }

    [Fact]
    public void Name_ValidFullName_ReturnsFullNameString()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        FullName fullName = new FullName("Lemon", "Beaubemon", "the");
        Employee employee = new Employee(id, fullName);

        // Act
        string name = employee.Name;

        // Assert
        Assert.Equal("Lemon the Beaubemon", name);
    }

    [Fact]
    public void Equality_TwoEmployeesWithSameIdAndFullName_AreEqual()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        FullName fullName = new FullName("Biscuit", "Gravy", "the");
        Employee employee1 = new Employee(id, fullName);
        Employee employee2 = new Employee(id, fullName);

        // Act
        bool areEqual = employee1.Equals(employee2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equality_TwoEmployeesWithDifferentIds_AreNotEqual()
    {
        // Arrange
        FullName fullName = new FullName("Lemon", "Beaubemon", "the");
        Employee employee1 = new Employee(Guid.NewGuid(), fullName);
        Employee employee2 = new Employee(Guid.NewGuid(), fullName);

        // Act
        bool areEqual = employee1.Equals(employee2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Employee_ImplementsIEntity_ReturnsTrue()
    {
        // Arrange
        FullName fullName = new FullName("Biscuit", "Gravy", "the");
        Employee employee = new Employee(Guid.NewGuid(), fullName);

        // Act
        bool implementsInterface = employee is IEntity;

        // Assert
        Assert.True(implementsInterface);
    }
}
