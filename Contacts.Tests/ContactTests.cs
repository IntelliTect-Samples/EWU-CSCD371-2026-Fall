namespace Contacts.Tests;

[TestClass]
public class ContactTests
{
    private static FullName Ada => new("Ada", "Lovelace");

    private static readonly string[] AdaEmail = ["ada@example.com"];

    private static readonly string[] AdaEmails = ["ada@example.com", "ada@ewu.edu"];

    [TestMethod]
    public void Equals_SameDataDifferentIds_AreEqual()
    {
        Student first = new(Ada, ["ada@example.com"], "CS");
        Student second = new(Ada, ["ada@example.com"], "CS");

        Assert.AreNotEqual(first.Id, second.Id);
        Assert.AreEqual(first, second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void Equals_StudentAndEmployeeWithSameData_AreNotEqual()
    {
        Student student = new(Ada, ["ada@example.com"], "Engineer");
        Employee employee = new(Ada, ["ada@example.com"], "Engineer");

        Assert.IsFalse(student.Equals((object)employee));
    }

    [TestMethod]
    public void Equals_DifferentEmail_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Student(Ada, ["ada@example.com"], "CS"),
            new Student(Ada, ["other@example.com"], "CS"));
    }

    [TestMethod]
    public void Constructor_BlankEmail_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Student(Ada, [" "], "CS"));
    }

    [TestMethod]
    public void Student_SetsAllProperties()
    {
        Student student = new(Ada, ["ada@example.com"], "CS", "555-0100");

        Assert.AreEqual(Ada, student.Name);
        CollectionAssert.AreEqual(AdaEmail, student.Emails.ToArray());
        Assert.AreEqual("CS", student.Major);
        Assert.AreEqual("555-0100", student.Phone);
        Assert.AreNotEqual(Guid.Empty, student.Id);
    }

    [TestMethod]
    public void Employee_SetsAllProperties()
    {
        Employee employee = new(Ada, ["ada@example.com"], "Engineer", "555-0100");

        Assert.AreEqual(Ada, employee.Name);
        CollectionAssert.AreEqual(AdaEmail, employee.Emails.ToArray());
        Assert.AreEqual("Engineer", employee.JobTitle);
        Assert.AreEqual("555-0100", employee.Phone);
        Assert.AreNotEqual(Guid.Empty, employee.Id);
    }

    [TestMethod]
    public void Contacts_ImplementIContact()
    {
        Assert.IsInstanceOfType<IContact>(new Student(Ada, ["ada@example.com"], "CS"));
        Assert.IsInstanceOfType<IContact>(new Employee(Ada, ["ada@example.com"], "Engineer"));
    }

    [TestMethod]
    public void Constructor_NullName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new Student(null!, ["ada@example.com"], "CS"));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void Student_BlankMajor_Throws(string? major)
    {
        Assert.Throws<ArgumentException>(() => new Student(Ada, ["ada@example.com"], major!));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void Employee_BlankJobTitle_Throws(string? jobTitle)
    {
        Assert.Throws<ArgumentException>(() => new Employee(Ada, ["ada@example.com"], jobTitle!));
    }

    [TestMethod]
    public void Constructor_TrimsMajorJobTitleAndPhone()
    {
        Assert.AreEqual("CS", new Student(Ada, ["ada@example.com"], " CS ").Major);
        Assert.AreEqual("Engineer", new Employee(Ada, ["ada@example.com"], " Engineer ").JobTitle);
        Assert.AreEqual("555-0100", new Student(Ada, ["ada@example.com"], "CS", " 555-0100 ").Phone);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void Constructor_BlankPhone_BecomesNull(string? phone)
    {
        Assert.IsNull(new Student(Ada, ["ada@example.com"], "CS", phone).Phone);
    }

    [TestMethod]
    public void Equals_DifferentMajor_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Student(Ada, ["ada@example.com"], "CS"),
            new Student(Ada, ["ada@example.com"], "Math"));
    }

    [TestMethod]
    public void Equals_DifferentJobTitle_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Employee(Ada, ["ada@example.com"], "Engineer"),
            new Employee(Ada, ["ada@example.com"], "Manager"));
    }

    [TestMethod]
    public void Equals_DifferentPhone_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Student(Ada, ["ada@example.com"], "CS", "555-0100"),
            new Student(Ada, ["ada@example.com"], "CS"));
    }

    [TestMethod]
    public void Equals_DifferentName_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Student(Ada, ["ada@example.com"], "CS"),
            new Student(new FullName("Grace", "Hopper"), ["ada@example.com"], "CS"));
    }

    [TestMethod]
    public void Equals_EmployeesWithSameDataDifferentIds_AreEqual()
    {
        Employee first = new(Ada, ["ada@example.com"], "Engineer");
        Employee second = new(Ada, ["ada@example.com"], "Engineer");

        Assert.AreNotEqual(first.Id, second.Id);
        Assert.AreEqual(first, second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void Equals_Null_IsFalse()
    {
        Student student = new(Ada, ["ada@example.com"], "CS");

        Assert.IsFalse(student.Equals(null));
        Assert.IsFalse(student == null);
    }

    [TestMethod]
    public void Equals_ThroughIContact_UsesValueEquality()
    {
        IContact first = new Student(Ada, ["ada@example.com"], "CS");
        IContact second = new Student(Ada, ["ada@example.com"], "CS");

        Assert.IsTrue(first.Equals(second));
    }

    [TestMethod]
    public void With_ChangedId_StillEqual()
    {
        Student student = new(Ada, ["ada@example.com"], "CS");

        Student copy = student with { Id = Guid.NewGuid() };

        Assert.AreNotEqual(student.Id, copy.Id);
        Assert.AreEqual(student, copy);
    }

    [TestMethod]
    public void Emails_MultipleEmails_AreKeptInOrder()
    {
        Student student = new(Ada, ["ada@example.com", "ada@ewu.edu"], "CS");

        CollectionAssert.AreEqual(AdaEmails, student.Emails.ToArray());
    }

    [TestMethod]
    public void Emails_AreTrimmedAndDuplicatesRemoved()
    {
        Student student = new(Ada, [" ada@example.com ", "ada@example.com", "ADA@example.com"], "CS");

        CollectionAssert.AreEqual(AdaEmail, student.Emails.ToArray());
    }

    [TestMethod]
    public void Emails_ChangingSourceListAfterwards_DoesNotChangeContact()
    {
        List<string> emails = ["ada@example.com"];
        Student student = new(Ada, emails, "CS");

        emails.Add("other@example.com");

        Assert.HasCount(1, student.Emails);
    }

    [TestMethod]
    public void Emails_Empty_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Student(Ada, [], "CS"));
    }

    [TestMethod]
    public void Emails_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new Student(Ada, null!, "CS"));
    }

    [TestMethod]
    public void Equals_SameEmailsDifferentOrder_AreEqual()
    {
        Student first = new(Ada, ["ada@example.com", "ada@ewu.edu"], "CS");
        Student second = new(Ada, ["ada@ewu.edu", "ada@example.com"], "CS");

        Assert.AreEqual(first, second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void Equals_EmailsDifferingOnlyInCase_AreEqual()
    {
        Student first = new(Ada, ["Ada@Example.com"], "CS");
        Student second = new(Ada, ["ada@example.com"], "CS");

        Assert.AreEqual(first, second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void Equals_ExtraEmail_AreNotEqual()
    {
        Assert.AreNotEqual(
            new Student(Ada, ["ada@example.com"], "CS"),
            new Student(Ada, ["ada@example.com", "ada@ewu.edu"], "CS"));
    }
}
