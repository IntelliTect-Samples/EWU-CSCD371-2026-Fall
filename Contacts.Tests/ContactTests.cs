using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactTests
{
    [TestMethod]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Student(null!, "ValidEmail"));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceEmail_StoresNull(string? email)
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), email, "ValidPhoneNumber");

        Assert.IsNull(contact.Email);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpacePhoneNumber_StoresNull(string? phoneNumber)
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), "ValidEmail", phoneNumber);

        Assert.IsNull(contact.PhoneNumber);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceAddress_StoresNull(string? address)
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), "ValidEmail", address: address);

        Assert.IsNull(contact.Address);
    }

    [TestMethod]
    public void Constructor_AddressOnly_StoresAddress()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), address: "ValidAddress");

        Assert.AreEqual("ValidAddress", contact.Address);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    public void Constructor_NoUsableContactMethods_ThrowsArgumentException(string? contactInformation)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new Student(new FullName("ValidFirst", "ValidLast"),
            contactInformation, contactInformation, contactInformation));
    }

    [TestMethod]
    public void Equals_IdenticalSeparateInstances_ReturnsTrue()
    {
        Contact originalContact = new Student(new FullName("ValidFirst", "ValidLast"),
            "ValidEmail", "ValidPhoneNumber", "ValidAddress");
        Contact comparedContact = new Student(new FullName("ValidFirst", "ValidLast"),
            "ValidEmail", "ValidPhoneNumber", "ValidAddress");

        Assert.AreNotEqual(originalContact.Id, comparedContact.Id);
        Assert.IsTrue(Equals(originalContact, comparedContact));
    }

    [TestMethod]
    [DataRow("DifferentFirst", "ValidEmail", "ValidPhoneNumber", "ValidAddress")]
    [DataRow("ValidFirst", "DifferentEmail", "ValidPhoneNumber", "ValidAddress")]
    [DataRow("ValidFirst", "ValidEmail", "DifferentPhoneNumber", "ValidAddress")]
    [DataRow("ValidFirst", "ValidEmail", "ValidPhoneNumber", "DifferentAddress")]
    public void Equals_OneContactComponentDiffers_ReturnsFalse(
        string firstName, string email, string phoneNumber, string address)
    {
        Contact originalContact = new Student(new FullName(firstName, "ValidLast"), email, phoneNumber, address);
        Contact comparedContact = new Student(new FullName("ValidFirst", "ValidLast"),
            "ValidEmail", "ValidPhoneNumber", "ValidAddress");

        Assert.IsFalse(Equals(originalContact, comparedContact));
    }

    [TestMethod]
    public void Equals_NullContact_ReturnsFalse()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

        Assert.IsFalse(contact.Equals(null));
    }

    [TestMethod]
    public void Equals_StudentAndEmployeeWithSameData_ReturnsFalse()
    {
        Contact student = new Student(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Contact employee = new Employee(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

        Assert.IsFalse(student.Equals(employee));
        Assert.IsFalse(employee.Equals(student));
    }

    [TestMethod]
    public void GetHashCode_IdenticalSeparateInstances_ReturnsEqualHashCodes()
    {
        Contact originalContact = new Student(new FullName("ValidFirst", "ValidLast"),
            "ValidEmail", "ValidPhoneNumber", "ValidAddress");
        Contact comparedContact = new Student(new FullName("ValidFirst", "ValidLast"),
            "ValidEmail", "ValidPhoneNumber", "ValidAddress");

        Assert.AreEqual(originalContact.GetHashCode(), comparedContact.GetHashCode());
    }

    [TestMethod]
    public void ToString_NullPhoneNumberAndAddress_ReturnsExpectedString()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        string expectedOutput = $"Id: {contact.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Email: ValidEmail{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, contact.ToString());
    }

    [TestMethod]
    public void ToString_ProvidedContactMethods_ReturnsExpectedString()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"),
            "ValidEmail", "ValidPhoneNumber", "ValidAddress");
        string expectedOutput = $"Id: {contact.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Email: ValidEmail{Environment.NewLine}"
            + $"PhoneNumber: ValidPhoneNumber{Environment.NewLine}"
            + $"Address: ValidAddress{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, contact.ToString());
    }
}
