using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactTests
{
    // shared data and helpers to prevent accidental data mismatch from influencing test results
    private static readonly FullName TestName = new("John", "Smith");
    private const string TestEmail = "email";
    private static readonly ContactEmails TestContactEmails = new([TestEmail]);
    private static readonly ContactId TestId = new('T', 1);
    
    private static Student CreateStudent() => new(TestName, TestContactEmails) { Id = TestId };
    // used to show all data but ID is same
    private static Student CreateStudent(ContactId id) => new(TestName, TestContactEmails) { Id = id };
    private static Employee CreateEmployee() => new(TestName, TestContactEmails) { Id = TestId };
    
    
     [TestMethod]
     public void Students_WithSameData_HaveDifferentIds()
     {
         Student student1 = CreateStudent(new ContactId('S', 1));
         Student student2 = CreateStudent(new ContactId('S', 2));
         
         Assert.AreNotEqual(student1.Id, student2.Id);
     }
    
     [TestMethod]
     public void Students_WithSameData_AreEqual()
     {
         Student student1 = CreateStudent();
         Student student2 = CreateStudent();
         
         Assert.AreEqual(student1, student2);
     }
    
     [TestMethod]
     public void Students_WithSameData_HaveSameHashCode()
     {
         Student student1 = CreateStudent();
         Student student2 = CreateStudent();
    
         Assert.AreEqual(
             student1.GetHashCode(),
             student2.GetHashCode());
     }
    
     [TestMethod]
     public void Student_And_Employee_WithSameData_AreNotEqual()
     {
         IContact student = CreateStudent();
         IContact employee = CreateEmployee();
    
         Assert.AreNotEqual(student, employee);
     }
}