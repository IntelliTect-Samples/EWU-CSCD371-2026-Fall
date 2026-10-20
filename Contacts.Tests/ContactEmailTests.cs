using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactEmailTests
{
    private const string TestEmail = "email";
    private const string TestEmail2 = "email2";

    #region Equality Proofs

    [TestMethod]
    public void ContactEmails_WithSameEmailContents_AreEqual()
    {
        ContactEmails emails1 = new([TestEmail]);
        ContactEmails emails2 = new([TestEmail]);
        
        Assert.AreEqual(emails1, emails2);
    }
    
    [TestMethod]
    public void ContactEmails_WithSameEmailContents_HaveSameHashCode()
    {
        ContactEmails emails1 = new([TestEmail]);
        ContactEmails emails2 = new([TestEmail]);
        
        Assert.AreEqual(emails1.GetHashCode(), emails2.GetHashCode());
    }

    // NOTE: Same emails in a different order will let records 'equate' to equal
    
    [TestMethod]
    public void ContactEmails_WithSameEmailsDifferentOrder_AreEqual()
    {
        ContactEmails emails1 = new([TestEmail, TestEmail2]);
        ContactEmails emails2 = new([TestEmail2, TestEmail]);
        
        Assert.AreEqual(emails1, emails2);
    }

    [TestMethod]
    public void ContactEmails_WithSameEmailsDifferentOrder_SameHashCode()
    {
        ContactEmails emails1 = new([TestEmail, TestEmail2]);
        ContactEmails emails2 = new([TestEmail2, TestEmail]);
        
        Assert.AreEqual(emails1.GetHashCode(), emails2.GetHashCode());
    }

    #endregion

    #region Inequality Proofs

    [TestMethod]
    public void ContactEmails_WithDifferentEmails_AreNotEqual()
    {
        ContactEmails emails1 = new([TestEmail]);
        ContactEmails emails2 = new([TestEmail2]);
        
        Assert.AreNotEqual(emails1, emails2);
    }

    #endregion
}