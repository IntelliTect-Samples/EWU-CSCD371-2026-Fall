using Xunit;

namespace Logger.Tests;

public class StorageTests
{
    private sealed class TestEntity(string name) : EntityBase
    {
        public override string Name { get; } = name;
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

    [Fact]
    public void Contains_EntitiesEqualByValueNotByReference_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        var storage = new Storage();
        TestEntity entity1 = new("Clone") { Id = id };
        TestEntity entity2 = new("Clone") { Id = id };

        storage.Add(entity1);

        // Act & Assert
        Assert.False(storage.Contains(entity2));
    }

    [Fact]
    public void Contains_EntitiesEqualByReference_ReturnsTrue()
    {
        // Arrange
        var storage = new Storage();
        TestEntity entity1 = new("SameReference") { Id = Guid.NewGuid() };
        TestEntity entity2 = entity1;

        storage.Add(entity1);

        // Act & Assert
        Assert.True(storage.Contains(entity2));
    }
}

