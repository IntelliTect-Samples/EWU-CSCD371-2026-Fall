using System.Text;

namespace Contacts;

public record class Employee : Contact
{
    public string? Employer { get; }
    public string? JobTitle { get; }

    public Employee(FullName name, IReadOnlyList<string>? emails = null, string? phoneNumber = null,
        string? address = null, string? employer = null, string? jobTitle = null)
        : base(name, emails, phoneNumber, address)
    {
        Employer = string.IsNullOrWhiteSpace(employer) ? null : employer;
        JobTitle = string.IsNullOrWhiteSpace(jobTitle) ? null : jobTitle;
    }

    public override string ToString()
    {
        StringBuilder builder = new(base.ToString());

        AppendField(builder, nameof(Employer), Employer);
        AppendField(builder, nameof(JobTitle), JobTitle);

        return builder.ToString();
    }
}
