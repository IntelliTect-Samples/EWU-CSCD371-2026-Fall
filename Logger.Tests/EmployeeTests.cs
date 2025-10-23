using Logger.Entities.People;
using Xunit;

namespace Logger.Tests;

public class EmployeeTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        FullName name = new("Homer", "J", "Simpson");
        Employee employee = new(name, 60000);

        Assert.Equal(name, employee.FullName);
        Assert.Equal("Homer J Simpson", employee.Name);
        Assert.Equal(60000, employee.Salary);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_InvalidSalary_ThrowsArgumentException(decimal salary)
    {
        FullName name = new("Carl", null, "Carlson");
        Assert.Throws<ArgumentOutOfRangeException>(() => new Employee(name, salary));
    }
}
