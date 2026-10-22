using System;
using System.Collections.Generic;
using Contacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;
 
[TestClass]
public class ContactEqualityTests
{
    private static readonly FullName Inigo = new("Inigo", "Montoya");
 
    [TestMethod]
    public void Equals_SameDataDifferentId_AreEqual()
    {
        Student a = new(Inigo, "inigo@ewu.edu");
        Student b = new(Inigo, "inigo@ewu.edu");
 
        Assert.AreNotEqual(a.Id, b.Id);
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }
 
    [TestMethod]
    public void Equals_StudentAndEmployeeWithSameData_AreNotEqual()
    {
        IContact student = new Student(Inigo, "inigo@ewu.edu");
        IContact employee = new Employee(Inigo, "inigo@ewu.edu");
 
        Assert.AreNotEqual(student, employee);
    }
 
    [TestMethod]
    public void Equals_DifferentUniqueMember_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Student(Inigo, "inigo@ewu.edu", "CS"),
            new Student(Inigo, "inigo@ewu.edu", "Math"));
    }
 
    [TestMethod]
    public void HashSet_EqualButDifferentInstance_IsFound()
    {
        HashSet<IContact> set = [new Student(Inigo, "inigo@ewu.edu")];
 
        Assert.IsTrue(set.Contains(new Student(Inigo, "inigo@ewu.edu")));
        Assert.IsFalse(set.Add(new Student(Inigo, "inigo@ewu.edu")));
    }
 
    [TestMethod]
    public void FullName_WhitespaceMiddle_TreatedAsNoMiddle()
    {
        Assert.AreEqual(
        new FullName("Inigo", "Montoya"),
        new FullName("Inigo", "  ", "Montoya"));
    }
 
    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void FullName_BlankFirst_Throws(string first)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new FullName(first, "Montoya"));
    }
 
    [TestMethod]
    public void FullName_ToString_IncludesMiddleWhenPresent()
    {
        Assert.AreEqual("Inigo X Montoya", new FullName("Inigo", "X","Montoya").ToString());
        Assert.AreEqual("Inigo Montoya", Inigo.ToString());
    }
}
