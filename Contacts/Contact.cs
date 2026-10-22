namespace Contacts;

public abstract record Contact(FullName Name, IReadOnlyList<string> Emails, string? Phone = null) : IContact
{
    private static readonly StringComparer EmailComparer = StringComparer.OrdinalIgnoreCase;

    public Guid Id { get; init; } = Guid.NewGuid();

    public FullName Name { get; } = Name ?? throw new ArgumentNullException(nameof(Name));

    public IReadOnlyList<string> Emails { get; } = RequiredEmails(Emails, nameof(Emails));

    public string? Phone { get; } = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();

    public virtual bool Equals(Contact? other) =>
        other is not null
        && EqualityContract == other.EqualityContract
        && Name == other.Name
        && Phone == other.Phone
        && SortedEmails().SequenceEqual(other.SortedEmails(), EmailComparer);

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(EqualityContract);
        hash.Add(Name);
        hash.Add(Phone);

        foreach (string email in SortedEmails())
        {
            hash.Add(email, EmailComparer);
        }

        return hash.ToHashCode();
    }

    protected static string Required(string value, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        return value.Trim();
    }

    private IEnumerable<string> SortedEmails() => Emails.Order(EmailComparer);

    private static string[] RequiredEmails(IReadOnlyList<string> emails, string paramName)
    {
        ArgumentNullException.ThrowIfNull(emails, paramName);

        string[] cleaned = [.. emails.Select(email => Required(email, paramName)).Distinct(EmailComparer)];

        if (cleaned.Length == 0)
        {
            throw new ArgumentException("At least one email is required.", paramName);
        }

        return cleaned;
    }
}