namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    private static FullName Ada => new("Ada", "Lovelace");

    [TestMethod]
    public void Add_EqualContactWithDifferentId_IsRejected()
    {
        ContactBook book = new();

        bool firstAdded = book.Add(new Student(Ada, "ada@example.com", "CS"));
        bool secondAdded = book.Add(new Student(Ada, "ada@example.com", "CS"));

        Assert.IsTrue(firstAdded);
        Assert.IsFalse(secondAdded);
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameData_BothAreStored()
    {
        ContactBook book = new();

        bool studentAdded = book.Add(new Student(Ada, "ada@example.com", "Engineer"));
        bool employeeAdded = book.Add(new Employee(Ada, "ada@example.com", "Engineer"));

        Assert.IsTrue(studentAdded);
        Assert.IsTrue(employeeAdded);
        Assert.HasCount(2, book.GetAll());
    }

    [TestMethod]
    public void Contains_EqualButDifferentInstance_ReturnsTrue()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, "ada@example.com", "CS"));

        Assert.IsTrue(book.Contains(new Student(Ada, "ada@example.com", "CS")));
    }

    [TestMethod]
    public void Remove_EqualButDifferentInstance_RemovesTheStoredContact()
    {
        ContactBook book = new();
        book.Add(new Student(Ada, "ada@example.com", "CS"));

        bool removed = book.Remove(new Student(Ada, "ada@example.com", "CS"));

        Assert.IsTrue(removed);
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Remove_ContactNotInBook_ReturnsFalse()
    {
        ContactBook book = new();

        Assert.IsFalse(book.Remove(new Student(Ada, "ada@example.com", "CS")));
    }
}
