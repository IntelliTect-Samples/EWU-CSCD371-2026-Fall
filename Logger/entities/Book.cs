namespace Logger.entities;

// The Name property is implemented implicitly because it should be publicly accessible as part of the class's API.
public record class Book : EntityBase
{
    public string Title { get; init; }
    public string Author { get; init; }
    public override string Name => $"{Title} by {Author}";

    public Book(string title, string author)
    {
        Title = title;
        Author = author;

        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(Author))
        {
            throw new ArgumentException("Author cannot be null or empty.", nameof(author));
        }
    }
}
