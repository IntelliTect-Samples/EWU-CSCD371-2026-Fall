using Xunit;
using System;
using Logger;

namespace Logger.Tests;

public record MockEntity : EntityBase
{
    public override string Name => "Mock Entity";
    public MockEntity() : base() { }
    public MockEntity(Guid id) : base(id) { }
}

public class EntityBaseTests
{
    [Fact]
    public void EntityBase_ImplementsIEntity_InterfaceMembersAccessible()
    {
        // Arrange
        var entity = new MockEntity();
        // Act & Assert
        Assert.IsAssignableFrom<IEntity>(entity);
        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.Equal("Mock Entity", entity.Name);
    }

    [Fact]
    public void EntityBase_WithExpression_CreatesNewRecordAndPreservesOriginal()
    {
        // Arrange
        var originalEntity = new MockEntity();
        var newId = Guid.NewGuid();

        // Act
        var modifiedEntity = originalEntity with { Id = newId };

        // Assert
        Assert.NotEqual(originalEntity.Id, modifiedEntity.Id);
        Assert.Equal(newId, modifiedEntity.Id);
        Assert.Equal(originalEntity.Name, modifiedEntity.Name);
    }

    [Fact]
    public void EntityBase_DefaultConstructor_GeneratesUniqueId()
    {
        // Arrange
        var entity1 = new MockEntity();
        var entity2 = new MockEntity();
        // Assert:
        Assert.NotEqual(entity1.Id, entity2.Id);
    }

    [Fact]
    public void EntityBase_ParameterizedConstructor_SetsExplicitId()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        // Act
        var entity = new MockEntity(expectedId);
        // Assert
        Assert.Equal(expectedId, entity.Id);
    }

    [Fact]
    public void EntityBase_Equality_ReturnsTrueForSameId()
    {
        // Arrange
        var sharedId = Guid.NewGuid();
        var entity1 = new MockEntity(sharedId);
        var entity2 = new MockEntity(sharedId);
        // Assert
        Assert.True(entity1.Equals(entity2));
        Assert.True(entity1 == entity2);
        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
    }

    [Fact]
    public void EntityBase_Equality_ReturnsFalseForDifferentIds()
    {
        // Arrange
        var entity1 = new MockEntity(Guid.NewGuid());
        var entity2 = new MockEntity(Guid.NewGuid());
        // Assert
        Assert.False(entity1.Equals(entity2));
        Assert.False(entity1 == entity2);
        Assert.NotEqual(entity1.GetHashCode(), entity2.GetHashCode());
    }
}