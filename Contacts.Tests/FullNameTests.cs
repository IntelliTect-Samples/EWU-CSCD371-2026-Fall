using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class FullNameTests
{
    // notes: Equality tests proves that two "equal" FulName Records are both equal and produce same hash
    // two unequal FullName records may produce same hash - but will not compare as equal
    
    // therefore we prove two FullNames refs, when they are supposed to compare equal: 
    // properties compare equal, and they produce equal hashes
    
    // when proving two FullName ref records are not equal:
    // we only show that the props do not compare equal.
    // as the hash may (very unlikely, but importantly, non-zero chance) compute the same
    
    // These Tests are to provide a "contract" in which:
    // editing FullName record in a way that breaks these tests is immediately noticed by failing tests
    
    // consts used to avoid the accidental mistype of strings effecting testing outcomes
    private const string FirstName = "John";
    private const string LastName = "Smith";
    private const string MiddleName = "Middle";
    
    #region Equality Proofs

    [TestMethod]
    public void FullNames_WithSameContents_AreEqual()
    {
        FullName name1 = new(FirstName, LastName);
        FullName name2 = new(FirstName, LastName);
        
        Assert.AreEqual(name1, name2);
    }
    
    [TestMethod]
    public void FullNames_WithSameContents_SameHash()
    {
        FullName name1 = new(FirstName, LastName);
        FullName name2 = new(FirstName, LastName);
        
        Assert.AreEqual(name1.GetHashCode(), name2.GetHashCode());
    }
    
    [TestMethod]
    public void FullName_EqualNonNullMiddleNames_AreEqual()
    {
        FullName name1 = new(FirstName, LastName, MiddleName);
        FullName name2 = new(FirstName, LastName, MiddleName);
        
        Assert.AreEqual(name1, name2);
    }

    [TestMethod]
    public void FullName_EqualNonNullMiddleNames_SameHash()
    {
        FullName name1 = new(FirstName, LastName, MiddleName);
        FullName name2 = new(FirstName, LastName, MiddleName);
        
        Assert.AreEqual(name1.GetHashCode(), name2.GetHashCode());
    }

    #endregion

    #region Inequality Proofs

    [TestMethod]
    public void FullName_WithDifferentFirstNames_AreNotEqual()
    {
        FullName name1 = new("name1", LastName);
        FullName name2 = new("name2", LastName);
        
        Assert.AreNotEqual(name1, name2);
    }
    
    [TestMethod]
    public void FullName_WithDifferentLastNames_AreNotEqual()
    {
        FullName name1 = new(FirstName, "name1");
        FullName name2 = new(FirstName, "name2");
        
        Assert.AreNotEqual(name1, name2);
    }
    
    [TestMethod]
    public void FullNames_DifferingNonNullMiddleNames_AreNotEqual()
    {
        FullName name1 = new(FirstName, LastName, "name1");
        FullName name2 = new(FirstName, LastName, "name2");
        
        Assert.AreNotEqual(name1, name2);
    }
    
    [TestMethod]
    public void FullName_NullAndNonNullMiddleName_AreNotEqual()
    {
        FullName name1 = new(FirstName, LastName);
        FullName name2 = new(FirstName, LastName, MiddleName);
        
        Assert.AreNotEqual(name1, name2);
    }

    #endregion
}