namespace Logger;

public record Book(Guid Id, string Title, string Author) : IEntity
{
    ///<summary>
    ///Implemented implicitly because Id is part of public API
    ///As this is a record, immutability is conserved through the constructor
    ///</summary>

    public Guid Id { get; init; } = Id;

    /// <summary>
    /// Implemented implicitly as a calculated property as books name is derived
    /// from title + author
    /// With no backing field, the getter computes the value
    /// </summary>
    public string Name => $"{Title} by {Author }";
    /// <summary>
    /// book must not logically allow renaming manually
    /// Therefore the setter is implemented explicity so that
    /// the interface is honored
    /// the public API still is immutable
    /// </summary>

    string IEntity.Name 
    {
        get => Name;
        set { }
    }
    ///<summary>
    ///setter ignored to preserve immutability
    /// </summary>
} 

