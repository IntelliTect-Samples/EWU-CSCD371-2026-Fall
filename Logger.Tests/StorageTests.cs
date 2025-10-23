using Xunit;

namespace Logger.Tests;

public class StorageTests
{
    [Fact]
    public void Contains_AddedEntity_ShouldReturnTrue()
    {
        // Arrange
        var storage = new Storage();
        Guid id = Guid.NewGuid();
        FullName name = new("Alice", "Johnson");
        Student student = new Student(id, name);
        // Act
        storage.Add(student);
        // Assert
        Assert.True(storage.Contains(student));
    }

    [Fact]
    public void Add_BookEntity_ShouldStoreEntity()
    {
        // Arrange
        var storage = new Storage();
        Guid id = Guid.NewGuid();
        Book book = new Book(id, "testTitle");
        // Act
        storage.Add(book);
        // Assert
        Assert.True(storage.Contains(book));
    }

    [Fact]
    public void Get_ExistingEntityById_ShouldReturnEntity()
    {
        // Arrange
        var storage = new Storage();
        Guid id = Guid.NewGuid();
        FullName name = new("Bob", "Smith");
        Employee employee = new Employee(id, name);
        storage.Add(employee);
        // Act
        var retrievedEntity = storage.Get(id);
        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal(employee, retrievedEntity);
    }

    [Fact]
    public void Get_NonExistingEntityById_ShouldReturnNull()
    {
        // Arrange
        var storage = new Storage();
        Guid nonExistingId = Guid.NewGuid();
        // Act
        var retrievedEntity = storage.Get(nonExistingId);
        // Assert
        Assert.Null(retrievedEntity);
    }

    [Fact]
    public void Remove_ExistingEntity_ShouldNoLongerContainEntity()
    {
        // Arrange
        var storage = new Storage();
        Guid id = Guid.NewGuid();
        FullName name = new("Charlie", "Brown");
        Student student = new Student(id, name);
        storage.Add(student);
        // Act
        storage.Remove(student);
        // Assert
        Assert.False(storage.Contains(student));
    }

    [Fact]
    public void Remove_NonExistingEntity_ShouldNotThrow()
    {
        // Arrange
        var storage = new Storage();
        Guid id = Guid.NewGuid();
        FullName name = new("Diana", "Prince");
        Student student = new Student(id, name);
        // Act & Assert
        var exception = Record.Exception(() => storage.Remove(student));
        Assert.Null(exception);
    }
}