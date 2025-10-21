using System;
using Xunit;

namespace Logger.Tests;

public class BookTest
{
	#pragma warning disable CA1707
	[Fact]
	public void Constructor_AssignsBook_Correctly()
	{

		Guid id = Guid.NewGuid();

		Book book = new(id, "Expert guid to C#", "Mark Michaelis");

		Assert.Equal(id, book.Id);
		Assert.Equal("Expert guid to C#", book.Title);
		Assert.Equal("Mark Michaelis", book.Author);
    }

	[Fact]
	public void Name_WhenAuthorIsNull_ReturnsException()
	{

		Assert.Throws<ArgumentException>(() => new Book(Guid.NewGuid(), "Expert guid to C#", null!));
    }

	[Fact]
	public void Constructor_WhenTitleIsNullorWhiteSpace_ThrowsException()
	{
		Guid id = Guid.NewGuid();
		Assert.Throws<ArgumentException>(() => new Book(id, null!));
		Assert.Throws<ArgumentException>(() => new Book(id, ""));
		Assert.Throws<ArgumentException>(() => new Book(id, "   "));
	}
	#pragma warning restore CA1707
}
