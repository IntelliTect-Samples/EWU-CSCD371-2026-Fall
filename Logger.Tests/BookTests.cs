using System;
using Xunit;

namespace Logger.Tests;

public class BookTests
{

    [Fact]
    public void Book_Create_InitializesProperties()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var book = new Book { Id = id, Title = "The Wave", Author = "Todd Strasser" };

        // Act
        var title = book.Title;
        var author = book.Author;
        Guid bookId = book.Id;

        // Assert
        Assert.Equal("The Wave", title);
        Assert.Equal("Todd Strasser", author);
        Assert.Equal(id, bookId);
    }

    [Fact]
    public void Book_SameTitle_NotEqual()
    {
        // Arrange
        var book1 = new Book { Title = "Law of Attraction" };
        var book2 = new Book { Title = "Law of Attraction" };

        // Act & Assert
        Assert.NotEqual(book1, book2);
    }

    [Fact]
    public void Author_AuthorNotSet_ReturnsNull()
    {
        // Arrange
        var book = new Book { Title = "Lock Wood" };

        // Act
        var author = book.Author;

        // Assert
        Assert.Null(author);
    }
}