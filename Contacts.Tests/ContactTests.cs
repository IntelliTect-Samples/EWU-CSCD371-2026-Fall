namespace Contacts.Tests;

[TestClass]
public class ContactTests
{
    private static FullName Ada => new("Ada", "Lovelace");

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
}
