namespace Contacts;

public interface IContact{
    Guid Id { get; }

    FullName Name { get; }

    string Email { get; }

    string? Phone { get; }
}

public abstract record Contact : IContact
{

    protected Contact(FullName name, string email, string? phone = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        Name = name;
        Email = email.Trim();
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
    }

    public Guid Id { get; init; } = Guid.NewGuid();

    public FullName Name { get; init; }

    public string Email { get; init; }

    public string? Phone { get; init; }

    public virtual bool Equals(Contact? other) =>
        other is not null &&
        EqualityContract == other.EqualityContract &&
        Name == other.Name &&
        Email == other.Email &&
        Phone == other.Phone;
    public override int GetHashCode() =>
        HashCode.Combine(EqualityContract, Name, Email, Phone);
    

}


public sealed record FullName
{

    public FullName(string first, string last, string? middle = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(first);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(last);
        firstName = first.Trim();
        middleName = string.IsNullOrWhiteSpace(middle) ? null : middle.Trim();
        lastName = last.Trim(); 
    }
    public string firstName { get; init; }
    public string? middleName { get; init; }
    public string lastName { get; init; }

    public override string ToString()
    {
        if (string.IsNullOrWhiteSpace(middleName))
        {
            return $"{firstName} {lastName}";
        }
        return $"{firstName} {middleName} {lastName}";
    }
}