using Logger.Entities.People;
using Xunit;

namespace Logger.Tests;

public class PersonTests
{
    [Fact]
    public void Constructor_ValidFullName_SetsProperty()
    {
        FullName fullName = new("Ned", "Flanders", "Sr");
        TestPerson person = new(fullName);

        Assert.Equal(fullName, person.FullName);
        Assert.Equal("Ned Flanders Sr", person.Name);
        Assert.NotEqual(Guid.Empty, person.Id);
    }

    [Fact]
    public void Constructor_NullFullName_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TestPerson(null!));
    }

    private sealed record class TestPerson(FullName name) : Person(name);
}
