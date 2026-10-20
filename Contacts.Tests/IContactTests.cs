using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

///summary
/// This is a test class for the IContact interface in the Contacts application.
///summary
[TestClass]
public class IContactTests
{
    // shared data and helpers to prevent accidental data mismatch from influencing test results
    private static readonly FullName TestName = new("John", "Smith");
    private const string TestEmail = "email";
    
    private static Student CreateStudent() => new(TestName, TestEmail);
    private static Employee CreateEmployee() => new(TestName, TestEmail);
    
    // Req: Two contacts with the same data are equal even though they have different ID's
    [TestMethod]
    public void Students_WithSameData_AreEqual()
    {
        Student student1 = CreateStudent();
        Student student2 = CreateStudent();
        
        Assert.AreEqual(student1, student2);
    }
    
    [TestMethod]
    public void Students_WithSameData_HaveDifferentIds()
    {
        Student student1 = CreateStudent();
        Student student2 = CreateStudent();
        
        Assert.AreNotEqual(student1.Id, student2.Id);
    }
    
    // Req: Equal contacts must produce the same HashCode for HashSet to reliably determine duplicates
    [TestMethod]
    public void Students_WithSameData_HaveSameHashCode()
    {
        Student student1 = CreateStudent();
        Student student2 = CreateStudent();

        Assert.AreEqual(
            student1.GetHashCode(),
            student2.GetHashCode());
    }
    
    // REQ: a student and an employee with otherwise identical data are not equal
    [TestMethod]
    public void StudentAndEmployee_WithSameData_AreNotEqual()
    {
        IContact student = CreateStudent();
        IContact employee = CreateEmployee();

        Assert.AreNotEqual(student, employee);
    }
}