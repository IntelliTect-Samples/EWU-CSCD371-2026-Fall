namespace Contacts;

public abstract record BaseContact(int Id, FullName Name) : IContact
{
    public abstract string ContactInfo { get; }

    public virtual bool Equals(BaseContact? other)
    {
        if (other is null)
            return false;

        return other.ContactInfo == ContactInfo &&
               other.Name == Name;
    }

    public override int GetHashCode()
        => HashCode.Combine(Name, ContactInfo);
}