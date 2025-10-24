using Xunit;
using Logger;

namespace Logger.Tests;

public class PersonTests
{

    private sealed record TestPerson : Person
    {
        public TestPerson(FullName fullName)
        {
            FullName = fullName;
        }
    }

    [Fact]
    public void Name_NoMiddleName_ReturnsFirstAndLast()
    {
        // Arrange
        FullName fullName = new(FirstName:"Alan", MiddleName:string.Empty,LastName: "Wake");
        Person person = new TestPerson(fullName);

        // Act
        string result = person.Name;

        // Assert
        Assert.Equal("Alan Wake", result);
    }

    [Fact]
    public void Name_WithMiddleName_ReturnsFirstMiddleLast()
    {
        // Arrange
        FullName fullName = new(FirstName:"Brennan",MiddleName: "Lee", LastName:"Mulligan");
        Person person = new TestPerson(fullName);

        // Act
        string result = person.Name;

        // Assert
        Assert.Equal("Brennan Lee Mulligan", result);
    }

}
