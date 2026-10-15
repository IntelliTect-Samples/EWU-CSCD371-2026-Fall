using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class FullNameTests
{
    [TestMethod]
    public void Constructor_NullFirstName_ThrowsNullArgumentException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new FullName(null!, "ValidLast"));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_EmptyFirstName_ThrowsArgumentException(string firstName)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new FullName(firstName, "ValidLast"));
    }

    [TestMethod]
    public void Constructor_NullLastName_ThrowsNullArgumentException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new FullName("ValidFirst", null!));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_EmptyLastName_ThrowsArgumentException(string lastName)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new FullName("ValidFirst", lastName));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_Normalization_StoredAsNull(string? middleName)
    {
        FullName name = new("ValidFirst", "ValidLast", middleName);
        Assert.IsNull(name.MiddleName);
    }

    [TestMethod]
    public void ToString_NullMiddleName_ReturnsExpectedString()
    {
        FullName name = new("ValidFirst", "ValidLast");

        Assert.AreEqual("ValidFirst ValidLast", name.ToString());
    }

    [TestMethod]
    public void ToString_PassedMiddleName_ReturnsExpectedString()
    {
        FullName name = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.AreEqual("ValidFirst ValidMiddle ValidLast", name.ToString());
    }

    [TestMethod]
    public void Equals_IdenticalSeparateInstances_ReturnsTrue()
    {
        FullName aName = new("ValidFirst", "ValidLast", "ValidMiddle");
        FullName bName = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.IsTrue(Equals(aName, bName));
    }

    [TestMethod]
    public void GetHashCode_IdenticalSeparateInstances_HaveEqualHashCodes()
    {
        FullName aName = new("ValidFirst", "ValidLast", "ValidMiddle");
        FullName bName = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.AreEqual(aName.GetHashCode(), bName.GetHashCode());
    }

    [TestMethod]
    [DataRow("DifferentFirst", "ValidLast", "ValidMiddle")]
    [DataRow("ValidFirst", "DifferentLast", "ValidMiddle")]
    [DataRow("ValidFirst", "ValidLast", "DifferentMiddle")]
    public void Equals_OneNameComponentDiffers_ReturnsFalse(string firstName, string lastName, string middleName)
    {
        FullName aName = new(firstName, lastName, middleName);
        FullName bName = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.IsFalse(Equals(aName, bName));
    }
}
