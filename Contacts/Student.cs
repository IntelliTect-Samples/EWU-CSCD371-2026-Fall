namespace Contacts;

public sealed record Student : Contact
{
    public Student(FullName name, string email, string school, string major, string? phone = null) : base(name, email, phone)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(school);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(major);
        School = school.Trim();
        Major = major.Trim();
    }

    public string School { get; init; }

    public string Major { get; init; }

}