namespace Logger;

// Change Book from a record to a class, since records cannot inherit from non-record classes.
// Implement the required properties and constructor.
public class Book : EntityBase
{
    public string Title { get; init; }
    public string Author { get; init; }

    
    public override string Name
    {
        get => $"{Title} by {Author}";
        set => throw new InvalidOperationException();
    }
}
