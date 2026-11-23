namespace PeopleEnricher.Tests;

[TestClass]
public class PersonTests
{
    [TestMethod]
    public void Person_WithValidData_ContainsExpectedValues()
    {
        const string firstName = "validFirstName";
        const string lastName = "validLastName";
        const string emailAddress = "validEmailAddress";
        const string zipCode = "12345";
        
        Person person = TestData.CreatePerson(
            firstName: firstName,
            lastName: lastName,
            emailAddress: emailAddress,
            zipCode: zipCode);
        
        Assert.AreEqual(firstName, person.FirstName);
        Assert.AreEqual(lastName, person.LastName);
        Assert.AreEqual(emailAddress, person.EmailAddress);
        Assert.AreEqual(zipCode, person.ZipCode);
    }

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
    public void Person_WhenFirstNameIsWhiteSpace_ThrowsArgumentException(string firstName)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(firstName: firstName));
        
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
    public void Person_WhenLastNameIsWhiteSpace_ThrowsArgumentException(string lastName)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(lastName: lastName));
        
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
    public void Person_WhenEmailAddressIsWhiteSpace_ThrowsArgumentException(string emailAddress)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(emailAddress: emailAddress));
        
        Assert.AreEqual("emailAddress", exception.ParamName);
    }

    #endregion

    #region ZipCode Tests

    [TestMethod]
    public void Person_WhenZipCodeIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreatePerson(zipCode: null!));
        
        Assert.AreEqual("zipCode", exception.ParamName);
    }
    
    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Person_WhenZipCodeIsWhiteSpace_ThrowsArgumentException(string zipCode)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreatePerson(zipCode: zipCode));
        
        Assert.AreEqual("zipCode", exception.ParamName);
    }

    [TestMethod]
    [DataRow("1234")]
    [DataRow("123456")]
    [DataRow("ABCDE")]
    public void Person_WhenZipCodeIsInvalid_ThrowsArgumentException(string zipCode)
    {
        ArgumentException exception = 
            Assert.ThrowsExactly<ArgumentException>(() =>
                TestData.CreatePerson(zipCode: zipCode));
        
        Assert.AreEqual("zipCode", exception.ParamName);
    }

    #endregion
}