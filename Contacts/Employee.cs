namespace Contacts;

public sealed record Employee : ContactBase
{
    public Employee(FullName name, string email, string company, string title)
        : base(name, email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(company);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Company = company.Trim();
        Title = title.Trim();
    }

    public string Company { get; }

    public string Title { get; }

    public override string Describe() => $"Employee: {Name} <{Email}> - {Title} at {Company}";
}
