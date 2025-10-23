using Logger.Entities.Books;
using Xunit;

namespace Logger.Tests;

public class BookTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        Book book = new("1984", new FullName("George", null, "Orwell"));

        Assert.Equal("1984 by George Orwell", book.Name);
        Assert.NotEqual(Guid.Empty, book.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidTitle_ThrowsArgumentException(string? title)
    {
        Assert.Throws<ArgumentException>(() => new Book(title!, new FullName("A", "B", "C")));
    }

    [Fact]
    public void Constructor_InvalidAuthor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Book("Title", null!));
    }
}
