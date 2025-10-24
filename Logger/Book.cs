namespace Logger;

public record Book : Entity
{
    public required string Title { get; init; }

    // Implicit implementation because Name getter should be publicly available via Book Instance
    public override string Name => $"{Title}";

    public virtual bool Equals(Book? obj)
    {
        if (obj is not Book otherBook)
        {
            return false;
        }

        return this.Title == otherBook.Title;
    }

    public override int GetHashCode()
    {
        return Title.GetHashCode();
    }
}