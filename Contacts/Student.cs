namespace Contacts;

public sealed record Student : ContactBase
{
    public Student(FullName name, string email, string school)
        : base(name, email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(school);

        School = school.Trim();
    }

    public string School { get; }

    public override string Describe() => $"Student: {Name} <{Email}> - {School}";
}
