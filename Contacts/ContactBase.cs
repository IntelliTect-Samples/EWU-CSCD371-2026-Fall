namespace Contacts;

/// <summary>
/// What students and employees share: an identity, a name and an email address.
/// </summary>
/// <remarks>
/// Records synthesize equality over every field, which would include <see cref="Id"/>.
/// Equality is written by hand here so that <see cref="Id"/> is left out while
/// <c>EqualityContract</c> stays in, which keeps a student and an employee with the
/// same data from being equal. Derived records add their own members to this.
/// </remarks>
public abstract record ContactBase : IContact
{
    protected ContactBase(FullName name, string email)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        if (!email.Contains('@'))
        {
            throw new ArgumentException("Email must contain '@'.", nameof(email));
        }

        Name = name;
        Email = email.Trim();
    }

    public Guid Id { get; } = Guid.NewGuid();

    public FullName Name { get; }

    public string Email { get; }

    public abstract string Describe();

    public virtual bool Equals(ContactBase? other) =>
        ReferenceEquals(this, other)
        || (other is not null
            && EqualityContract == other.EqualityContract
            && Name == other.Name
            && Email == other.Email);

    public override int GetHashCode() => HashCode.Combine(EqualityContract, Name, Email);
}
