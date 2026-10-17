
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contacts;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    private string _testDirectory = string.Empty;

    private string FilePath =>
        Path.Combine(_testDirectory, "contacts.json");

    [TestInitialize]
    public void SetUp()
    {
        _testDirectory = Path.Combine(
            Path.GetTempPath(),
            $"Contacts.Tests.{Guid.NewGuid():N}");

        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void TearDown()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, recursive: true);
    }

    private static FullName CreateName() =>
        new("Alex", "Smith", null);

    private static string[] CreateEmails() =>
        ["alex@example.com", "alex@work.example"];

    private static Student CreateStudent() =>
        new(
            CreateName(),
            "Computer Science",
            "2028",
            CreateEmails());

    private static Employee CreateEmployee() =>
        new(
            CreateName(),
            "IT",
            "Example Corp",
            CreateEmails());

    [TestMethod]
    public void Add_RejectsDuplicateContact()
    {
        var book = new ContactBook();
        var first = CreateStudent();
        var duplicate = CreateStudent();

        Assert.IsTrue(book.Add(first));
        Assert.IsFalse(book.Add(duplicate));
        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    public void Add_AllowsStudentAndEmployeeWithSameNameAndEmails()
    {
        var book = new ContactBook();

        var student = CreateStudent();
        var employee = CreateEmployee();

        Assert.AreEqual(student.name, employee.name);
        CollectionAssert.AreEqual(
            student.Emails.ToArray(),
            employee.Emails.ToArray());

        Assert.IsTrue(book.Add(student));
        Assert.IsTrue(book.Add(employee));
        Assert.AreEqual(2, book.GetAll().Length);
    }

    [TestMethod]
    public void ContainsAndRemove_WorkWithEqualDifferentInstance()
    {
        var book = new ContactBook();
        var stored = CreateStudent();
        var equivalent = CreateStudent();

        Assert.AreNotEqual(stored.ID, equivalent.ID);
        Assert.IsTrue(book.Add(stored));

        Assert.IsTrue(book.Contains(equivalent));
        Assert.IsTrue(book.Remove(equivalent));

        Assert.IsFalse(book.Contains(stored));
        Assert.IsFalse(book.Remove(equivalent));
        Assert.AreEqual(0, book.GetAll().Length);
    }

    [TestMethod]
    public void Contains_ReturnsFalseForMissingContact()
    {
        var book = new ContactBook();

        Assert.IsFalse(book.Contains(CreateStudent()));
    }

    [TestMethod]
    public void Load_WhenFileDoesNotExist_LeavesBookEmpty()
    {
        var book = new ContactBook(FilePath);

        book.Load();

        Assert.AreEqual(0, book.GetAll().Length);
    }

    [TestMethod]
    public void Load_WhenJsonIsNull_LeavesBookEmpty()
    {
        File.WriteAllText(FilePath, "null");

        var book = new ContactBook(FilePath);
        book.Load();

        Assert.AreEqual(0, book.GetAll().Length);
    }

    [TestMethod]
    public void SaveAndLoad_RoundTripsStudentAndEmployee()
    {
        var originalBook = new ContactBook(FilePath);
        var student = CreateStudent();
        var employee = CreateEmployee();

        Assert.IsTrue(originalBook.Add(student));
        Assert.IsTrue(originalBook.Add(employee));

        originalBook.Save();

        Assert.IsTrue(File.Exists(FilePath));

        var loadedBook = new ContactBook(FilePath);
        loadedBook.Load();

        var contacts = loadedBook.GetAll();

        Assert.AreEqual(2, contacts.Length);

        var loadedStudent = contacts.OfType<Student>().Single();
        var loadedEmployee = contacts.OfType<Employee>().Single();

        Assert.AreEqual(student.ID, loadedStudent.ID);
        Assert.AreEqual(student.name, loadedStudent.name);
        Assert.AreEqual(student.Major, loadedStudent.Major);
        Assert.AreEqual(
            student.GraduationYear,
            loadedStudent.GraduationYear);

        CollectionAssert.AreEqual(
            student.Emails.ToArray(),
            loadedStudent.Emails.ToArray());

        Assert.AreEqual(employee.ID, loadedEmployee.ID);
        Assert.AreEqual(employee.name, loadedEmployee.name);
        Assert.AreEqual(employee.Department, loadedEmployee.Department);
        Assert.AreEqual(employee.CompanyName, loadedEmployee.CompanyName);

        CollectionAssert.AreEqual(
            employee.Emails.ToArray(),
            loadedEmployee.Emails.ToArray());
    }

    [TestMethod]
    public void Load_DoesNotAddDuplicateContactsFromFile()
    {
        var first = CreateStudent();
        var duplicate = CreateStudent();

        var json = JsonSerializer.Serialize<IContact[]>(
            [first, duplicate],
            JsonOptionGiver.Options);

        File.WriteAllText(FilePath, json);

        var book = new ContactBook(FilePath);
        book.Load();

        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    public void Load_AdvancesNextIdBeyondHighestPersistedId()
    {
        var seed = CreateStudent();
        var highestId = seed.ID + 10_000;
        var savedContact = seed with { ID = highestId };

        var json = JsonSerializer.Serialize<IContact[]>(
            [savedContact],
            JsonOptionGiver.Options);

        File.WriteAllText(FilePath, json);

        var book = new ContactBook(FilePath);
        book.Load();

        var newContact = CreateEmployee();

        Assert.IsTrue(newContact.ID > highestId);
    }
}
