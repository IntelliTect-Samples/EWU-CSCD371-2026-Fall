using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public sealed class ContactBookTests
{
    [TestMethod]
    public void Entities_IsAHashSetOfIContact()
    {
        PropertyInfo? entities = typeof(ContactBook).GetProperty(
            "Entities", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.IsNotNull(entities);
        Assert.AreEqual(typeof(HashSet<IContact>), entities.PropertyType);
    }

    [TestMethod]
    public void Add_NewContact_ReturnsTrueAndStoresIt()
    {
        ContactBook book = new();

        bool added = book.Add(TestContacts.MakeStudent());

        Assert.IsTrue(added);
        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    public void Add_EqualContactWithDifferentId_ReturnsFalseAndDoesNotDuplicate()
    {
        ContactBook book = new();
        book.Add(TestContacts.MakeStudent());

        bool added = book.Add(TestContacts.MakeStudent());

        Assert.IsFalse(added);
        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    public void Add_SameInstanceTwice_ReturnsFalseSecondTime()
    {
        ContactBook book = new();
        Student student = TestContacts.MakeStudent();
        book.Add(student);

        Assert.IsFalse(book.Add(student));
        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameData_StoresBoth()
    {
        ContactBook book = new();

        bool studentAdded = book.Add(TestContacts.MakeStudent(school: "Acme"));
        bool employeeAdded = book.Add(TestContacts.MakeEmployee(company: "Acme", title: "Acme"));

        Assert.IsTrue(studentAdded);
        Assert.IsTrue(employeeAdded);
        Assert.AreEqual(2, book.GetAll().Length);
    }

    [TestMethod]
    public void Add_DifferentContacts_StoresAll()
    {
        ContactBook book = new();

        book.Add(TestContacts.MakeStudent(first: "Ada"));
        book.Add(TestContacts.MakeStudent(first: "Grace"));

        Assert.AreEqual(2, book.GetAll().Length);
    }

    [TestMethod]
    public void Add_Null_Throws()
    {
        ContactBook book = new();

        Assert.Throws<ArgumentNullException>(() => book.Add(null!));
    }

    [TestMethod]
    public void Contains_EqualButDifferentInstance_ReturnsTrue()
    {
        ContactBook book = new();
        Student stored = TestContacts.MakeStudent();
        Student lookup = TestContacts.MakeStudent();
        book.Add(stored);

        Assert.AreNotSame(stored, lookup);
        Assert.IsTrue(book.Contains(lookup));
    }

    [TestMethod]
    public void Contains_DifferentContact_ReturnsFalse()
    {
        ContactBook book = new();
        book.Add(TestContacts.MakeStudent());

        Assert.IsFalse(book.Contains(TestContacts.MakeStudent(first: "Grace")));
    }

    [TestMethod]
    public void Contains_EmployeeWhenOnlyEqualDataStudentStored_ReturnsFalse()
    {
        ContactBook book = new();
        book.Add(TestContacts.MakeStudent(school: "Acme"));

        Assert.IsFalse(book.Contains(TestContacts.MakeEmployee(company: "Acme", title: "Acme")));
    }

    [TestMethod]
    public void Contains_Null_Throws()
    {
        ContactBook book = new();

        Assert.Throws<ArgumentNullException>(() => book.Contains(null!));
    }

    [TestMethod]
    public void Remove_EqualButDifferentInstance_RemovesStoredContact()
    {
        ContactBook book = new();
        book.Add(TestContacts.MakeStudent());
        Student lookup = TestContacts.MakeStudent();

        bool removed = book.Remove(lookup);

        Assert.IsTrue(removed);
        Assert.AreEqual(0, book.GetAll().Length);
        Assert.IsFalse(book.Contains(lookup));
    }

    [TestMethod]
    public void Remove_ContactNotStored_ReturnsFalseAndKeepsOthers()
    {
        ContactBook book = new();
        book.Add(TestContacts.MakeStudent());

        bool removed = book.Remove(TestContacts.MakeStudent(first: "Grace"));

        Assert.IsFalse(removed);
        Assert.AreEqual(1, book.GetAll().Length);
    }

    [TestMethod]
    public void Remove_Student_DoesNotRemoveEmployeeWithSameData()
    {
        ContactBook book = new();
        book.Add(TestContacts.MakeStudent(school: "Acme"));
        book.Add(TestContacts.MakeEmployee(company: "Acme", title: "Acme"));

        book.Remove(TestContacts.MakeStudent(school: "Acme"));

        IContact[] remaining = book.GetAll();
        Assert.AreEqual(1, remaining.Length);
        Assert.IsInstanceOfType<Employee>(remaining[0]);
    }

    [TestMethod]
    public void Remove_Null_Throws()
    {
        ContactBook book = new();

        Assert.Throws<ArgumentNullException>(() => book.Remove(null!));
    }

    [TestMethod]
    public void GetAll_EmptyBook_ReturnsEmptyArray()
    {
        Assert.AreEqual(0, new ContactBook().GetAll().Length);
    }
}
