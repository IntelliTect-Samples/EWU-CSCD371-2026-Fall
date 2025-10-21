using Xunit;

namespace Logger.Tests;

public class StorageTests
{
    private sealed record TestEntity : EntityBase
    {
        public override string Name { get; }

        public TestEntity(string name)
        {
            Name = name;
        }
    }

    [Fact]
    public void Contains_WhenEntityNotPresent_ReturnsFalse()
    {
        // Arrange
        Storage storage = new();
        TestEntity entity = new("DoesNotExist") { Id = Guid.NewGuid() };

        // Act & Assert
        Assert.False(storage.Contains(entity));
    }

    [Fact]
    public void Add_WithEntity_StorageContainsEntity()
    {
        //Arrange
        Storage storage = new();
        TestEntity entity = new("TestEntry") { Id = Guid.NewGuid() };
        Assert.False(storage.Contains(entity));
        // as part of arrange, we must explicitly ensure the entity does not exist before adding can be tested

        //Act
        storage.Add(entity);

        //Assert
        Assert.True(storage.Contains(entity));
    }

    [Fact]
    public void Remove_WithEntity_StorageNotContainEntity()
    {
        // Arrange
        Storage storage = new();
        TestEntity entity = new("TestEntry") { Id = Guid.NewGuid() };

        storage.Add(entity);
        Assert.True(storage.Contains(entity));
        // as part of arrange, we must explicitly ensure the entity exists before removal can be tested

        // Act
        storage.Remove(entity);

        // Assert
        Assert.False(storage.Contains(entity));
    }
}

