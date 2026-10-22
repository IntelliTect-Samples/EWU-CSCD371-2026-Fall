using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactTests
{
    [TestMethod]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Student(null!, ["ValidEmail"]));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceEmails_StoresEmptyCollection(string? email)
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"),
            email is null ? null : [email], "ValidPhoneNumber");

        Assert.IsEmpty(contact.Emails);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpacePhoneNumber_StoresNull(string? phoneNumber)
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"], phoneNumber);

        Assert.IsNull(contact.PhoneNumber);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceAddress_StoresNull(string? address)
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"], address: address);

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
            contactInformation is null ? null : [contactInformation], contactInformation, contactInformation));
    }

    [TestMethod]
    public void Equals_IdenticalSeparateInstances_ReturnsTrue()
    {
        Contact originalContact = new Student(new FullName("ValidFirst", "ValidLast"),
            ["ValidEmail"], "ValidPhoneNumber", "ValidAddress");
        Contact comparedContact = new Student(new FullName("ValidFirst", "ValidLast"),
            ["ValidEmail"], "ValidPhoneNumber", "ValidAddress");

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
        Contact originalContact = new Student(new FullName(firstName, "ValidLast"), [email], phoneNumber, address);
        Contact comparedContact = new Student(new FullName("ValidFirst", "ValidLast"),
            ["ValidEmail"], "ValidPhoneNumber", "ValidAddress");

        Assert.IsFalse(Equals(originalContact, comparedContact));
    }

    [TestMethod]
    public void Equals_NullContact_ReturnsFalse()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);

        Assert.IsFalse(contact.Equals(null));
    }

    [TestMethod]
    public void Equals_StudentAndEmployeeWithSameData_ReturnsFalse()
    {
        Contact student = new Student(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        Contact employee = new Employee(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);

        Assert.IsFalse(student.Equals(employee));
        Assert.IsFalse(employee.Equals(student));
    }

    [TestMethod]
    public void GetHashCode_IdenticalSeparateInstances_ReturnsEqualHashCodes()
    {
        Contact originalContact = new Student(new FullName("ValidFirst", "ValidLast"),
            ["ValidEmail"], "ValidPhoneNumber", "ValidAddress");
        Contact comparedContact = new Student(new FullName("ValidFirst", "ValidLast"),
            ["ValidEmail"], "ValidPhoneNumber", "ValidAddress");

        Assert.AreEqual(originalContact.GetHashCode(), comparedContact.GetHashCode());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Emails_MultipleAddresses_AreTrimmedSortedAndDeduplicated(bool isEmployee)
    {
        Contact contact = CreateContact(isEmployee,
            [" work@example.com ", "personal@example.com", "work@example.com", "", " "]);
        string[] expected = ["personal@example.com", "work@example.com"];

        CollectionAssert.AreEqual(expected, contact.Emails.ToArray());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Equals_SameEmailsInDifferentOrder_ReturnsTrueAndSameHashCode(bool isEmployee)
    {
        Contact original = CreateContact(isEmployee, ["work@example.com", "personal@example.com"]);
        Contact reordered = CreateContact(isEmployee, ["personal@example.com", "work@example.com"]);

        Assert.AreNotEqual(original.Id, reordered.Id);
        Assert.AreEqual(original, reordered);
        Assert.AreEqual(original.GetHashCode(), reordered.GetHashCode());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Equals_AdditionalEmail_ReturnsFalse(bool isEmployee)
    {
        Contact original = CreateContact(isEmployee, ["work@example.com"]);
        Contact additional = CreateContact(isEmployee, ["work@example.com", "personal@example.com"]);

        Assert.AreNotEqual(original, additional);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Equals_OneOfMultipleEmailsDiffers_ReturnsFalse(bool isEmployee)
    {
        Contact original = CreateContact(isEmployee, ["work@example.com", "personal@example.com"]);
        Contact different = CreateContact(isEmployee, ["work@example.com", "other@example.com"]);

        Assert.AreNotEqual(original, different);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Equals_EmailCaseDiffers_ReturnsFalse(bool isEmployee)
    {
        Contact original = CreateContact(isEmployee, ["Alice@example.com"]);
        Contact different = CreateContact(isEmployee, ["alice@example.com"]);

        Assert.AreNotEqual(original, different);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Emails_ChangingSourceCollection_PreservesContactAndHashSetMembership(bool isEmployee)
    {
        List<string> emails = ["work@example.com", "personal@example.com"];
        Contact contact = CreateContact(isEmployee, emails);
        Contact original = CreateContact(isEmployee, ["work@example.com", "personal@example.com"]);
        ContactBook contactBook = new();
        contactBook.Add(contact);
        int originalHashCode = contact.GetHashCode();

        emails[0] = "changed@example.com";
        emails.Add("new@example.com");

        Assert.AreEqual(original, contact);
        Assert.AreEqual(originalHashCode, contact.GetHashCode());
        Assert.IsTrue(contactBook.Contains(original));
        Assert.IsFalse(contactBook.Add(original));
        Assert.IsTrue(contactBook.Remove(original));
        Assert.IsEmpty(contactBook.GetAll());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Emails_ManyAddresses_AreAllRetained(bool isEmployee)
    {
        string[] emails = [.. Enumerable.Range(0, 100).Select(index => $"contact{index}@example.com")];
        Contact contact = CreateContact(isEmployee, emails);

        Assert.HasCount(100, contact.Emails);
        CollectionAssert.AreEquivalent(emails, contact.Emails.ToArray());
    }

    [TestMethod]
    public void Emails_ExposedCollection_DoesNotAllowChanges()
    {
        Contact contact = CreateContact(false, ["work@example.com"]);
        Contact original = CreateContact(false, ["work@example.com"]);

        if (contact.Emails is IList<string> emails)
        {
            Assert.ThrowsExactly<NotSupportedException>(() => emails[0] = "changed@example.com");
        }

        Assert.AreEqual(original, contact);
        Assert.AreEqual(original.GetHashCode(), contact.GetHashCode());
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Constructor_EmptyEmailsWithoutOtherContactMethods_ThrowsArgumentException(bool isEmployee)
    {
        Assert.ThrowsExactly<ArgumentException>(() => CreateContact(isEmployee, []));
    }

    [TestMethod]
    public void ToString_MultipleEmails_ShowsAllAddresses()
    {
        Contact contact = CreateContact(false, ["work@example.com", "personal@example.com"]);

        StringAssert.Contains(contact.ToString(), "Emails: personal@example.com, work@example.com");
    }

    [TestMethod]
    public void ToString_NoEmails_OmitsEmailsField()
    {
        Contact contact = new Student(new FullName("Alice", "Smith"), [], phoneNumber: "555-0100");

        Assert.IsFalse(contact.ToString().Contains("Emails:", StringComparison.Ordinal));
    }

    [TestMethod]
    public void ToString_NullPhoneNumberAndAddress_ReturnsExpectedString()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        string expectedOutput = $"Id: {contact.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Emails: ValidEmail{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, contact.ToString());
    }

    [TestMethod]
    public void ToString_ProvidedContactMethods_ReturnsExpectedString()
    {
        Contact contact = new Student(new FullName("ValidFirst", "ValidLast"),
            ["ValidEmail"], "ValidPhoneNumber", "ValidAddress");
        string expectedOutput = $"Id: {contact.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Emails: ValidEmail{Environment.NewLine}"
            + $"PhoneNumber: ValidPhoneNumber{Environment.NewLine}"
            + $"Address: ValidAddress{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, contact.ToString());
    }

    private static Contact CreateContact(bool isEmployee, IReadOnlyList<string> emails) =>
        isEmployee
            ? new Employee(new FullName("Alice", "Smith"), emails)
            : new Student(new FullName("Alice", "Smith"), emails);
}
