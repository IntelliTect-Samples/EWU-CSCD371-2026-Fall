namespace Contacts;
 
public sealed record Employee : Contact
{
    public Employee(FullName name, string email, string company, string jobTitle, string? phone = null)
        : base(name, email, phone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(company);
        ArgumentException.ThrowIfNullOrWhiteSpace(jobTitle);
 
        Company = company.Trim();
        JobTitle = jobTitle.Trim();
    }
 
    public string Company { get; }
 
    public string JobTitle { get; }
}
