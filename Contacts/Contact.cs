namespace Contacts;

public abstract record Contact(FullName Name, string Email, string? Phone = null) : IContact
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public FullName Name { get; } = Name ?? throw new ArgumentNullException(nameof(Name));

    public string Email { get; } = Required(Email, nameof(Email));

    public string? Phone { get; } = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();

    protected static string Required(string value, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        return value.Trim();
    }
}
