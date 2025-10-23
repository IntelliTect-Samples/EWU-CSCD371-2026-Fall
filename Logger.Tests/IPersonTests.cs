using System;
using Xunit;

namespace Logger.Tests;

public class IPersonTests
{
    private sealed record TestPerson(Guid Id, FullName FullName) : IPerson
    {
        public string Name => FullName.Name;
    }

    [Fact]
    public void IPerson_ValidParams_CreatesPerson()
    {
        Guid id = Guid.NewGuid();
        FullName name = new("Ryan", "Hirte", "A.");
        IPerson person = new TestPerson(id, name);
        Assert.Equal(id, person.Id);
        Assert.Equal(name, person.FullName);
        Assert.Equal("Ryan A. Hirte", person.Name);
    }

    [Fact]
    public void IPerson_NullMiddleName_IsEqual()
    {
        Guid id = Guid.NewGuid();
        FullName name = new("John", "Deer");
        IPerson person = new TestPerson(id, name);
        Assert.Equal("John Deer", person.Name);
    }

    [Fact]
    public void IPerson_SameValues_IsEqual()
    {
        Guid id = Guid.NewGuid();
        FullName name = new("Inigo", "Montoya");
        IPerson person1 = new TestPerson(id, name);
        IPerson person2 = new TestPerson(id, name);
        Assert.Equal(person1, person2);
    }

    [Fact]
    public void IPerson_DifferentId_NotEqual()
    {
        Guid id1 = Guid.NewGuid();
        Guid id2 = Guid.NewGuid();
        FullName name = new("Inigo", "Montoya");
        IPerson person1 = new TestPerson(id1, name);
        IPerson person2 = new TestPerson(id2, name);
        Assert.NotEqual(person1, person2);
    }
}
