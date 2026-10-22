using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    [TestMethod]
    public void Add_NullContact_ThrowsArgumentNullException()
    {
        ContactBook contactBook = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => contactBook.Add(null!));
    }

    [TestMethod]
    public void Add_DuplicateContact_ReturnsFalseAndKeepsSingleContact()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Student duplicateStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);

        Assert.IsTrue(contactBook.Add(storedStudent));
        Assert.IsFalse(contactBook.Add(duplicateStudent));
        Assert.ContainsSingle(contactBook.GetAll());
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameData_StoresBoth()
    {
        ContactBook contactBook = new();
        Student student = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Employee employee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);

        contactBook.Add(student);

        Assert.IsTrue(contactBook.Add(employee));
        Assert.HasCount(2, contactBook.GetAll());
    }

    [TestMethod]
    public void Contains_NullContact_ThrowsArgumentNullException()
    {
        ContactBook contactBook = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => contactBook.Contains(null!));
    }

    [TestMethod]
    public void Contains_EqualButDifferentInstance_ReturnsTrue()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Student matchingStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);

        contactBook.Add(storedStudent);

        Assert.IsTrue(contactBook.Contains(matchingStudent));
    }

    [TestMethod]
    public void Contains_AbsentContact_ReturnsFalse()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Student absentStudent = new(new FullName("ValidFirst", "ValidLast"), ["DifferentEmail"]);

        contactBook.Add(storedStudent);

        Assert.IsFalse(contactBook.Contains(absentStudent));
    }

    [TestMethod]
    public void Remove_NullContact_ThrowsArgumentNullException()
    {
        ContactBook contactBook = new();

        Assert.ThrowsExactly<ArgumentNullException>(() => contactBook.Remove(null!));
    }

    [TestMethod]
    public void Remove_EqualButDifferentInstance_RemovesStoredContact()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Student matchingStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);

        contactBook.Add(storedStudent);

        Assert.IsTrue(contactBook.Remove(matchingStudent));
        Assert.IsEmpty(contactBook.GetAll());
    }

    [TestMethod]
    public void Remove_AbsentContact_ReturnsFalseAndPreservesStoredContact()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Student absentStudent = new(new FullName("ValidFirst", "ValidLast"), ["DifferentEmail"]);

        contactBook.Add(storedStudent);

        Assert.IsFalse(contactBook.Remove(absentStudent));
        IContact remainingContact = Assert.ContainsSingle(contactBook.GetAll());
        Assert.AreSame(storedStudent, remainingContact);
    }

    [TestMethod]
    public void SaveToFile_ThenLoadFromFile_PreservesContactsAndIds()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");
        Student student = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail", "SchoolEmail"], schoolName: "ValidSchool");
        Employee employee = new(new FullName("OtherFirst", "OtherLast"), ["OtherEmail", "WorkEmail"], employer: "ValidEmployer");
        ContactBook contactBook = new();
        contactBook.Add(student);
        contactBook.Add(employee);

        try
        {
            contactBook.SaveToFile(filePath);

            ContactBook loadedContactBook = ContactBook.LoadFromFile(filePath);
            Assert.HasCount(2, loadedContactBook.GetAll());
            Assert.IsTrue(loadedContactBook.Contains(student));
            Assert.IsTrue(loadedContactBook.Contains(employee));
            Assert.AreEqual(student.Id, loadedContactBook.GetAll().OfType<Student>().Single().Id);
            Assert.AreEqual(employee.Id, loadedContactBook.GetAll().OfType<Employee>().Single().Id);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [TestMethod]
    public void LoadFromFile_MissingFile_ReturnsEmptyContactBook()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");

        ContactBook contactBook = ContactBook.LoadFromFile(filePath);

        Assert.IsEmpty(contactBook.GetAll());
    }

    [TestMethod]
    public void SaveToFile_SerializationFails_PreservesExistingFile()
    {
        string directory = Path.Combine(Path.GetTempPath(),
            $"contacts-test-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directory);
        string filePath = Path.Combine(directory, "contacts.json");

        try
        {
            ContactBook contactBook = new();
            contactBook.Add(new Student(new FullName("Alice", "Smith"),
                ["alice@example.com"]));

            contactBook.SaveToFile(filePath);
            string originalJson = File.ReadAllText(filePath);

            contactBook.Add(new UnsupportedContact());

            Assert.ThrowsExactly<NotSupportedException>(
                () => contactBook.SaveToFile(filePath));

            Assert.AreEqual(originalJson, File.ReadAllText(filePath));
            Assert.HasCount(1, Directory.GetFiles(directory));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Add_ReorderedEmails_RejectsDuplicateAndSupportsContainsAndRemove(bool isEmployee)
    {
        FullName name = new("Alice", "Smith");
        Contact original = isEmployee
            ? new Employee(name, ["work@example.com", "personal@example.com"])
            : new Student(name, ["work@example.com", "personal@example.com"]);
        Contact reordered = isEmployee
            ? new Employee(name, ["personal@example.com", "work@example.com"])
            : new Student(name, ["personal@example.com", "work@example.com"]);
        ContactBook contactBook = new();

        Assert.IsTrue(contactBook.Add(original));
        Assert.IsFalse(contactBook.Add(reordered));
        Assert.ContainsSingle(contactBook.GetAll());
        Assert.IsTrue(contactBook.Contains(reordered));
        Assert.IsTrue(contactBook.Remove(reordered));
        Assert.IsEmpty(contactBook.GetAll());
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameMultipleEmails_StoresBoth()
    {
        FullName name = new("Alice", "Smith");
        ContactBook contactBook = new();

        Assert.IsTrue(contactBook.Add(new Student(name, ["work@example.com", "personal@example.com"])));
        Assert.IsTrue(contactBook.Add(new Employee(name, ["personal@example.com", "work@example.com"])));
        Assert.HasCount(2, contactBook.GetAll());
    }

    [TestMethod]
    public void SaveToFile_MultipleEmails_WritesArray()
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"contacts-{Guid.NewGuid():N}.json");
        ContactBook contactBook = new();
        contactBook.Add(new Student(new FullName("Alice", "Smith"), ["work@example.com", "personal@example.com"]));

        try
        {
            contactBook.SaveToFile(filePath);
            using JsonDocument json = JsonDocument.Parse(File.ReadAllText(filePath));
            JsonElement entry = json.RootElement[0];
            JsonElement emails = entry.GetProperty(nameof(IContact.Emails));

            Assert.AreEqual(JsonValueKind.Array, emails.ValueKind);
            Assert.AreEqual(2, emails.GetArrayLength());
            Assert.IsFalse(entry.TryGetProperty("Email", out _));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    private sealed record UnsupportedContact()
        : Contact(new FullName("Unsupported", "Contact"),
            emails: ["unsupported@example.com"]);
}
