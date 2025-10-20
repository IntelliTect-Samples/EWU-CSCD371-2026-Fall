using System;
using Xunit;

namespace Logger.Tests;

public class BookTest
{

	[Fact]
	public void Constructor_AssignsId_Correctly() { 

		Guid id = Guid.NewGuid();

		Book book = new(id, "Expert guid to C#", "Mark Michaelis");

		Assert.Equal(id, book.Id);
	}

	//[Fact]
	//public void Name_
}
