using Xunit;

namespace Logger.Tests;

public class EntityBaseTests
{
    private class TestEntity : EntityBase
    {
        public override string Name { get; set; } = "TestName";
    }

    [Fact]
    public void EntityBase_DerivedClassMustImplementName_ProvidesConcreteValue()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        TestEntity entity = new() { Id = id };

        // Act
        string name = entity.Name;
        Guid actualId = entity.Id;

        // Assert
        Assert.Equal("TestName", name); 
        Assert.Equal(id, actualId);
    }


    [Fact]
    public void EntityBase_ImplicitIEntityImplementation_ExposesIdAndName()
    {
        // Arrange
        IEntity entity = new TestEntity { Id = Guid.NewGuid(), Name = "Polymorphic" };

        // Act
        string name = entity.Name;
        Guid id = entity.Id;

        // Assert
        Assert.Equal("Polymorphic", name);
        Assert.NotEqual(Guid.Empty, id);
    }
}