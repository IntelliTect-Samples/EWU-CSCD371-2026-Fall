using Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    [TestMethod]
    public void Add_DuplicateStudent_ReturnsFalse()
    {
        ContactBook book = new();

        Student student1 = new(
            new FullName("Harry", "Potter"),
            "harry@hogwarts.edu",
            "Defense Against the Dark Arts");

        Student student2 = new(
            new FullName("Harry", "Potter"),
            "harry@hogwarts.edu",
            "Defense Against the Dark Arts");

        Assert.IsTrue(book.Add(student1));
        Assert.IsFalse(book.Add(student2));
        Assert.HasCount(1, book.GetAll());
    }


    [TestMethod]
    public void Add_StudentAndEmployee_BothStored()
    {
        ContactBook book = new();
        FullName name = new("Harry", "Potter");

        Student student = new(name, "harry@hogwarts.edu");
        Employee employee = new(name, "harry@hogwarts.edu");

        Assert.IsTrue(book.Add(student));
        Assert.IsTrue(book.Add(employee));
        Assert.HasCount(2, book.GetAll());
    }


    [TestMethod]
    public void Contains_EqualDifferentInstance_ReturnsTrue()
    {
        ContactBook book = new();
        FullName name = new("Harry", "Potter");

        Student original = new(name, "harry@hogwarts.edu", "Magic");
        Student lookup = new(name, "harry@hogwarts.edu", "Magic");

        book.Add(original);

        Assert.AreNotEqual(original.Id, lookup.Id);
        Assert.IsTrue(book.Contains(lookup));
    }


    [TestMethod]
    public void Remove_EqualDifferentInstance_ReturnsTrue()
    {
        ContactBook book = new();
        FullName name = new("Harry", "Potter");

        Student original = new(name, "harry@hogwarts.edu", "Magic");
        Student removal = new(name, "harry@hogwarts.edu", "Magic");

        book.Add(original);

        Assert.IsTrue(book.Remove(removal));
        Assert.IsFalse(book.Contains(original));
        Assert.HasCount(0, book.GetAll());
    }
}
