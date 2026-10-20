using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    // shared data and helpers to prevent accidental data mismatch from influencing test results
    private static readonly FullName TestName = new("John", "Smith");
    private static readonly ContactEmails TestContactEmails = new(["test@email"]);
    private static readonly ContactId TestId = new('T', 1);
    
    private static Student CreateStudent() => new(TestName, TestContactEmails) { Id = TestId };
    private static Employee CreateEmployee() => new(TestName, TestContactEmails) { Id = TestId };
    

    /// <summary>
    /// Tests that adding a student to the contact book successfully stores the student.
    /// </summary>
    [TestMethod]
    public void ContactBook_AddStudent_AddsStudent()
    {
        ContactBook book = new();
    
        Student student = CreateStudent();
        book.Add(student);
        
        Assert.IsTrue(book.Contains(student));
    }
    

    /// <summary>
    /// Tests that the contact book correctly identifies a student with equal data as contained, even if it's a different reference.
    /// </summary>
    [TestMethod]
    public void Contains_EqualStudents_ReturnsTrue()
    {
        ContactBook book = new();
        
        Student stored = CreateStudent();
        Student lookup = CreateStudent();
        
        book.Add(stored);
        
        // validate stored and lookup are not same ref
        Assert.AreNotSame(stored, lookup);
        
        // equality wise the same however
        Assert.AreEqual(stored, lookup);
        
        // lookup by the not added ref should be true
        Assert.IsTrue(book.Contains(lookup));
    }
    

    /// <summary>
    /// Tests that adding a student and an employee with equal data to the contact book successfully stores both contacts, demonstrating that the contact book can handle different types with equal data.
    /// </summary>
    [TestMethod]
    public void ContactBookWithStudent_AddingEqualDataEmployee_StoresBothContacts()
    {
        ContactBook book = new();
        
        Student student = CreateStudent();
        Employee employee = CreateEmployee();
        
        book.Add(student);
        book.Add(employee);
        
        Assert.IsTrue(book.Contains(student));
        Assert.IsTrue(book.Contains(employee));
        Assert.HasCount(2, book.GetAll());
    }
    

    /// <summary>
    /// Tests that the contact book does not consider an employee with equal data to a contained student as equal, ensuring that the contact book maintains type distinction even when data is identical.
    /// </summary>
    [TestMethod]
    public void ContactBookWithStudent_ContainsWithEqualDataEmployee_ReturnsFalse()
    {
        ContactBook book = new();
        
        Student student = CreateStudent();
        Employee employee = CreateEmployee();
        
        book.Add(student);
        
        Assert.IsTrue(book.Contains(student));
        // collection behavior should not find an employee of equal data to a contained student as equal
        Assert.IsFalse(book.Contains(employee));
    }


    /// <summary>
    /// Tests that removing a student from the contact book successfully removes the student, and that the contact book no longer contains the student after removal.
    /// </summary>
    [TestMethod]
    public void ContactBook_RemoveStudent_RemovesStudent()
    {
        ContactBook book = new();
    
        Student student = CreateStudent();
        book.Add(student);
        
        Assert.IsTrue(book.Contains(student));
        
        book.Remove(student);
        
        Assert.IsFalse(book.Contains(student));
    }
}