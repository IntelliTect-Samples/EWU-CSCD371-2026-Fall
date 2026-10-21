namespace Contacts;

public abstract record Contact(FullName Name, string Email) : IContact
{
    public FullName Name { get; init; } = Name ?? throw new ArgumentNullException(nameof(Name));
 
    public string Email { get; init; } = RequiredEmail(Email);
 
    public Guid Id { get; init; } = Guid.NewGuid();

    public virtual bool Equals(Contact? other) =>
        other is not null
        && EqualityContract == other.EqualityContract
        && Name == other.Name
        && Email == other.Email;
 
    public override int GetHashCode() => HashCode.Combine(EqualityContract, Name, Email);
 
    private static string RequiredEmail(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        return email.Trim();
    }
}
 
