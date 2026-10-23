using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public sealed class FullNameTests
{
    [TestMethod]
    public void Constructor_StoresAndTrimsParts()
    {
        FullName name = new("  Ada ", " Lovelace  ", " King ");

        Assert.AreEqual("Ada", name.First);
        Assert.AreEqual("King", name.Middle);
        Assert.AreEqual("Lovelace", name.Last);
    }

    [TestMethod]
    public void Constructor_MiddleOmitted_IsNull()
    {
        FullName name = new("Ada", "Lovelace");

        Assert.IsNull(name.Middle);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_BlankMiddle_IsNull(string middle)
    {
        FullName name = new("Ada", "Lovelace", middle);

        Assert.IsNull(name.Middle);
    }

    [TestMethod]
    public void Constructor_NullMiddle_IsNull()
    {
        FullName name = new("Ada", "Lovelace", null);

        Assert.IsNull(name.Middle);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_BlankFirst_Throws(string first)
    {
        Assert.Throws<ArgumentException>(() => _ = new FullName(first, "Lovelace"));
    }

    [TestMethod]
    public void Constructor_NullFirst_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new FullName(null!, "Lovelace"));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_BlankLast_Throws(string last)
    {
        Assert.Throws<ArgumentException>(() => _ = new FullName("Ada", last));
    }

    [TestMethod]
    public void Constructor_NullLast_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new FullName("Ada", null!));
    }

    [TestMethod]
    public void Equals_SameParts_AreEqualWithSameHashCode()
    {
        FullName a = new("Ada", "Lovelace", "King");
        FullName b = new("Ada", "Lovelace", "King");

        Assert.AreNotSame(a, b);
        Assert.AreEqual(a, b);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void Equals_BlankMiddleAndNoMiddle_AreEqual()
    {
        Assert.AreEqual(new FullName("Ada", "Lovelace", " "), new FullName("Ada", "Lovelace"));
    }

    [TestMethod]
    public void Equals_DifferentMiddle_AreNotEqual()
    {
        Assert.AreNotEqual(new FullName("Ada", "Lovelace", "King"), new FullName("Ada", "Lovelace"));
    }

    [TestMethod]
    public void ToString_WithoutMiddle_ShowsFirstAndLast()
    {
        Assert.AreEqual("Ada Lovelace", new FullName("Ada", "Lovelace").ToString());
    }

    [TestMethod]
    public void ToString_WithMiddle_ShowsAllParts()
    {
        Assert.AreEqual("Ada King Lovelace", new FullName("Ada", "Lovelace", "King").ToString());
    }
}
