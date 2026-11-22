namespace PeopleEnricher.Tests;

[TestClass]
public class PersonTests
{
    #region Id Tests

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void Person_WhenIdIsNegativeOrZero_ThrowsArgumentOutOfRangeException(int id)
    {
        ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            TestData.CreatePerson(id: id));
        
        Assert.AreEqual("id", exception.ParamName);
    }

    #endregion

    #region FirstName Tests

    [TestMethod]
    public void Person_WhenFirstNameIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreatePerson(firstName: null!));
        
        Assert.AreEqual("firstName", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Person_WhenFirstNameIsWhiteSpace_ThrowsArgumentException(string? firstName)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(firstName: firstName!));
        
        Assert.AreEqual("firstName", exception.ParamName);
    }

    #endregion

    #region LastName Tests
    
    [TestMethod]
    public void Person_WhenLastNameIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreatePerson(lastName: null!));
        
        Assert.AreEqual("lastName", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Person_WhenLastNameIsWhiteSpace_ThrowsArgumentException(string? lastName)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(lastName: lastName!));
        
        Assert.AreEqual("lastName", exception.ParamName);
    }

    #endregion

    #region EmailAddress Tests

    [TestMethod]
    public void Person_WhenEmailAddressIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreatePerson(emailAddress: null!));
        
        Assert.AreEqual("emailAddress", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Person_WhenEmailAddressIsWhiteSpace_ThrowsArgumentException(string? emailAddress)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(emailAddress: emailAddress!));
        
        Assert.AreEqual("emailAddress", exception.ParamName);
    }

    #endregion

    #region Address Tests

    [TestMethod]
    public void Person_WhenAddressIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            new Person(
                id: 1,
                firstName: "firstName",
                lastName: "lastName",
                emailAddress: "emailAddress",
                address: null!));
        
        Assert.AreEqual("address", exception.ParamName);
    }

    #endregion
}