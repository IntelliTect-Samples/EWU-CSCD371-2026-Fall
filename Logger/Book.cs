namespace Logger;

public record Book : EntityBase
{
   // public new Guid Id { get; init; }
    public string Title { get; init; }
    public string? Author { get; init; }

    /// <summary>
    /// Id is used implicitly to expose it as part of the public API.
    /// bring immutability by using init-only setters. 
    /// Every Bookstore has a unique identifier, allowing the system to track
    ///
    /// Name is calculated based on Title and Author, being exposed implicitly
    /// as part of the public API and should be able to be seen publicly.
    /// Usually we refer to books by their title and author together. Usually..
    /// </summary>

    public Book(Guid id, string title, string? author = null)
    {

        Id = id;
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException($"'{nameof(title)}' cannot be null or whitespace.", nameof(title))
            : title;

        Author = string.IsNullOrWhiteSpace(author)
            ? throw new ArgumentException($"'{nameof(author)}' cannot be null ot whitespace.", nameof(author))
            : author;
    }
    
    public override string Name =>  $"{Title} by {Author}";

    
} 

