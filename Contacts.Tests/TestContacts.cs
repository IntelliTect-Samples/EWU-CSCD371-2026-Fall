namespace Contacts.Tests;

internal static class TestContacts
{
    public static Student MakeStudent(
        string first = "Ada",
        string last = "Lovelace",
        string email = "ada@example.com",
        string school = "EWU") =>
        new(new FullName(first, last), email, school);

    public static Employee MakeEmployee(
        string first = "Ada",
        string last = "Lovelace",
        string email = "ada@example.com",
        string company = "EWU",
        string title = "Engineer") =>
        new(new FullName(first, last), email, company, title);
}
