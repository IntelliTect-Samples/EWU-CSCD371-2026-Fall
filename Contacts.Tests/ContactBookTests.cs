using System.Reflection;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    private static FullName Ada => new("Ada", "Lovelace");

    [TestMethod]
    public void Add_EqualContactWithDifferentId_IsRejected()
    {
        ContactBook book = new();

        bool firstAdded = book.Add(new Student(Ada, ["ada@example.com"], "CS"));
        bool secondAdded = book.Add(new Student(Ada, ["ada@example.com"], "CS"));

        Assert.IsTrue(firstAdded);
        Assert.IsFalse(secondAdded);
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameData_BothAreStored()
    {
        ContactBook book = new();

        bool studentAdded = book.Add(new Student(Ada, ["ada@example.com"], "Engineer"));
        bool employeeAdded = book.Add(new Employee(Ada, ["ada@example.com"], "Engineer"));

        Assert.IsTrue(studentAdded);
        Assert.IsTrue(employeeAdded);
        Assert.HasCount(2, book.GetAll());
    }

    [TestMethod]
    public void Contains_EqualButDifferentInstance_ReturnsTrue()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, ["ada@example.com"], "CS"));

        Assert.IsTrue(book.Contains(new Student(Ada, ["ada@example.com"], "CS")));
    }

    [TestMethod]
    public void Remove_EqualButDifferentInstance_RemovesTheStoredContact()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, ["ada@example.com"], "CS"));

        bool removed = book.Remove(new Student(Ada, ["ada@example.com"], "CS"));

        Assert.IsTrue(removed);
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Remove_ContactNotInBook_ReturnsFalse()
    {
        ContactBook book = new();

        Assert.IsFalse(book.Remove(new Student(Ada, ["ada@example.com"], "CS")));
    }

    [TestMethod]
    public void Entities_IsHashSetOfIContact()
    {
        PropertyInfo? entities = typeof(ContactBook).GetProperty("Entities", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(entities);
        Assert.AreEqual(typeof(HashSet<IContact>), entities.PropertyType);
    }

    [TestMethod]
    public void GetAll_NewBook_IsEmpty()
    {
        Assert.IsEmpty(new ContactBook().GetAll());
    }

    [TestMethod]
    public void Contains_ContactNotInBook_ReturnsFalse()
    {
        Assert.IsFalse(new ContactBook().Contains(new Student(Ada, ["ada@example.com"], "CS")));
    }

    [TestMethod]
    public void Add_EmailsDifferingOnlyInCaseAndOrder_IsRejected()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, ["ada@example.com", "ada@ewu.edu"], "CS"));

        Assert.IsFalse(book.Add(new Student(Ada, ["ADA@ewu.edu", "Ada@Example.com"], "CS")));
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Replace_ExistingContact_ReplacesIt()
    {
        ContactBook book = new();
        Student original = new(Ada, ["ada@example.com"], "CS");
        Student updated = new(Ada, ["ada@example.com"], "Math");
        book.Add(original);

        bool replaced = book.Replace(original, updated);

        Assert.IsTrue(replaced);
        Assert.IsFalse(book.Contains(original));
        Assert.IsTrue(book.Contains(updated));
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Replace_WithEqualContact_Succeeds()
    {
        ContactBook book = new();
        Student original = new(Ada, ["ada@example.com"], "CS");
        book.Add(original);

        Assert.IsTrue(book.Replace(original, new Student(Ada, ["ada@example.com"], "CS")));
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Replace_IntoDuplicateOfAnotherContact_KeepsOriginal()
    {
        ContactBook book = new();
        Student original = new(Ada, ["ada@example.com"], "CS");
        Student other = new(new FullName("Grace", "Hopper"), ["grace@example.com"], "CS");
        book.Add(original);
        book.Add(other);

        bool replaced = book.Replace(original, other with { });

        Assert.IsFalse(replaced);
        Assert.IsTrue(book.Contains(original));
        Assert.IsTrue(book.Contains(other));
        Assert.HasCount(2, book.GetAll());
    }

    [TestMethod]
    public void Replace_ContactNotInBook_ReturnsFalseAndAddsNothing()
    {
        ContactBook book = new();

        bool replaced = book.Replace(
            new Student(Ada, ["ada@example.com"], "CS"),
            new Student(Ada, ["ada@example.com"], "Math"));

        Assert.IsFalse(replaced);
        Assert.IsEmpty(book.GetAll());
    }
}
