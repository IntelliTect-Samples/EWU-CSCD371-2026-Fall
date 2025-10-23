using Logger.Entities.People;
using Xunit;

namespace Logger.Tests;

public class StudentTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        FullName name = new("Lisa", null, "Simpson");
        Student student = new(name, 4);

        Assert.Equal(name, student.FullName);
        Assert.Equal("Lisa Simpson", student.Name);
        Assert.Equal(4, student.Gpa);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4.1)]
    public void Constructor_InvalidGpa_ThrowsArgumentException(decimal gpa)
    {
        FullName name = new("Milhouse", null, "Van Houten");
        Assert.Throws<ArgumentOutOfRangeException>(() => new Student(name, gpa));
    }
}
