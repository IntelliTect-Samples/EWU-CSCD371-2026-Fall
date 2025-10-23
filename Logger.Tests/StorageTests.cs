using Logger.entities;
using System;
using Xunit;

namespace Logger.Tests;

public class TestEntity : IEntity
{
    public Guid Id { get; } = Guid.NewGuid();
    Guid IEntity.Id { get => Id; init => throw new NotImplementedException(); }
    string IEntity.Name => throw new NotImplementedException();
}

public class StorageTests
{
    private static Storage CreateStorageWithEntity(IEntity entity)
    {
        var storage = new Storage();
        storage.Add(entity);
        return storage;
    }

    [Theory]
    [InlineData("Book")]
    [InlineData("Employee")]
    [InlineData("Student")]
    public void Add_Entity_ShouldBeContained(string entityType)
    {
        // Arrange
        IEntity entity = entityType switch
        {
            "Book" => new Book("1984", "George Orwell"),
            "Employee" => new Employee(new FullName("Alice", "B", "Johnson"), "Engineer"),
            "Student" => new Student(new FullName("Bob", "C", "Smith"), "2026"),
            _ => throw new ArgumentException("Unknown entity type")
        };
        var storage = new Storage();

        // Act
        storage.Add(entity);

        // Assert
        Assert.True(storage.Contains(entity));
    }

    [Theory]
    [InlineData("Book")]
    [InlineData("Employee")]
    [InlineData("Student")]
    public void Remove_Entity_ShouldNotBeContained(string entityType)
    {
        // Arrange
        IEntity entity = entityType switch
        {
            "Book" => new Book("1984", "George Orwell"),
            "Employee" => new Employee(new FullName("Alice", "B", "Johnson"), "Engineer"),
            "Student" => new Student(new FullName("Bob", "C", "Smith"), "2026"),
            _ => throw new ArgumentException("Unknown entity type")
        };
        var storage = CreateStorageWithEntity(entity);

        // Act
        storage.Remove(entity);

        // Assert
        Assert.False(storage.Contains(entity));
    }

    [Theory]
    [InlineData("Book")]
    [InlineData("Employee")]
    [InlineData("Student")]
    public void Get_EntityById_ShouldReturnEntity(string entityType)
    {
        // Arrange
        IEntity entity = entityType switch
        {
            "Book" => new Book("1984", "George Orwell"),
            "Employee" => new Employee(new FullName("Alice", "B", "Johnson"), "Engineer"),
            "Student" => new Student(new FullName("Bob", "C", "Smith"), "2026"),
            _ => throw new ArgumentException("Unknown entity type")
        };
        var storage = CreateStorageWithEntity(entity);

        // Act
        var retrieved = storage.Get(entity.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(entity, retrieved);
        Assert.Equal(entity.Name, ((dynamic)retrieved).Name);
    }

    [Fact]
    public void Get_NonExistingId_ShouldReturnNull()
    {
        // Arrange
        var storage = new Storage();

        // Act
        var retrieved = storage.Get(Guid.NewGuid());

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public void Add_MultipleEntities_ShouldContainAll()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book("1984", "George Orwell");
        var employee = new Employee(new FullName("Alice", "B", "Johnson"), "Engineer");
        var student = new Student(new FullName("Bob", "C", "Smith"), "2026");

        // Act
        storage.Add(book);
        storage.Add(employee);
        storage.Add(student);

        // Assert
        Assert.True(storage.Contains(book));
        Assert.True(storage.Contains(employee));
        Assert.True(storage.Contains(student));
    }

    [Fact]
    public void Remove_NonExistingEntity_ShouldNotThrow()
    {
        // Arrange
        var storage = new Storage();
        var book = new Book("1984", "George Orwell"); 

        // Act & Assert
        var exception = Record.Exception(() => storage.Remove(book));

        Assert.Null(exception); 
        Assert.False(storage.Contains(book)); 
    }
}
