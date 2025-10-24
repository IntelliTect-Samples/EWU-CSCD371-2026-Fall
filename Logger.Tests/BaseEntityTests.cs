using System;
using System.Collections.Generic;
using Xunit;

namespace Logger.Tests;

public class BaseEntityTests
{
    [Fact]
    public void Constructor_ValidGuid_SetsId()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();

        // Act
        TestEntity entity = new TestEntity(expectedId, "Test");

        // Assert
        Assert.Equal(expectedId, entity.Id);
    }

    [Fact]
    public void Id_IsImmutable_AfterConstruction()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity = new TestEntity(id, "Test");

        // Act
        Guid actualId = entity.Id;

        // Assert
        Assert.Equal(id, actualId);
    }

    [Fact]
    public void Name_IsImplementedByDerivedClass_ReturnsCorrectValue()
    {
        // Arrange
        string expectedName = "Test Entity";
        Guid id = Guid.NewGuid();

        // Act
        TestEntity entity = new TestEntity(id, expectedName);

        // Assert
        Assert.Equal(expectedName, entity.Name);
    }

    [Fact]
    public void Equals_TwoEntitiesWithSameIdAndName_ReturnsTrue()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity1 = new TestEntity(id, "Same");
        TestEntity entity2 = new TestEntity(id, "Same");

        // Act
        bool areEqual = entity1.Equals(entity2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equals_TwoEntitiesWithDifferentId_ReturnsFalse()
    {
        // Arrange
        TestEntity entity1 = new TestEntity(Guid.NewGuid(), "A");
        TestEntity entity2 = new TestEntity(Guid.NewGuid(), "A");

        // Act
        bool areEqual = entity1.Equals(entity2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetHashCode_TwoEntitiesWithSameData_AreEqual()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity1 = new TestEntity(id, "Same");
        TestEntity entity2 = new TestEntity(id, "Same");

        // Act
        int hash1 = entity1.GetHashCode();
        int hash2 = entity2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void BaseEntity_ImplementsIEntity_ReturnsTrue()
    {
        // Arrange
        TestEntity entity = new TestEntity(Guid.NewGuid(), "Test");

        // Act
        bool implementsInterface = entity is IEntity;

        // Assert
        Assert.True(implementsInterface);
    }
    private sealed record class TestEntity(Guid Id, string CustomName) : BaseEntity(Id)
    {
        public override string Name { get; } = CustomName;
    }
}
