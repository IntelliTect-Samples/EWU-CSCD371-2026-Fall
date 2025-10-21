using System;
using Xunit;

namespace Logger.Tests;

public class EmployeeTest
{
#pragma warning disable CA1707
    [Fact]
	public void Constructor_AssignsEmployee_Correctly()
	{
		Guid id = Guid.NewGuid();
		FullName fullname = new FullName("Uli", "Agular", "A");
		
		Employee employee = new(id, fullname, "Developer");

		Assert.Equal(id, employee.Id);
		Assert.Equal(fullname, employee.FullName);
		Assert.Equal("Developer", employee.Position);
    }

	[Fact]
	public void Constructor_WithNullOrWhiteSpacePosition_ThrowsException()
	{
		Guid id = Guid.NewGuid();
		FullName fullname = new FullName("Leoniel", "Messi", "A");
		
		var exception1 = Assert.Throws<ArgumentException>(() => new Employee(id, fullname, null));
		Assert.Contains("position", exception1.Message);
		
		var exception2 = Assert.Throws<ArgumentException>(() => new Employee(id, fullname, ""));
		Assert.Contains("position", exception2.Message);
		
		var exception3 = Assert.Throws<ArgumentException>(() => new Employee(id, fullname, "   "));
		Assert.Contains("position", exception3.Message);
    }

	[Fact]
	public void Constructor_WithoutPosition_AssignsNullPosition()
	{
		Guid id = Guid.NewGuid();
		FullName fullname = new FullName("Rudy", "Madrigal", "B");
		
		var exception = Assert.Throws<ArgumentException>(() => new Employee(id, fullname));
		Assert.Contains("position", exception.Message);
    }

	[Fact]
	public void Constructor_WithAndWithoutMiddleName_ReturnsCorrectFormat()
	{
		Guid id1 = Guid.NewGuid();
		
		FullName fullname1 = new FullName("Ana", "Smith", "L");
		Employee employee1 = new(id1, fullname1, "Manager");
		
		Assert.Equal("Ana L Smith", employee1.Name);
		
		Guid id2 = Guid.NewGuid();
		
		FullName fullname2 = new FullName("Bob", "Johnson", null);
		Employee employee2 = new(id2, fullname2, "Analyst");
		
		Assert.Equal("Bob Johnson", employee2.Name);
    }
#pragma warning restore CA1707

}
