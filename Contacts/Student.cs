using System.Text;

namespace Contacts;

public record class Student : Contact
{
    public string? SchoolName { get; }
    public string? Major { get; }

    public Student(FullName name, IReadOnlyList<string>? emails = null, string? phoneNumber = null,
        string? address = null, string? schoolName = null, string? major = null)
        : base(name, emails, phoneNumber, address)
    {
        SchoolName = string.IsNullOrWhiteSpace(schoolName) ? null : schoolName;
        Major = string.IsNullOrWhiteSpace(major) ? null : major;
    }

    public override string ToString()
    {
        StringBuilder builder = new(base.ToString());

        AppendField(builder, nameof(SchoolName), SchoolName);
        AppendField(builder, nameof(Major), Major);

        return builder.ToString();
    }
}
