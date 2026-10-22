using System.Text;
using System.Text.Json.Serialization;

namespace Contacts;

public abstract record class Contact : IContact
{
    private static readonly StringComparer EmailComparer = StringComparer.Ordinal;

    [JsonInclude]
    public Guid Id { get; internal set; }
    public FullName Name { get; }
    public IReadOnlyList<string> Emails { get; }
    public string? PhoneNumber { get; }
    public string? Address { get; }

    protected Contact(FullName name, IReadOnlyList<string>? emails = null, string? phoneNumber = null, string? address = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        Id = Guid.NewGuid();
        Name = name;
        string[] cleanedEmails = [.. (emails ?? [])
            .Where(email => !string.IsNullOrWhiteSpace(email))
            .Select(email => email.Trim())
            .Distinct(EmailComparer)
            .Order(EmailComparer)];
        Emails = Array.AsReadOnly(cleanedEmails);
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber;
        Address = string.IsNullOrWhiteSpace(address) ? null : address;

        if (Emails.Count == 0 && PhoneNumber is null && Address is null)
        {
            throw new ArgumentException("At least one contact method must be entered");
        }
    }

    public virtual bool Equals(Contact? other)
    {
        if (other is null || EqualityContract != other.EqualityContract)
        {
            return false;
        }
        return Name == other.Name && Emails.SequenceEqual(other.Emails, EmailComparer) &&
            PhoneNumber == other.PhoneNumber && Address == other.Address;
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(EqualityContract);
        hash.Add(Name);
        hash.Add(PhoneNumber);
        hash.Add(Address);
        foreach (string email in Emails)
        {
            hash.Add(email, EmailComparer);
        }

        return hash.ToHashCode();
    }

    public override string ToString()
    {
        StringBuilder builder = new();

        AppendField(builder, nameof(Id), Id.ToString());
        AppendField(builder, nameof(Name), Name.ToString());
        AppendField(builder, nameof(Emails), Emails.Count == 0 ? null : string.Join(", ", Emails));
        AppendField(builder, nameof(PhoneNumber), PhoneNumber);
        AppendField(builder, nameof(Address), Address);

        return builder.ToString();
    }

    protected static void AppendField(StringBuilder builder, string label, string? value)
    {
        if (value is null)
        {
            return;
        }

        builder.Append(label);
        builder.Append(": ");
        builder.AppendLine(value);
    }
}
