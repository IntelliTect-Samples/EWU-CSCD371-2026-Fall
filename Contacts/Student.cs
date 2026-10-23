namespace Contacts;

public sealed record Student(int Id, FullName Name, string Major)
    : BaseContact(Id, Name)
{
    public override string ContactInfo => "Student";
}