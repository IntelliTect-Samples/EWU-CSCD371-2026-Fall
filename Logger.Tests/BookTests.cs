using Xunit;

namespace Logger.Tests;

public class BookTests
{
    [Fact]
    public void Book_NameProperty_ReturnsCorrectFormat()
    {
        // Arrange
        var title = "The Great Gatsby";
        var author = "F. Scott Fitzgerald";
        var book = new entities.Book(title, author);
        // Act
        var name = book.Name;
        // Assert
        Assert.Equal("The Great Gatsby by F. Scott Fitzgerald", name);
    }

    [Fact]
    public void Book_TitleProperty_IsSetCorrectly()
    {
        // Arrange
        var title = "1984";
        var author = "George Orwell";
        var book = new entities.Book(title, author);
        // Act & Assert
        Assert.Equal(title, book.Title);
    }

    [Fact]
    public void Book_AuthorProperty_IsSetCorrectly()
    {
        // Arrange
        var title = "To Kill a Mockingbird";
        var author = "Harper Lee";
        var book = new entities.Book(title, author);
        // Act & Assert
        Assert.Equal(author, book.Author);
    }

    [Fact]
    public void Book_Id_IsReadOnlyAfterInitialization()
    {
        // Arrange
        var book = new Logger.entities.Book("Test", "TestAuthor");
        // Act & Assert
        Assert.NotEqual(Guid.Empty, book.Id);
    }

    [Fact]
    public void Book_Equality_ReturnsFalseForIdenticalContentDueToDifferentIds()
    {
        // Arrange
        var title = "The Hitchhiker's Guide";
        var author = "Douglas Adams";
        // Act
        var book1 = new Logger.entities.Book(title, author);
        var book2 = new Logger.entities.Book(title, author);
        // Assert
        Assert.NotEqual(book1.Id, book2.Id);
        Assert.False(book1.Equals(book2));
        Assert.False(book1 == book2);
        Assert.NotEqual(book1.GetHashCode(), book2.GetHashCode());
    }

    [Fact]
    public void Book_Id_CanBeSetExplicitlyOnInitialization()
    {
        // Arrange
        var expectedId = new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00");
        // Act
        var book = new Logger.entities.Book("Id Test", "Test Author") { Id = expectedId };
        // Assert
        Assert.Equal(expectedId, book.Id);
    }

    [Fact]
    public void Book_Constructor_ThrowsException_ForEmptyTitle()
    {
        // Arrange, Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Logger.entities.Book("", "Author"));
        Assert.Equal("Title cannot be null or empty. (Parameter 'title')", exception.Message);
    }

    [Fact]
    public void Book_Constructor_ThrowsException_ForEmptyAuthor()
    {
        // Arrange, Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new Logger.entities.Book("Title", ""));
        Assert.Equal("Author cannot be null or empty. (Parameter 'author')", exception.Message);
    }
}
