using System;
using Xunit;

namespace Logger.Tests;

public class StudentTest
{
#pragma warning disable CA1707

    [Fact]
	public void Student_AssignsStudent_Correctly()
	{
		Guid id = Guid.NewGuid();
		FullName Name1 = new("Glitter", "Sparkles", "M");
		Student student = new(id, Name1, "S123456");
		Assert.Equal(id, student.Id);
		Assert.Equal(Name1, student.FullName);
		Assert.Equal("S123456", student.StudentNumber);
	}

	[Fact]
	public void Student_WithNullStudentNumber_ThrowsException()
	{
		Guid id = Guid.NewGuid();
		FullName Name1 = new("Glitter", "Sparkles", "M");

		var Exception = Assert.Throws<ArgumentException>(() => new Student(id, Name1, null!));
		Assert.Contains("studentNumber", Exception.Message);

	}

	[Fact]
	public void Student_WithWhiteSpace_ThrowsException()
	{
		Guid id = Guid.NewGuid();
		FullName Name1 = new("Spongebob", "Squarepants", "S");
		var Exception = Assert.Throws<ArgumentException>(() => new Student(id, Name1, "   "));
		Assert.Contains("studentNumber", Exception.Message);
	}

	[Fact]
	public void Student_Property_ReturnsCorrectFullName()
	{
		Guid id = Guid.NewGuid();
		Assert.Throws<ArgumentException>(() => new Student(id, new FullName("Glitter", "Sparkles", "M"), null!));

	}

	[Fact]
	public void Name_WithoutMiddleName_ReturnsCorrectFormat()
	{
		var fullName = new FullName("Patrick", "Star", null);
		var student = new Student(Guid.NewGuid(), fullName, "12345");

		string name = student.Name;
		Assert.Equal("Patrick Star", name);
    }
#pragma warning restore CA1707
}
