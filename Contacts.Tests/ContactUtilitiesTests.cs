using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class ContactUtilitiesTests
{
    #region Name Validation Tests
    
    [TestMethod]
    public void ValidateName_WithNull_ReturnsFalseAndErrorMessage()
    {
        const string? name = null;
        
        bool result = ContactUtilities.ValidateName(name, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Name cannot be empty", errorMessage);
    }

    [TestMethod]
    public void ValidateName_WithWhiteSpace_ReturnsFalseAndErrorMessage()
    {
        const string name = " ";
        
        bool result = ContactUtilities.ValidateName(name, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Name cannot be empty", errorMessage);
    }

    [TestMethod]
    [DataRow("John")]
    [DataRow("Smith")]
    [DataRow("O'Brien")]
    [DataRow("Jones-Perry")]
    public void ValidateName_WithProperName_ReturnsTrueAndEmptyErrorMessage(string name)
    {
        bool result = ContactUtilities.ValidateName(name, out string errorMessage);
        
        Assert.IsTrue(result);
        Assert.AreEqual(string.Empty, errorMessage);
    }

    [TestMethod]
    [DataRow("John1")]
    [DataRow("John@")]
    [DataRow("John!")]
    public void ValidateName_WithInvalidCharacter_ReturnsFalseAndErrorMessage(string name)
    {
        bool result = ContactUtilities.ValidateName(name, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual(
            "Names may only contain letters. Apostrophes (') and hyphens (-) are allowed between letters.",
            errorMessage);
    }
    
    [TestMethod]
    [DataRow("-John", '-')]
    [DataRow("John-", '-')]
    [DataRow("'John", '\'')]
    [DataRow("John'", '\'')]
    [DataRow("Jo-'hn", '-')]
    [DataRow("Jo'-hn", '\'')]
    public void ValidateName_WithImproperStructure_ReturnsFalseAndErrorMessage(string name, char invalidCharacter)
    {
        bool result = ContactUtilities.ValidateName(name, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual(
            $"The Character: ({invalidCharacter}) must have a letter on both sides.",
            errorMessage);
    }

    #endregion

    #region Email Validation Tests

    [TestMethod]
    public void ValidateEmail_WithNull_ReturnsFalseAndErrorMessage()
    {
        const string? email = null;
        
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Email cannot be empty", errorMessage);
    }

    [TestMethod]
    public void ValidateEmail_WithWhiteSpace_ReturnsFalseAndErrorMessage()
    {
        const string email = " ";
        
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Email cannot be empty", errorMessage);
    }

    [TestMethod]
    public void ValidateEmail_WithNoAtCharacter_ReturnsFalseAndErrorMessage()
    {
        const string email = "no at";
        
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Email must contain exactly one '@' character.", errorMessage);
    }

    [TestMethod]
    public void ValidateEmail_WithMultipleAtCharacters_ReturnsFalseAndErrorMessage()
    {
        const string email = "one @ two @, red @, blue @";
        
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Email must contain exactly one '@' character.", errorMessage);
    }

    [TestMethod]
    [DataRow("@noLocal")]
    [DataRow("   @blankSpaceLocal")]
    public void ValidateEmail_InvalidLocalPart_ReturnsFalseAndErrorMessage(string email)
    {
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual("Email address must contain at least one character prior to '@'.", errorMessage);
    }

    [TestMethod]
    [DataRow("john@")]
    [DataRow("john@com")]
    [DataRow("john@.com")]
    [DataRow("john@t.t")]
    [DataRow("john@wrong.t")]
    [DataRow("john@wrong.")]
    public void ValidateEmail_InvalidDomainFormat_ReturnsFalseAndErrorMessage(string email)
    {
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual(
            "Email Domain must follow domain.extension format, with at least 1 character prior to '.' and 2 post.",
            errorMessage
        );
    }

    [TestMethod]
    [DataRow("john@company.123")]
    [DataRow("john@company.a&")]
    public void ValidateEmail_InvalidEmailDomainExtension_ReturnsFalseAndErrorMessage(string email)
    {
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsFalse(result);
        Assert.AreEqual(
            "Email domain extension may only contain letters.",
            errorMessage
        );
    }

    [TestMethod]
    [DataRow("john@company.com")]
    [DataRow("john.smith@company.org")]
    [DataRow("johnsmith@company.co.uk")]
    public void ValidateEmail_ValidEmailExamples_ReturnsTrueAndEmptyErrorMessage(string email)
    {
        bool result = ContactUtilities.ValidateEmail(email, out string errorMessage);
        
        Assert.IsTrue(result);
        Assert.AreEqual(string.Empty, errorMessage);
    }

    #endregion
}