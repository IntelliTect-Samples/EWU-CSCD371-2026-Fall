using System;


namespace Logger;

public record Book : EntityBase
{
    public string Title { get; init; }
    public string Author { get; init; }
    public string ISBN { get; init; }

    

     // Name is implemented implicitly - it's natural for consumers to access a book's name
    // directly without needing to cast to IEntity. The book's name is its title.
    public override string Name => Title;

    public Book(Guid id, string title, string author, string isbn) : base(id)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Author = author ?? throw new ArgumentNullException(nameof(author));
        ISBN = isbn ?? throw new ArgumentNullException(nameof(isbn));
    }
}