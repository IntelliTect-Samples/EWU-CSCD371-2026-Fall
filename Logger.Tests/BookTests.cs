using System;
using System.Collections.Generic;
using Xunit;

namespace Logger.Tests;

public class BookTests
{
    [Fact]
    public void Name_ValidName_ReturnsName()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string bookName = "Dungeon Crawler Carl";
        Book book = new Book(id, bookName);

        // Act
        string name = book.Name;

        // Assert
        Assert.Equal("Dungeon Crawler Carl", name);
    }

    [Fact]
    public void Name_NullOrWhitespace_ReturnsEmptyString()
    {
        // Arrange
        String? nullInput = null;
        Guid id1 = Guid.NewGuid();
        Book book1 = new Book(id1, nullInput!);

        Guid id2 = Guid.NewGuid();
        Book book2 = new Book(id2, "   ");

        // Act
        string name1 = book1.Name;
        string name2 = book2.Name;

        // Assert
        Assert.Equal(string.Empty, name1);
        Assert.Equal(string.Empty, name2);
    }

    [Fact]
    public void Constructor_SetsIdCorrectly()
    {
        // Arrange
        Guid expectedId = Guid.NewGuid();
        string bookName = "Dungeon Crawler Carl";

        // Act
        Book book = new Book(expectedId, bookName);

        // Assert
        Assert.Equal(expectedId, book.Id);
    }

    [Fact]
    public void Equality_TwoBooksWithSameIdAndName_AreEqual()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string bookName = "Dungeon Crawler Carl";
        Book book1 = new Book(id, bookName);
        Book book2 = new Book(id, bookName);

        // Act
        bool areEqual = book1.Equals(book2);

        // Assert
        Assert.True(areEqual);
    }

    [Fact]
    public void Equality_TwoBooksWithDifferentId_AreNotEqual()
    {
        // Arrange
        Book book1 = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");
        Book book2 = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");

        // Act
        bool areEqual = book1.Equals(book2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void GetHashCode_TwoBooksWithSameIdAndName_HaveSameHashCode()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string bookName = "Dungeon Crawler Carl";
        Book book1 = new Book(id, bookName);
        Book book2 = new Book(id, bookName);

        // Act
        int hash1 = book1.GetHashCode();
        int hash2 = book2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void Book_ImplementsIEntity_ReturnsTrue()
    {
        // Arrange
        Book book = new Book(Guid.NewGuid(), "Dungeon Crawler Carl");

        // Act
        bool implementsInterface = book is IEntity;

        // Assert
        Assert.True(implementsInterface);
    }
}
