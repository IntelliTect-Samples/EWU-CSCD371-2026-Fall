using System;   
using Xunit;

namespace Logger.Tests;

public class EntityBaseTests
{
    [Fact]
    public void EntityBase_DerivedProvidesNameAndId_SetsProperties()
    {
        Guid id = new();
        TestDerived derived = new TestDerived { Id = id };

        Assert.Equal(id, derived.Id);
        Assert.Equal("Test Derived", derived.Name);
    }

    [Fact]
    public void EntityBase_IsAbstract_True()
    {
        Assert.True(typeof(Logger.EntityBase).IsAbstract);
    }

    [Fact]
    public void EntityBase_NameProperty_IsAbstract()
    {
        System.Reflection.PropertyInfo? prop = typeof(Logger.EntityBase).GetProperty("Name");
        System.Reflection.MethodInfo? getter = prop?.GetGetMethod();
        Assert.NotNull(getter);
        Assert.True(getter!.IsAbstract);
    }

    [Fact]
    public void EntityBase_Derived_CanBeUsedAsIEntity()
    {
        TestDerived d = new TestDerived { Id = Guid.NewGuid() };
        IEntity i = d;
        Assert.Equal(d.Name, i.Name);
        Assert.Equal(d.Id, i.Id);
    }

    private sealed class TestDerived : Logger.EntityBase
    {
        public override string Name => "Test Derived";
    }
}
