namespace Contacts;

public abstract record Contact(FullName Name, string Email, string? Phone = null) : IContact
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public FullName Name { get; } = Name ?? throw new ArgumentNullException(nameof(Name));

    public string Email { get; } = Required(Email, nameof(Email));

    public string? Phone { get; } = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();

    public virtual bool Equals(Contact? other) =>
        other is not null
        && EqualityContract == other.EqualityContract
        && Name == other.Name
        && Email == other.Email
        && Phone == other.Phone;

    public override int GetHashCode() => HashCode.Combine(EqualityContract, Name, Email, Phone);

    protected static string Required(string value, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        return value.Trim();
    }
}
