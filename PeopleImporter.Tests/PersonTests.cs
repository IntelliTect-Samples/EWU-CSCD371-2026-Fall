using Assignment;

namespace PeopleImporter.Tests;

[TestClass]
public sealed class PersonTests
{
    private static Address CreateAddress()
    {
        return new Address(
            "123 Main St",
            "Anytown",
            "CA",
            "12345");
    }

    [TestMethod]
    public void Constructor_ValidArguments_SetsProperties()
    {
        Address address = CreateAddress();

        Person person = new(
            1,
            "John",
            "Doe",
            address,
            "john.doe@example.com");

        Assert.AreEqual(1, person.ID);
        Assert.AreEqual("John", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreSame(address, person.Address);
        Assert.AreEqual("john.doe@example.com", person.EmailAddress);
    }

    [TestMethod]
    public void Constructor_ZeroId_ThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new Person(
                    0,
                    "John",
                    "Doe",
                    CreateAddress(),
                    "john.doe@example.com"));

        Assert.AreEqual("id", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NegativeId_ThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new Person(
                    -1,
                    "John",
                    "Doe",
                    CreateAddress(),
                    "john.doe@example.com"));

        Assert.AreEqual("id", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullFirstName_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Person(
                    1,
                    null!,
                    "Doe",
                    CreateAddress(),
                    "john.doe@example.com"));

        Assert.AreEqual("firstName", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Person(
                    1,
                    "John",
                    null!,
                    CreateAddress(),
                    "john.doe@example.com"));

        Assert.AreEqual("lastName", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullAddress_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Person(
                    1,
                    "John",
                    "Doe",
                    null!,
                    "john.doe@example.com"));

        Assert.AreEqual("address", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullEmailAddress_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Person(
                    1,
                    "John",
                    "Doe",
                    CreateAddress(),
                    null!));

        Assert.AreEqual("emailAddress", exception.ParamName);
    }
}