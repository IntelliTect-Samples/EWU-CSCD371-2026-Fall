using System;
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
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Student duplicateStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

        Assert.IsTrue(contactBook.Add(storedStudent));
        Assert.IsFalse(contactBook.Add(duplicateStudent));
        Assert.ContainsSingle(contactBook.GetAll());
    }

    [TestMethod]
    public void Add_StudentAndEmployeeWithSameData_StoresBoth()
    {
        ContactBook contactBook = new();
        Student student = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Employee employee = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

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
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Student matchingStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

        contactBook.Add(storedStudent);

        Assert.IsTrue(contactBook.Contains(matchingStudent));
    }

    [TestMethod]
    public void Contains_AbsentContact_ReturnsFalse()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Student absentStudent = new(new FullName("ValidFirst", "ValidLast"), "DifferentEmail");

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
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Student matchingStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

        contactBook.Add(storedStudent);

        Assert.IsTrue(contactBook.Remove(matchingStudent));
        Assert.IsEmpty(contactBook.GetAll());
    }

    [TestMethod]
    public void Remove_AbsentContact_ReturnsFalseAndPreservesStoredContact()
    {
        ContactBook contactBook = new();
        Student storedStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Student absentStudent = new(new FullName("ValidFirst", "ValidLast"), "DifferentEmail");

        contactBook.Add(storedStudent);

        Assert.IsFalse(contactBook.Remove(absentStudent));
        IContact remainingContact = Assert.ContainsSingle(contactBook.GetAll());
        Assert.AreSame(storedStudent, remainingContact);
    }
}
