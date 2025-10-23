using Logger.Entities;

namespace Logger.Entities.Books;

public record class Book : EntityBase
{
    private string Title { get; init; }
    private FullName Author { get; init; }

    /// <summary>
    /// Implemented implicitly since Name is part of a Book's public identity
    /// </summary>
    public override string Name => $"{Title} by {Author}";

    public Book(string title, FullName author)
    {
        Title = string.IsNullOrWhiteSpace(title)
                ? throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title))
                : title;
        ArgumentNullException.ThrowIfNull(author, nameof(author));
        Author = author;
    }
}
