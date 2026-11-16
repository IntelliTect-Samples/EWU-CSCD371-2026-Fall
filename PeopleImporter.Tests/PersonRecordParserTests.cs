using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PeopleImporter.Tests;

[TestClass]
public sealed class PersonRecordParserTests
{
    [TestMethod]
    public void Parse_ValidRecord_ReturnsPerson()
    {
        const string record =
            "1,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";

        Person person = PersonRecordParser.Parse(record, 2);

        Assert.AreEqual(1, person.ID);
        Assert.AreEqual("John", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("john.doe@example.com", person.EmailAddress);

        Assert.IsNotNull(person.Address);
        Assert.AreEqual("123 Main St", person.Address.StreetAddress);
        Assert.AreEqual("Seattle", person.Address.City);
        Assert.AreEqual("WA", person.Address.State);
        Assert.AreEqual("98101", person.Address.Zip);
    }

    [TestMethod]
    public void Parse_TrimsWhitespace_ReturnsTrimmedValues()
    {
        const string record =
            "1, John , Doe , john.doe@example.com , 123 Main St , Seattle , WA , 98101 ";

        Person person = PersonRecordParser.Parse(record, 2);

        Assert.AreEqual("John", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("john.doe@example.com", person.EmailAddress);
        Assert.AreEqual("123 Main St", person.Address.StreetAddress);
        Assert.AreEqual("Seattle", person.Address.City);
    }

    [TestMethod]
    public void Parse_NonNumericId_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "ABC,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";


        InvalidPersonRecordException exception =
            Assert.Throws<InvalidPersonRecordException>(
                () => PersonRecordParser.Parse(record, 5));

        Assert.IsInstanceOfType(exception.InnerException, typeof(FormatException));
    }

    [TestMethod]
    public void Parse_ZeroId_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "0,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";

        InvalidPersonRecordException exception =
            Assert.Throws<InvalidPersonRecordException>(
                () => PersonRecordParser.Parse(record, 6));

        Assert.IsInstanceOfType(
            exception.InnerException,
            typeof(ArgumentOutOfRangeException));
    }

    [TestMethod]
    public void Parse_MissingFirstName_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "1,,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";

        Assert.Throws<InvalidPersonRecordException>(
            () => PersonRecordParser.Parse(record, 7));
    }

    [TestMethod]
    public void Parse_MissingLastName_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "1,John,,john.doe@example.com,123 Main St,Seattle,WA,98101";

        Assert.Throws<InvalidPersonRecordException>(
            () => PersonRecordParser.Parse(record, 8));
    }

    [TestMethod]
    public void Parse_MissingEmail_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "1,John,Doe,,123 Main St,Seattle,WA,98101";

        Assert.Throws<InvalidPersonRecordException>(
            () => PersonRecordParser.Parse(record, 9));
    }

    [TestMethod]
    public void Parse_MissingStreetAddress_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "1,John,Doe,john.doe@example.com,,Seattle,WA,98101";

        Assert.Throws<InvalidPersonRecordException>(
            () => PersonRecordParser.Parse(record, 10));
    }

    [TestMethod]
    public void Parse_TooFewColumns_ThrowsInvalidPersonRecordException()
    {
        const string record = "1,John,Doe";

        Assert.Throws<InvalidPersonRecordException>(
            () => PersonRecordParser.Parse(record, 11));
    }

    [TestMethod]
    public void Parse_TooManyColumns_ThrowsInvalidPersonRecordException()
    {
        const string record =
            "1,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101,Extra";

        Assert.Throws<InvalidPersonRecordException>(
            () => PersonRecordParser.Parse(record, 12));
    }

    [TestMethod]
    public void Parse_ExceptionContainsLineNumber()
    {
        const string record =
            "ABC,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";

        InvalidPersonRecordException exception =
            Assert.Throws<InvalidPersonRecordException>(
                () => PersonRecordParser.Parse(record, 42));

        Assert.AreEqual(42, exception.LineNumber);
    }

    [TestMethod]
    public void Parse_ExceptionContainsRawLine()
    {
        const string record =
            "ABC,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";

        InvalidPersonRecordException exception =
            Assert.Throws<InvalidPersonRecordException>(
                () => PersonRecordParser.Parse(record, 42));

        Assert.AreEqual(record, exception.RawLine);
    }

    [TestMethod]
    public void Parse_InvalidId_PreservesInnerException()
    {
        const string record =
            "ABC,John,Doe,john.doe@example.com,123 Main St,Seattle,WA,98101";

        InvalidPersonRecordException exception =
            Assert.Throws<InvalidPersonRecordException>(
                () => PersonRecordParser.Parse(record, 42));

        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType(
            exception.InnerException,
            typeof(FormatException));
    }
}