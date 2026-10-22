namespace Contacts.Tests;

[TestClass]
public class FullNameTests
{
    [TestMethod]
    public void Constructor_BlankFirstName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new FullName(" ", "Lovelace"));
    }

    [TestMethod]
    public void Constructor_BlankMiddleName_BecomesNull()
    {
        Assert.IsNull(new FullName("Ada", "Lovelace", " ").MiddleName);
    }

    [TestMethod]
    public void ToString_ShowsMiddleNameOnlyWhenPresent()
    {
        Assert.AreEqual("Ada Lovelace", new FullName("Ada", "Lovelace").ToString());
        Assert.AreEqual("Ada King Lovelace", new FullName("Ada", "Lovelace", "King").ToString());
    }

    [TestMethod]
    public void Constructor_NullFirstName_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FullName(null!, "Lovelace"));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public void Constructor_BlankLastName_Throws(string? last)
    {
        Assert.Throws<ArgumentException>(() => new FullName("Ada", last!));
    }

    [TestMethod]
    public void Constructor_NoMiddleName_MiddleNameIsNull()
    {
        FullName name = new("Ada", "Lovelace");

        Assert.AreEqual("Ada", name.FirstName);
        Assert.AreEqual("Lovelace", name.LastName);
        Assert.IsNull(name.MiddleName);
    }

    [TestMethod]
    public void Constructor_TrimsAllNames()
    {
        FullName name = new(" Ada ", " Lovelace ", " King ");

        Assert.AreEqual("Ada", name.FirstName);
        Assert.AreEqual("Lovelace", name.LastName);
        Assert.AreEqual("King", name.MiddleName);
    }

    [TestMethod]
    public void Equals_SameNamesAfterTrimming_AreEqual()
    {
        Assert.AreEqual(new FullName("Ada", "Lovelace", "King"), new FullName(" Ada", "Lovelace ", " King "));
    }

    [TestMethod]
    public void Equals_DifferentMiddleName_AreNotEqual()
    {
        Assert.AreNotEqual(new FullName("Ada", "Lovelace", "King"), new FullName("Ada", "Lovelace"));
    }
}
