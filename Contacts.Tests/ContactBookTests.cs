using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public sealed class ContactBookTests
{
    private sealed class TestContact : IContact;

    [TestMethod]
    public void AddNewContactContainsContact()
    {
        ContactBook book = new();
        TestContact contact = new();

        book.Add(contact);

        Assert.IsTrue(book.Contains(contact));
    }
}
