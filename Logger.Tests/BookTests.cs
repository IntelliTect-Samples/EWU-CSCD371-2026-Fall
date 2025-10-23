using Xunit;

namespace Logger.Tests;

public class BookTests
{
    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789012", "The Great Gatsby", "F. Scott Fitzgerald", "9780743273565")]
    [InlineData("87654321-4321-4321-4321-210987654321", "1984", "George Orwell", "9780451524935")]
    [InlineData("11111111-1111-1111-1111-111111111111", "Title Only", null, null)]
    [InlineData("22222222-2222-2222-2222-222222222222", "With Author", "Some Author", null)]
    [InlineData("33333333-3333-3333-3333-333333333333", "With ISBN", null, "1234567890")]
    public void Constructor_ShouldInitializeProperties(string idString, string title, string? author, string? isbn)
    {
        // Arrange
        var id = Guid.Parse(idString);
        // Act
        Book book = new(id, title, author, isbn);
        // Assert
        Assert.Equal(id, book.Id);
        Assert.Equal(title, book.Title);
        Assert.Equal(author, book.Author);
        Assert.Equal(isbn, book.ISBN);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenTitleIsNullOrWhiteSpace()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Book(id, null!));
        Assert.Throws<ArgumentException>(() => new Book(id, ""));
        Assert.Throws<ArgumentException>(() => new Book(id, "   "));
    }

    [Fact]
    public void Constructor_ExtraWhitespaceParams_ShouldTrimParams()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "  Sample Title  ";
        var author = "  Sample Author  ";
        var isbn = "  1234567890  ";
        // Act
        Book book = new(id, title, author, isbn);
        // Assert
        Assert.Equal("Sample Title", book.Title);
        Assert.Equal("Sample Author", book.Author);
        Assert.Equal("1234567890", book.ISBN);
    }

    [Fact]
    public void title_should_be_trimmed()
    {
        // Arrange
        var id = Guid.NewGuid();
        var title = "  Trimmed Title  ";
        // Act
        Book book = new(id, title);
        // Assert
        Assert.Equal("Trimmed Title", book.Title);
    }

    [Fact]
    public void Testing_Equality_BehavesProperly()
    { 
        // Arrange
        var id = Guid.NewGuid();
        var book1 = new Book(id, "Sample Title", "Author Name", "1234567890");
        var book2 = new Book(id, "Sample Title", "Author Name", "1234567890");
        // Act & Assert
        // Should be equal because all properties are the same
        Assert.Equal(book1, book2);
    }

    [Fact]
    public void Testing_NonEquality_BehavesProperly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var book1 = new Book(id, "Sample Title", "Author Name", "1234567890");
        var book2 = new Book(id, "Sample Title2", "Author Name", "1234567890");
        // Act & Assert
        // Should not be equal because of different titles
        Assert.NotEqual(book1, book2);
    }
}