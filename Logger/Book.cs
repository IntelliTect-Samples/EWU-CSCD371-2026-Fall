namespace Logger;

public record Book : IEntity
{


    public Guid Id { get; init; }
    public string Title { get; init; }
    public string? Author { get; init; }

    /// <summary>
    /// Id is used implicitly to expose it as part of the public API.
    /// bring immutability by using init-only setters. 
    /// Every Bookstore has a unique identifier, allowing the system to track
    ///
    /// Name is calculated based on Title and Author, being exposed implicitly
    /// as part of the public API and should be able to be seen publicly.
    /// </summary>

    public Book(Guid id, string title, string? author = null)
    {
        Id = id;
        Title = title;
        Author = author;
    }
    
    public string Name => Author is null ? Title : $"{Title} by {Author}";

    
} 

