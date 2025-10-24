using System;
using System.Collections.Generic;
using Xunit;

namespace Logger.Tests;

public class StorageTests
{
    [Fact]
    public void Add_NewEntity_ContainsEntity()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");

        // Act
        storage.Add(book);

        // Assert
        Assert.True(storage.Contains(book));
    }

    [Fact]
    public void Add_SameEntityTwice_DoesNotDuplicate()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");

        // Act
        storage.Add(book);
        storage.Add(book);

        // Assert
        Assert.Equal(1, storage.Count);
    }

    [Fact]
    public void Remove_ExistingEntity_RemovesEntity()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");
        storage.Add(book);

        // Act
        storage.Remove(book);

        // Assert
        Assert.False(storage.Contains(book));
    }

    [Fact]
    public void Remove_NonExistingEntity_DoesNotThrow()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");

        // Act & Assert
        var exception = Record.Exception(() => storage.Remove(book));
        Assert.Null(exception);
    }

    [Fact]
    public void Get_ExistingEntityById_ReturnsEntity()
    {
        // Arrange
        Storage storage = new Storage();
        Guid id = Guid.NewGuid();
        Employee employee = new Employee(id, new FullName("Biscuit", "Gravy", "the"));
        storage.Add(employee);

        // Act
        IEntity? retrieved = storage.Get(id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(id, ((dynamic)retrieved).Id);
    }

    [Fact]
    public void Get_NonExistingEntityById_ReturnsNull()
    {
        // Arrange
        Storage storage = new Storage();
        Guid id = Guid.NewGuid();

        // Act
        IEntity? retrieved = storage.Get(id);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public void Storage_CanHandleMultipleEntityTypes()
    {
        // Arrange
        Storage storage = new Storage();
        Book book = new Book(Guid.NewGuid(), "The Assassins Apprentice");
        Student student = new Student(Guid.NewGuid(), new FullName("Boius", "Bohemian", "The Third"));
        Employee employee = new Employee(Guid.NewGuid(), new FullName("Lemon", "Beaubemon", "the"));

        // Act
        storage.Add(book);
        storage.Add(student);
        storage.Add(employee);

        // Assert
        Assert.True(storage.Contains(book));
        Assert.True(storage.Contains(student));
        Assert.True(storage.Contains(employee));
    }
}
