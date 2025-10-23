using System;
using Xunit;

namespace Logger.Tests;

public class IEntityTests
{
    [Fact]
    public void IEntity_ValidParams_CreatesEntity()
    {
        Guid id = Guid.NewGuid();
        IEntity impl = new TestEntity { Id = id, Name = "X" };

        Assert.Equal(id, impl.Id);
        Assert.Equal("X", impl.Name);
    }

    [Fact]
    public void IEntity_ObjectInitialized_EqualityOfDifferentParts()
    {
        Guid id = Guid.NewGuid();
        TestEntity t = new TestEntity { Id = id, Name = "Name" };
        Assert.Equal(id, t.Id);
        Assert.Equal("Name", t.Name);
    }

    private sealed record TestEntity : IEntity
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
