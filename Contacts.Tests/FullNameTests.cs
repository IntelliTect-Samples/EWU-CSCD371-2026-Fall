
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Contacts;

namespace Contacts.Tests;

[TestClass]
public class FullNameTests
{
    [TestMethod]
    public void ToString_WithoutMiddleName_ReturnsFirstAndLast()
    {
        var name = new FullName("Alex", "Smith", null);

        Assert.AreEqual("Alex Smith", name.ToString());
    }

    [TestMethod]
    public void ToString_WithMiddleName_ReturnsAllThreeParts()
    {
        var name = new FullName("Alex", "Smith", "Jordan");

        Assert.AreEqual("Alex Jordan Smith", name.ToString());
    }

    [TestMethod]
    public void Equality_UsesAllNameParts()
    {
        var first = new FullName("Alex", "Smith", null);
        var equivalent = new FullName("Alex", "Smith", null);
        var different = new FullName("Alex", "Smith", "Jordan");

        Assert.AreEqual(first, equivalent);
        Assert.AreNotEqual(first, different);
    }
}
