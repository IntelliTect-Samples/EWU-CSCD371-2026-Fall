namespace Contacts;

public interface IContact
{
    ContactId Id { get; }
    FullName Name { get; }
    ContactEmails Emails { get; }
}

public record ContactId(char TypePrefix, int Number)
{
    /// <summary>
    /// Returns the contact ID as its type prefix followed by its number.
    /// </summary>
    /// <returns>The formatted contact ID.</returns>
    public override string ToString() => $"{TypePrefix}{Number}";
}

public record FullName(
    string FirstName,
    string LastName,
    string? MiddleName = null
);

public record ContactEmails(IReadOnlyList<string> Emails)
{
    /// <summary>
    /// Determines whether this instance has the same emails as another instance,
    /// regardless of their order.
    /// </summary>
    /// <param name="other">The email collection to compare with this instance.</param>
    /// <returns><see langword="true"/> if both instances contain the same emails; otherwise, <see langword="false"/>.</returns>
    public virtual bool Equals(ContactEmails? other)
    {
        // the same emails in any order should equate equal
        return other is not null
            && Emails.OrderBy(email => email)
                .SequenceEqual(
                    other.Emails.OrderBy(email => email)); 
    }
    
    /// <summary>
    /// Returns a hash code based on the emails, independent of their order.
    /// </summary>
    /// <returns>A hash code for this email collection.</returns>
    public override int GetHashCode()
    {
        // note: the same emails in different order should return same hashcode
        HashCode hash = new();
        
        foreach (string email in Emails.OrderBy(email => email))
            hash.Add(email);
        
        return hash.ToHashCode();
    }
}

public abstract record BaseContact(FullName Name, ContactEmails Emails) : IContact
{
    public required ContactId Id { get; init; }

    // equals definition is shared between derivers
    /// <summary>
    /// Determines whether this contact has the same name and emails as another contact.
    /// </summary>
    /// <param name="other">The contact to compare with this instance.</param>
    /// <returns><see langword="true"/> if both contacts have the same name and emails; otherwise, <see langword="false"/>.</returns>
    public virtual bool Equals(BaseContact? other)
    {
        return other is not null
            && Name == other.Name
            && Emails == other.Emails;
    }
    
    // hashcode definition is shared between derivers
    /// <summary>
    /// Returns a hash code based on the contact's name and emails.
    /// </summary>
    /// <returns>A hash code for this contact.</returns>
    public override int GetHashCode() => HashCode.Combine(Name, Emails);
}

public sealed record Student(FullName Name, ContactEmails Emails) : BaseContact(Name, Emails)
{
    // unique to Student
    public const char Prefix = 'S';
}

public sealed record Employee(FullName Name, ContactEmails Emails) : BaseContact(Name, Emails)
{
    // unique to Employee
    public const char Prefix = 'E';
}
