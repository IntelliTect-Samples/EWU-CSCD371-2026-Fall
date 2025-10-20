using Xunit;

namespace Logger.Tests;

public class EntityBaseTests
{
    private sealed class TestEntity(string name) : EntityBase
    {
        public override string Name { get; } = name;
    }

    [Fact]
    public void EntityBase_DerivedClassMustImplementName_ProvidesConcreteValue()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity = new("TestName") { Id = id };

        // Act
        string name = entity.Name;
        Guid actualId = entity.Id;

        // Assert
        Assert.Equal("TestName", name);
        Assert.Equal(id, actualId);
    }
}