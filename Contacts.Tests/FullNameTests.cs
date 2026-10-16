using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class FullNameTests
{
    [TestMethod]
    public void Constructor_NullFirstName_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new FullName(null!, "ValidLast"));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_EmptyOrWhiteSpaceFirstName_ThrowsArgumentException(string firstName)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new FullName(firstName, "ValidLast"));
    }

    [TestMethod]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new FullName("ValidFirst", null!));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_EmptyOrWhiteSpaceLastName_ThrowsArgumentException(string lastName)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new FullName("ValidFirst", lastName));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceMiddleName_StoresNull(string? middleName)
    {
        FullName name = new("ValidFirst", "ValidLast", middleName);
        Assert.IsNull(name.MiddleName);
    }

    [TestMethod]
    public void Equals_IdenticalSeparateInstances_ReturnsTrue()
    {
        FullName originalName = new("ValidFirst", "ValidLast", "ValidMiddle");
        FullName comparedName = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.IsTrue(Equals(originalName, comparedName));
    }

    [TestMethod]
    [DataRow("DifferentFirst", "ValidLast", "ValidMiddle")]
    [DataRow("ValidFirst", "DifferentLast", "ValidMiddle")]
    [DataRow("ValidFirst", "ValidLast", "DifferentMiddle")]
    public void Equals_OneNameComponentDiffers_ReturnsFalse(string firstName, string lastName, string middleName)
    {
        FullName originalName = new(firstName, lastName, middleName);
        FullName comparedName = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.IsFalse(Equals(originalName, comparedName));
    }

    [TestMethod]
    public void GetHashCode_IdenticalSeparateInstances_ReturnsEqualHashCodes()
    {
        FullName originalName = new("ValidFirst", "ValidLast", "ValidMiddle");
        FullName comparedName = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.AreEqual(originalName.GetHashCode(), comparedName.GetHashCode());
    }

    [TestMethod]
    public void ToString_NullMiddleName_ReturnsExpectedString()
    {
        FullName name = new("ValidFirst", "ValidLast");

        Assert.AreEqual("ValidFirst ValidLast", name.ToString());
    }

    [TestMethod]
    public void ToString_ProvidedMiddleName_ReturnsExpectedString()
    {
        FullName name = new("ValidFirst", "ValidLast", "ValidMiddle");

        Assert.AreEqual("ValidFirst ValidMiddle ValidLast", name.ToString());
    }
}
