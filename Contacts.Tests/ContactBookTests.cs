using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    private static FullName Name(string first = "John", string last = "Doe") => new(first, last, null);
    private static Student S(int id, string first = "Daniel", string major = "CS") => new(id, Name(first), major);
    private static Employee E(int id, string first = "Greg", string dept = "EE") => new(id, Name(first), dept);

    [TestMethod]
    public void Add_NewContact_ReturnsTrueAndStores()
    {
        var book = new ContactBook();
        Assert.IsTrue(book.Add(S(1)));
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Add_Duplicate_DifferentId_IsRejectedAndNotStored()
    {
        var book = new ContactBook();
        book.Add(S(1));

        Assert.IsFalse(book.Add(S(2)));
        Assert.HasCount(1, book.GetAll());
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameData_BothStored()
    {
        var book = new ContactBook();

        Assert.IsTrue(book.Add(S(1, "Sam", "CS")));
        Assert.IsTrue(book.Add(E(2, "Sam", "CS")));
        Assert.HasCount(2, book.GetAll());
    }

    [TestMethod]
    public void Add_Null_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => { new ContactBook().Add(null!); });
    }

    [TestMethod]
    public void Contains_EqualButDifferentInstance_ReturnsTrue()
    {
        var book = new ContactBook();
        book.Add(S(1));

        Assert.IsTrue(book.Contains(S(99)));
    }

    [TestMethod]
    public void Remove_EqualButDifferentInstance_IsFoundAndRemoved()
    {
        var book = new ContactBook();
        book.Add(E(1, "Eve", "HR"));

        Assert.IsTrue(book.Remove(E(42, "Eve", "HR")));
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void Remove_OnlyRemovesMatchingType()
    {
        var book = new ContactBook();
        book.Add(S(1, "Sam", "CS"));
        book.Add(E(2, "Sam", "CS"));

        Assert.IsTrue(book.Remove(S(99, "Sam", "CS")));

        Assert.HasCount(1, book.GetAll());
        Assert.AreEqual(typeof(Employee), book.GetAll().Single().GetType());
    }

    [TestMethod]
    public void Remove_Missing_ReturnsFalse()
    {
        var book = new ContactBook();
        book.Add(S(1));

        Assert.IsFalse(book.Remove(S(2, "Nobody")));
    }

    [TestMethod]
    public void RemoveById_Existing_ReturnsTrueAndRemoves()
    {
        var book = new ContactBook();
        book.Add(S(1));

        Assert.IsTrue(book.RemoveById(1));
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void RemoveById_Missing_ReturnsFalse()
    {
        var book = new ContactBook();
        book.Add(S(1));

        Assert.IsFalse(book.RemoveById(99));
    }

    [TestMethod]
    public void RemoveById_EmptyBook_ReturnsFalse() =>
        Assert.IsFalse(new ContactBook().RemoveById(1));

    [TestMethod]
    public void GetAll_EmptyBook_ReturnsEmpty() =>
        Assert.IsEmpty(new ContactBook().GetAll());

    [TestMethod]
    public void IsEmpty_NewBook_IsTrue() =>
        Assert.IsTrue(new ContactBook().IsEmpty);

    [TestMethod]
    public void IsEmpty_AfterAdd_IsFalse()
    {
        var book = new ContactBook();
        book.Add(S(1));

        Assert.IsFalse(book.IsEmpty);
    }
}