namespace Logger;

public record Book : IEntity
{
    // GUID is defined implicitly because it should make sense that it be displayed and there's no danger of it being
    // misused since it's an init-only property that serves as a simple identifier. Having it accessible directly through
    // Book is okay in this case I think.
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string? Author { get; init; }
    public string? ISBN { get; init; }

    public Book(Guid id, string title, string? author = null, string? isbn = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        Id = id;
        Title = title.Trim();
        Author = string.IsNullOrWhiteSpace(author) ? null : author.Trim();
        ISBN = string.IsNullOrWhiteSpace(isbn) ? null : isbn.Trim();
    }

    // Name is defined implicitly because it will always include a title, and may optionally include author and ISBN.
    // I think it makes sense that a book have a name that's available to see.
    public string Name => $"{Title}{(Author is null ? "" : $" by {Author}")}{(ISBN is null ? "" : $" ({ISBN})")}";
}

