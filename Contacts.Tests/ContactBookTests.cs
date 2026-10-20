using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public sealed class ContactBookTests
{
    // Test 1: Full names should be equal
    [TestMethod]
    public void FullNamesWithSameDataAreEqual()
    {
        FullName name1 = new("John", "Smith");
        FullName name2 = new("John", "Smith");

        Assert.AreEqual(name1, name2);
    }

    // Test 2: Different IDs should not affect equality
    [TestMethod]
    public void StudentsWithSameDataAreEqual()
    {
        Student student1 = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Student student2 = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Assert.AreEqual(student1, student2);
        Assert.AreEqual(
            student1.GetHashCode(),
            student2.GetHashCode());
    }

    // Test 3: Capitalization should not matter
    [TestMethod]
    public void ContactsIgnoreCapitalization()
    {
        ContactBook book = new();

        Student student1 = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Student student2 = new(
            new FullName("john", "smith"),
            "JOHN@EMAIL.COM",
            "computer science");

        Assert.IsTrue(book.Add(student1));
        Assert.IsFalse(book.Add(student2));
        Assert.AreEqual(1, book.GetAll().Length);
    }

    // Test 4: Different emails should not be equal
    [TestMethod]
    public void DifferentEmailsAreNotEqual()
    {
        Student student1 = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Student student2 = new(
            new FullName("John", "Smith"),
            "different@email.com",
            "Computer Science");

        Assert.AreNotEqual(student1, student2);
    }

    // Test 5: Student and employee are different types
    [TestMethod]
    public void StudentAndEmployeeCanBothBeStored()
    {
        ContactBook book = new();

        Student student = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Employee employee = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Assert.IsTrue(book.Add(student));
        Assert.IsTrue(book.Add(employee));
        Assert.AreEqual(2, book.GetAll().Length);
    }

    // Test 6: Contains should find an equal contact
    [TestMethod]
    public void ContainsFindsEqualContact()
    {
        ContactBook book = new();

        Student student1 = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Student student2 = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        book.Add(student1);

        Assert.IsTrue(book.Contains(student2));
    }

    // Test 7: Remove should work with an equal contact
    [TestMethod]
    public void RemoveWorksWithEqualContact()
    {
        ContactBook book = new();

        Employee employee1 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Sales");

        Employee employee2 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Sales");

        book.Add(employee1);

        Assert.IsTrue(book.Remove(employee2));
        Assert.AreEqual(0, book.GetAll().Length);
    }

    // Test 8: Removing a missing contact returns false
    [TestMethod]
    public void RemoveMissingContactReturnsFalse()
    {
        ContactBook book = new();

        Student student = new(
            new FullName("John", "Smith"),
            "john@email.com",
            "Computer Science");

        Assert.IsFalse(book.Remove(student));
    }

    // Test 9: Employees with matching data are equal
    [TestMethod]
    public void EmployeesWithSameDataAreEqual()
    {
        Employee employee1 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Sales");

        Employee employee2 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Sales");

        Assert.AreEqual(employee1, employee2);
        Assert.AreEqual(
            employee1.GetHashCode(),
            employee2.GetHashCode());
    }

    // Test 10: Employee capitalization should not matter
    [TestMethod]
    public void EmployeesIgnoreCapitalization()
    {
        ContactBook book = new();

        Employee employee1 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Sales");

        Employee employee2 = new(
            new FullName("jane", "doe"),
            "JANE@EMAIL.COM",
            "sales");

        Assert.IsTrue(book.Add(employee1));
        Assert.IsFalse(book.Add(employee2));
        Assert.AreEqual(1, book.GetAll().Length);
    }

    // Test 11: Different departments are not equal
    [TestMethod]
    public void DifferentDepartmentsAreNotEqual()
    {
        Employee employee1 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Sales");

        Employee employee2 = new(
            new FullName("Jane", "Doe"),
            "jane@email.com",
            "Marketing");

        Assert.AreNotEqual(employee1, employee2);
    }
}
