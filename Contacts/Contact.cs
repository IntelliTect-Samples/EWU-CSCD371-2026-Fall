using System.Text;

namespace Contacts;

public abstract record class Contact : IContact
{
    public Guid Id { get; }
    public FullName Name { get; }
    public string? Email { get; }
    public string? PhoneNumber { get; }
    public string? Address { get; }

    protected Contact(FullName name, string? email = null, string? phoneNumber = null, string? address = null)
    {
        ArgumentNullException.ThrowIfNull(name);

        Id = Guid.NewGuid();
        Name = name;
        Email = string.IsNullOrWhiteSpace(email) ? null : email;
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber;
        Address = string.IsNullOrWhiteSpace(address) ? null : address;

        if (Email is null && PhoneNumber is null && Address is null)
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
        return Name == other.Name && Email == other.Email &&
            PhoneNumber == other.PhoneNumber && Address == other.Address;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EqualityContract, Name, Email, PhoneNumber, Address);
    }

    public override string ToString()
    {
        StringBuilder builder = new();

        AppendField(builder, nameof(Id), Id.ToString());
        AppendField(builder, nameof(Name), Name.ToString());
        AppendField(builder, nameof(Email), Email);
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
