
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contacts;

namespace Contacts.Tests;

[TestClass]
public class ContactEqualityTests
{
    private static FullName CreateName() =>
        new("Alex", "Smith", null);

    private static string[] CreateEmails() =>
        ["alex@example.com", "alex@work.example"];

    private static Student CreateStudent(
        string major = "Computer Science",
        string graduationYear = "2028",
        IReadOnlyList<string>? emails = null,
        FullName? name = null) =>
        new(
            name ?? CreateName(),
            major,
            graduationYear,
            emails ?? CreateEmails());

    private static Employee CreateEmployee(
        string department = "IT",
        string companyName = "Example Corp",
        IReadOnlyList<string>? emails = null,
        FullName? name = null) =>
        new(
            name ?? CreateName(),
            department,
            companyName,
            emails ?? CreateEmails());

    [TestMethod]
    public void EqualStudents_WithDifferentIds_AreEqual()
    {
        var first = CreateStudent();
        var second = CreateStudent(
            emails: new List<string>(CreateEmails()));

        Assert.AreNotEqual(first.ID, second.ID);
        Assert.AreEqual(first, second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void StudentEquality_IncludesMajor()
    {
        Assert.AreNotEqual(
            CreateStudent(),
            CreateStudent(major: "Mathematics"));
    }

    [TestMethod]
    public void StudentEquality_IncludesGraduationYear()
    {
        Assert.AreNotEqual(
            CreateStudent(),
            CreateStudent(graduationYear: "2029"));
    }

    [TestMethod]
    public void EmployeeEquality_IncludesDepartment()
    {
        Assert.AreNotEqual(
            CreateEmployee(),
            CreateEmployee(department: "Finance"));
    }

    [TestMethod]
    public void EmployeeEquality_IncludesCompany()
    {
        Assert.AreNotEqual(
            CreateEmployee(),
            CreateEmployee(companyName: "Different Corp"));
    }

    [TestMethod]
    public void ContactEquality_IncludesEmailContents()
    {
        var first = CreateStudent();

        var differentEmails = new[]
        {
            "alex@example.com",
            "different@work.example"
        };

        var second = CreateStudent(emails: differentEmails);

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void ContactEquality_IncludesEmailOrder()
    {
        var first = CreateStudent();

        var reversedEmails = CreateEmails().Reverse().ToArray();
        var second = CreateStudent(emails: reversedEmails);

        Assert.AreNotEqual(first, second);
    }

    [TestMethod]
    public void BaseEquality_ReturnsFalseForNullOrDifferentContactType()
    {
        ContactBase student = CreateStudent();
        ContactBase employee = CreateEmployee();

        Assert.IsFalse(student.Equals((ContactBase?)null));
        Assert.IsFalse(student.Equals(employee));
    }
}
