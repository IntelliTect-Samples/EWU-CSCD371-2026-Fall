namespace PeopleEnricher.Tests;

[TestClass]
public class AddressTests
{
    #region StreetAddress Tests

    [TestMethod]
    public void Address_StreetAddressIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreateAddress(streetAddress: null!));
        
        Assert.AreEqual("streetAddress", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Address_StreetAddressIsEmptyOrWhiteSpace_ThrowsArgumentException(string streetAddress)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreateAddress(streetAddress: streetAddress));
        
        Assert.AreEqual("streetAddress", exception.ParamName);
    }

    #endregion
    
    #region City Tests

    [TestMethod]
    public void Address_CityIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreateAddress(city: null!));
        
        Assert.AreEqual("city", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Address_CityIsEmptyOrWhiteSpace_ThrowsArgumentException(string city)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreateAddress(city: city));
        
        Assert.AreEqual("city", exception.ParamName);
    }

    #endregion
    
    #region State Tests

    [TestMethod]
    public void Address_StateIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreateAddress(state: null!));
        
        Assert.AreEqual("state", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Address_StateIsEmptyOrWhiteSpace_ThrowsArgumentException(string state)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreateAddress(state: state));
        
        Assert.AreEqual("state", exception.ParamName);
    }

    #endregion
    
    #region Zip Tests

    [TestMethod]
    public void Address_ZipIsNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() =>
            TestData.CreateAddress(zip: null!));
        
        Assert.AreEqual("zip", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    public void Address_ZipIsEmptyOrWhiteSpace_ThrowsArgumentException(string zip)
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(() =>
            TestData.CreateAddress(zip: zip));
        
        Assert.AreEqual("zip", exception.ParamName);
    }

    #endregion
}