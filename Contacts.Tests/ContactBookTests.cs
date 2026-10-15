using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactBookTests
{
    
    [TestMethod]
    public void Add_DuplicateEntries_BookContainsOneContact()
    {
        Student aStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");
        Student bStudent = new(new FullName("ValidFirst", "ValidLast"), "ValidEmail");

        ContactBook book = new();
        book.Add(aStudent);
        book.Add(bStudent);

        Assert.ContainsSingle(book.GetAll());
    }
}
