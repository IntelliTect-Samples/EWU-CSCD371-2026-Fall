#nullable enable
namespace Logger;

/// <summary>
/// Student entity.
/// IEntity is implemented implicitly, including init only to prevent mutation.
/// </summary>
public sealed record class Student : PersonEntity
{
    public string? StudentNumber { get; init; }

    public bool Equals(Student? other) =>
            other is not null && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}

