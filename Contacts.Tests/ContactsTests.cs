using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace Contacts.Tests;

[TestClass]
public sealed class ContactsTests
{
    // Helpers

    private static FullName CreateName() => new("Prince", "Menethil", "Arthas");

    private static Student CreateStudent() =>
        new(CreateName(), "Prince@example.com", "Lordaeron", "Princeling", "123-456-1337");

    private static Employee CreateEmployee() =>
        new(CreateName(), "Prince@example.com", "Silver Hand", "Paladin", "123-456-1337");

    // ContactBook -------------------------------------------------------------------------------

    [TestMethod]
    public void ContactBookNewBookIsEmpty()
    {
        var book = new ContactBook();

        Assert.AreEqual(0, book.Count);
        Assert.IsEmpty(book.GetAll());
    }

    [TestMethod]
    public void ContactBookAddNewContactReturnsTrue()
    {
        var book = new ContactBook();

        Assert.IsTrue(book.Add(CreateStudent()));
        Assert.AreEqual(1, book.Count);
    }

    [TestMethod]
    public void ContactBookAddDuplicateIsRejected()
    {
        var book = new ContactBook();
        book.Add(CreateStudent());

        bool added = book.Add(CreateStudent());

        Assert.IsFalse(added);
        Assert.AreEqual(1, book.Count);
    }

    [TestMethod]
    public void ContactBookAddSameInstanceTwiceIsRejected()
    {
        var book = new ContactBook();
        var student = CreateStudent();
        book.Add(student);

        Assert.IsFalse(book.Add(student));
        Assert.AreEqual(1, book.Count);
    }

    [TestMethod]
    public void ContactBookStudentAndEmployeeWithSameDataAreBothStored()
    {
        var book = new ContactBook();

        Assert.IsTrue(book.Add(CreateStudent()));
        Assert.IsTrue(book.Add(CreateEmployee()));
        Assert.AreEqual(2, book.Count);
    }

    [TestMethod]
    public void ContactBookContainsFindsEqualButDifferentInstance()
    {
        var book = new ContactBook();
        var stored = CreateStudent();
        book.Add(stored);
        var lookup = CreateStudent();

        Assert.AreNotEqual(stored.Id, lookup.Id);
        Assert.IsTrue(book.Contains(lookup));
    }

    [TestMethod]
    public void ContactBookContainsReturnsFalseForMissingContact()
    {
        var book = new ContactBook();
        book.Add(CreateStudent());

        Assert.IsFalse(book.Contains(CreateEmployee()));
    }

    [TestMethod]
    public void ContactBookRemoveEqualButDifferentInstanceRemovesStoredContact()
    {
        var book = new ContactBook();
        var stored = CreateStudent();
        book.Add(stored);
        var toRemove = CreateStudent();

        Assert.AreNotEqual(stored.Id, toRemove.Id);
        Assert.IsTrue(book.Remove(toRemove));
        Assert.IsFalse(book.Contains(stored));
        Assert.AreEqual(0, book.Count);
    }

    [TestMethod]
    public void ContactBookRemoveMissingContactReturnsFalse()
    {
        var book = new ContactBook();
        book.Add(CreateStudent());

        Assert.IsFalse(book.Remove(CreateEmployee()));
        Assert.AreEqual(1, book.Count);
    }

    [TestMethod]
    public void ContactBookRemoveOnlyRemovesMatchingType()
    {
        var book = new ContactBook();
        book.Add(CreateStudent());
        book.Add(CreateEmployee());

        Assert.IsTrue(book.Remove(CreateStudent()));
        Assert.IsFalse(book.Contains(CreateStudent()));
        Assert.IsTrue(book.Contains(CreateEmployee()));
    }

    [TestMethod]
    public void ContactBookGetAllReturnsStoredContacts()
    {
        var book = new ContactBook();
        var student = CreateStudent();
        var employee = CreateEmployee();
        book.Add(student);
        book.Add(employee);

        IContact[] all = book.GetAll();

        Assert.HasCount(2, all);
        CollectionAssert.Contains(all, student);
        CollectionAssert.Contains(all, employee);
    }

    [TestMethod]
    public void ContactBookAddNullThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new ContactBook().Add(null!));

    [TestMethod]
    public void ContactBookRemoveNullThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new ContactBook().Remove(null!));

    [TestMethod]
    public void ContactBookContainsNullThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new ContactBook().Contains(null!));

    // Employee-------------------------------------------------------------------------------

    [TestMethod]
    public void EmployeeConstructorSetsProperties()
    {
        var employee = new Employee(CreateName(), "Prince@example.com", "Silver Hand", "Paladin", "123-456-1337");

        Assert.AreEqual(CreateName(), employee.Name);
        Assert.AreEqual("Prince@example.com", employee.Email);
        Assert.AreEqual("123-456-1337", employee.Phone);
        Assert.AreEqual("Silver Hand", employee.Company);
        Assert.AreEqual("Paladin", employee.JobTitle);
    }

    [TestMethod]
    public void EmployeeConstructorTrimsValues()
    {
        var employee = new Employee(CreateName(), "Prince@example.com", " Silver Hand ", " Paladin ");

        Assert.AreEqual("Silver Hand", employee.Company);
        Assert.AreEqual("Paladin", employee.JobTitle);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void EmployeeBlankCompanyThrows(string company) =>
        Assert.ThrowsExactly<ArgumentException>(() => new Employee(CreateName(), "Prince@example.com", company, "Paladin"));

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void EmployeeBlankJobTitleThrows(string jobTitle) =>
        Assert.ThrowsExactly<ArgumentException>(() => new Employee(CreateName(), "Prince@example.com", "Silver Hand", jobTitle));

    [TestMethod]
    public void EmployeesWithSameDataAndDifferentIdsAreEqual()
    {
        var a = CreateEmployee();
        var b = CreateEmployee();

        Assert.AreNotEqual(a.Id, b.Id);
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void EmployeesWithDifferentCompaniesAreNotEqual()
    {
        var a = CreateEmployee();
        var b = new Employee(CreateName(), "Prince@example.com", "Globex", "Paladin", "123-456-1337");

        Assert.AreNotEqual(a, b);
    }

    [TestMethod]
    public void EmployeesWithDifferentJobTitlesAreNotEqual()
    {
        var a = CreateEmployee();
        var b = new Employee(CreateName(), "Prince@example.com", "Silver Hand", "Manager", "123-456-1337");

        Assert.AreNotEqual(a, b);
    }

    // FullName -------------------------------------------------------------------------------

    [TestMethod]
    public void FullNameConstructorSetsProperties()
    {
        var name = new FullName("Prince", "Menethil", "Arthas");

        Assert.AreEqual("Prince", name.firstName);
        Assert.AreEqual("Menethil", name.lastName);
        Assert.AreEqual("Arthas", name.middleName);
    }

    [TestMethod]
    public void FullNameMiddleDefaultsToNull()
    {
        var name = new FullName("Prince", "Menethil");

        Assert.IsNull(name.middleName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void FullNameBlankMiddleBecomesNull(string middle)
    {
        var name = new FullName("Prince", "Menethil", middle);

        Assert.IsNull(name.middleName);
    }

    [TestMethod]
    public void FullNameConstructorTrimsWhitespace()
    {
        var name = new FullName("  Prince ", " Menethil  ", " Arthas ");

        Assert.AreEqual("Prince", name.firstName);
        Assert.AreEqual("Menethil", name.lastName);
        Assert.AreEqual("Arthas", name.middleName);
    }

    [TestMethod]
    public void FullNameNullFirstThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new FullName(null!, "Menethil"));

    [TestMethod]
    public void FullNameNullLastThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new FullName("Prince", null!));

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void FullNameBlankFirstThrows(string first) =>
        Assert.ThrowsExactly<ArgumentException>(() => new FullName(first, "Menethil"));

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void FullNameBlankLastThrows(string last) =>
        Assert.ThrowsExactly<ArgumentException>(() => new FullName("Prince", last));

    [TestMethod]
    public void FullNamesWithSameValuesAreEqual()
    {
        var a = new FullName("Prince", "Menethil", "Arthas");
        var b = new FullName("Prince", "Menethil", "Arthas");

        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void FullNameBlankAndMissingMiddleAreEqual() =>
        Assert.AreEqual(new FullName("Prince", "Menethil", " "), new FullName("Prince", "Menethil"));

    [TestMethod]
    public void FullNamesWithDifferentMiddleAreNotEqual() =>
        Assert.AreNotEqual(new FullName("Prince", "Menethil", "Arthas"), new FullName("Prince", "Menethil"));

    [TestMethod]
    public void FullNameToStringWithoutMiddle() =>
        Assert.AreEqual("Prince Menethil", new FullName("Prince", "Menethil").ToString());

    [TestMethod]
    public void FullNameToStringWithMiddle() =>
        Assert.AreEqual("Prince Arthas Menethil", new FullName("Prince", "Menethil", "Arthas").ToString());

    // IContact -------------------------------------------------------------------------------

    [TestMethod]
    public void ContactConstructorTrimsEmailAndPhone()
    {
        var student = new Student(CreateName(), " Prince@example.com ", "Lordaeron", "Princeling", " 123-456-1337 ");

        Assert.AreEqual("Prince@example.com", student.Email);
        Assert.AreEqual("123-456-1337", student.Phone);
    }

    [TestMethod]
    public void ContactPhoneDefaultsToNull()
    {
        var student = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Princeling");

        Assert.IsNull(student.Phone);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void ContactBlankPhoneBecomesNull(string phone)
    {
        var student = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Princeling", phone);

        Assert.IsNull(student.Phone);
    }

    [TestMethod]
    public void ContactEachInstanceGetsNewId()
    {
        var a = CreateStudent();
        var b = CreateStudent();

        Assert.AreNotEqual(Guid.Empty, a.Id);
        Assert.AreNotEqual(a.Id, b.Id);
    }

    [TestMethod]
    public void ContactNullNameThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new Student(null!, "Prince@example.com", "Lordaeron", "Princeling"));

    [TestMethod]
    public void ContactNullEmailThrows() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => new Student(CreateName(), null!, "Lordaeron", "Princeling"));

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void ContactBlankEmailThrows(string email) =>
        Assert.ThrowsExactly<ArgumentException>(() => new Employee(CreateName(), email, "Silver Hand", "Paladin"));

    [TestMethod]
    public void ContactEqualityHoldsWhenComparedAsIContact()
    {
        IContact a = CreateStudent();
        IContact b = CreateStudent();

        Assert.IsTrue(a.Equals(b));
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void ContactBlankAndMissingPhoneAreEqual()
    {
        var a = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Princeling", " ");
        var b = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Princeling");

        Assert.AreEqual(a, b);
    }

    [TestMethod]
    public void ContactsWithDifferentNamesAreNotEqual()
    {
        var a = CreateStudent();
        var b = new Student(new FullName("Jaina", "Proudmoore"), "Prince@example.com", "Lordaeron", "Princeling", "123-456-1337");

        Assert.AreNotEqual(a, b);
    }

    [TestMethod]
    public void ContactsWithDifferentEmailsAreNotEqual()
    {
        var a = CreateStudent();
        var b = new Student(CreateName(), "other@example.com", "Lordaeron", "Princeling", "123-456-1337");
        
        Assert.AreNotEqual(a, b);
    }

    [TestMethod]
    public void ContactsWithDifferentPhonesAreNotEqual()
    {
        var a = CreateStudent();
        var b = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Princeling");

        Assert.AreNotEqual(a, b);
    }

    [TestMethod]
    public void ContactIsNotEqualToNull()
    {
        var student = CreateStudent();

        Assert.IsFalse(student.Equals(null));
    }

    [TestMethod]
    public void ContactStudentAndEmployeeWithSameSharedDataAreNotEqual()
    {
        IContact student = CreateStudent();
        IContact employee = CreateEmployee();

        Assert.IsFalse(student.Equals(employee));
        Assert.IsFalse(employee.Equals(student));
    }

    // Student -------------------------------------------------------------------------------

    [TestMethod]
    public void StudentConstructorSetsProperties()
    {
        var student = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Princeling", "123-456-1337");

        Assert.AreEqual(CreateName(), student.Name);
        Assert.AreEqual("Prince@example.com", student.Email);
        Assert.AreEqual("123-456-1337", student.Phone);
        Assert.AreEqual("Lordaeron", student.School);
        Assert.AreEqual("Princeling", student.Major);
    }

    [TestMethod]
    public void StudentConstructorTrimsValues()
    {
        var student = new Student(CreateName(), "Prince@example.com", " Lordaeron ", " Princeling ");

        Assert.AreEqual("Lordaeron", student.School);
        Assert.AreEqual("Princeling", student.Major);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void StudentBlankSchoolThrows(string school) =>
        Assert.ThrowsExactly<ArgumentException>(() => new Student(CreateName(), "Prince@example.com", school, "Princeling"));

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void StudentBlankMajorThrows(string major) =>
        Assert.ThrowsExactly<ArgumentException>(() => new Student(CreateName(), "Prince@example.com", "Lordaeron", major));

    [TestMethod]
    public void StudentsWithSameDataAndDifferentIdsAreEqual()
    {
        var a = CreateStudent();
        var b = CreateStudent();

        Assert.AreNotEqual(a.Id, b.Id);
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void StudentsWithDifferentSchoolsAreNotEqual()
    {
        var a = CreateStudent();
        var b = new Student(CreateName(), "Prince@example.com", "Dalaran", "Princeling", "123-456-1337");

        Assert.AreNotEqual(a, b);
    }

    [TestMethod]
    public void StudentsWithDifferentMajorsAreNotEqual()
    {
        var a = CreateStudent();
        var b = new Student(CreateName(), "Prince@example.com", "Lordaeron", "Death Knight", "123-456-1337");

        Assert.AreNotEqual(a, b);
    }
}
