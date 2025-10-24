using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Logger.Tests;
public class StorageTests
{
    // Helpers
    private static Student MakeStudent(string idSuffix = "001") =>
        new Student
        {
            FullName = new FullName("John", "Doe"),
            StudentId = $"S{idSuffix}"
        };

    private static Employee MakeEmployee(string position = "Engineer") =>
        new Employee
        {
            FullName = new FullName("Jane", "Doe"),
            Position = position
        };

    private static Book MakeBook(string title = "How to Code: 101") =>
        new Book
        {
            Title = title
        };

    [Fact]
    public void Add_WithNewEntity_Success()
    {
        // Arrange
        Storage storage = new Storage();
        Student student = MakeStudent();
        // Act
        storage.Add(student);
        // Assert
        Assert.True(storage.Contains(student));
    }

    [Fact]
    public void Remove_WithExistingEntity_Success()
    {
        // Arrange
        Storage storage = new Storage();
        Employee employee = MakeEmployee();
        storage.Add(employee);
        // Act
        storage.Remove(employee);
        // Assert
        Assert.False(storage.Contains(employee));
    }

    [Fact]
    public void Contains_WithEntityNotAdded_ReturnsFalse()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = MakeBook();
        // Act
        bool contains = storage.Contains(book);
        // Assert
        Assert.False(contains);
    }

    [Fact]
    public void Get_WithExistingEntity_ReturnsMatchingEntity()
    {
        // Arrange
        Storage storage = new Storage();
        Student student = MakeStudent();
        storage.Add(student);
        Guid existingId = ((IEntity)student).Id;

        // Act
        IEntity? result = storage.Get(existingId);

        // Assert
        Assert.Same(student, result);
    }

    [Fact]
    public void Get_WithUnknownId_ReturnsNull()
    {
        // Arrange
        Storage storage = new Storage();
        storage.Add(MakeStudent());
        storage.Add(MakeEmployee());
        storage.Add(MakeBook());
        Guid unknownGuid = Guid.NewGuid();

        // Act
        IEntity? result = storage.Get(unknownGuid);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Add_SameEntityTwice_StorageStillContainsEntity()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = MakeBook();

        // Act
        storage.Add(book);
        storage.Add(book);

        // Assert
        Assert.True(storage.Contains(book));
    }

}