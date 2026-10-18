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
}
