using System;
using System.Collections.Generic;
using Xunit;

namespace Logger.Tests;

public class StudentTests
{
    [Fact]
    public void Constructor_ValidValues_SetsProperties()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        FullName fullName = new FullName("Boius", "Bohemian", "The Third");

        // Act
        Student student = new Student(expectedId, fullName);

        // Assert
        Assert.Equal(expectedId, student.Id);
        Assert.Equal(fullName, student.fullName);
    }

    [Fact]
    public void Name_ValidFullName_ReturnsFullNameString()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        FullName fullName = new FullName("Boius", "Bohemian", "The Third");
        Student student = new Student(id, fullName);

        // Act
        string name = student.Name;

        // Assert
        Assert.Equal("Boius The Third Bohemian", name);
    }

    [Fact]
    public void Equality_TwoStudentsWithSameIdAndFullName_AreEqual()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        FullName fullName = new FullName("Boius", "Bohemian", "The Third");
        Student student1 = new Student(id, fullName);
        Student student2 = new Student(id, fullName);

        // Act
        bool areEqual = student1.Equals(student2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equality_TwoStudentsWithDifferentIds_AreNotEqual()
    {
        // Arrange
        FullName fullName = new FullName("Boius", "Bohemian", "The Third");
        Student student1 = new Student(Guid.NewGuid(), fullName);
        Student student2 = new Student(Guid.NewGuid(), fullName);

        // Act
        bool areEqual = student1.Equals(student2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Student_ImplementsIEntity_ReturnsTrue()
    {
        // Arrange
        FullName fullName = new FullName("Boius", "Bohemian", "The Third");
        Student student = new Student(Guid.NewGuid(), fullName);

        // Act
        bool implementsInterface = student is IEntity;

        // Assert
        Assert.True(implementsInterface);
    }
}
