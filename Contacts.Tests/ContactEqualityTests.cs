using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public sealed class ContactEqualityTests
{
    [TestMethod]
    public void Id_IsDifferentForEachInstance()
    {
        Assert.AreNotEqual(TestContacts.MakeStudent().Id, TestContacts.MakeStudent().Id);
    }

    [TestMethod]
    public void Students_SameData_AreEqualDespiteDifferentIds()
    {
        Student a = TestContacts.MakeStudent();
        Student b = TestContacts.MakeStudent();

        Assert.AreNotEqual(a.Id, b.Id);
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Employees_SameData_AreEqualDespiteDifferentIds()
    {
        Employee a = TestContacts.MakeEmployee();
        Employee b = TestContacts.MakeEmployee();

        Assert.AreNotEqual(a.Id, b.Id);
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Students_EqualThroughIContact()
    {
        IContact a = TestContacts.MakeStudent();
        IContact b = TestContacts.MakeStudent();

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Students_DifferentName_AreNotEqual()
    {
        Assert.AreNotEqual(TestContacts.MakeStudent(), TestContacts.MakeStudent(first: "Grace"));
    }

    [TestMethod]
    public void Students_DifferentEmail_AreNotEqual()
    {
        Assert.AreNotEqual(TestContacts.MakeStudent(), TestContacts.MakeStudent(email: "other@example.com"));
    }

    [TestMethod]
    public void Students_DifferentSchool_AreNotEqual()
    {
        Assert.AreNotEqual(TestContacts.MakeStudent(), TestContacts.MakeStudent(school: "UW"));
    }

    [TestMethod]
    public void Employees_DifferentCompany_AreNotEqual()
    {
        Assert.AreNotEqual(TestContacts.MakeEmployee(), TestContacts.MakeEmployee(company: "Acme"));
    }

    [TestMethod]
    public void Employees_DifferentTitle_AreNotEqual()
    {
        Assert.AreNotEqual(TestContacts.MakeEmployee(), TestContacts.MakeEmployee(title: "Manager"));
    }

    [TestMethod]
    public void StudentAndEmployee_OtherwiseIdenticalData_AreNotEqual()
    {
        IContact student = TestContacts.MakeStudent(school: "Acme");
        IContact employee = TestContacts.MakeEmployee(company: "Acme", title: "Acme");

        Assert.AreNotEqual(student, employee);
        Assert.AreNotEqual(employee, student);
    }

    [TestMethod]
    public void Equals_Null_IsFalse()
    {
        Assert.IsFalse(TestContacts.MakeStudent().Equals(null));
    }

    [TestMethod]
    public void Constructor_NullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new Student(null!, "ada@example.com", "EWU"));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("not-an-email")]
    public void Constructor_InvalidEmail_Throws(string email)
    {
        Assert.Throws<ArgumentException>(() => _ = TestContacts.MakeStudent(email: email));
    }

    [TestMethod]
    public void Constructor_NullEmail_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new Student(new FullName("Ada", "Lovelace"), null!, "EWU"));
    }

    [TestMethod]
    public void Constructor_BlankSchool_Throws()
    {
        Assert.Throws<ArgumentException>(() => _ = TestContacts.MakeStudent(school: " "));
    }

    [TestMethod]
    public void Constructor_BlankCompanyOrTitle_Throws()
    {
        Assert.Throws<ArgumentException>(() => _ = TestContacts.MakeEmployee(company: ""));
        Assert.Throws<ArgumentException>(() => _ = TestContacts.MakeEmployee(title: ""));
    }

    [TestMethod]
    public void Describe_ShowsKindNameEmailAndExtraData()
    {
        Assert.AreEqual(
            "Student: Ada Lovelace <ada@example.com> - EWU",
            TestContacts.MakeStudent().Describe());
        Assert.AreEqual(
            "Employee: Ada Lovelace <ada@example.com> - Engineer at EWU",
            TestContacts.MakeEmployee().Describe());
    }
}
