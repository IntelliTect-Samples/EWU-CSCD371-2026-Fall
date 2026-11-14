using Assignment;

namespace PeopleImporter.Tests;

[TestClass]
public sealed class InvalidPersonRecordExceptionTests
{
    [TestMethod]
    public void DefaultConstructorSetsDefaultMessage()
    {
        InvalidPersonRecordException exception = new();

        Assert.AreEqual(
            "An invalid person record was encountered.",
            exception.Message);

        Assert.IsNull(exception.LineNumber);
        Assert.IsNull(exception.RawLine);
    }

    [TestMethod]
    public void MessageConstructorSetsMessage()
    {
        InvalidPersonRecordException exception =
            new("Invalid ID.");

        Assert.AreEqual("Invalid ID.", exception.Message);
    }

    [TestMethod]
    public void MessageAndInnerExceptionConstructorPreservesInnerException()
    {
        FormatException innerException = new("Invalid integer.");

        InvalidPersonRecordException exception =
            new("Unable to parse person record.", innerException);

        Assert.AreSame(innerException, exception.InnerException);
        Assert.AreEqual("Unable to parse person record.", exception.Message);
    }

    [TestMethod]
    public void RecordConstructorSetsRecordInformation()
    {
        InvalidPersonRecordException exception = new(
            "Invalid person ID.",
            3,
            "abc,John,Doe,john@example.com,123 Main St,Anytown,CA,12345");

        Assert.AreEqual(3, exception.LineNumber);
        Assert.AreEqual(
            "abc,John,Doe,john@example.com,123 Main St,Anytown,CA,12345",
            exception.RawLine);
        Assert.AreEqual("Invalid person ID.", exception.Message);
        Assert.IsNull(exception.InnerException);
    }

    [TestMethod]
    public void RecordConstructorWithInnerExceptionPreservesInnerException()
    {
        FormatException innerException = new("Invalid integer.");

        InvalidPersonRecordException exception = new(
            "Unable to parse person ID.",
            2,
            "abc,John,Doe,john@example.com,123 Main St,Anytown,CA,12345",
            innerException);

        Assert.AreEqual(2, exception.LineNumber);
        Assert.AreSame(innerException, exception.InnerException);
    }

    [TestMethod]
    public void RecordConstructorZeroLineNumberThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new InvalidPersonRecordException(
                    "Invalid record.",
                    0,
                    "invalid"));

        Assert.AreEqual("lineNumber", exception.ParamName);
    }

    [TestMethod]
    public void RecordConstructorNegativeLineNumberThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new InvalidPersonRecordException(
                    "Invalid record.",
                    -1,
                    "invalid"));

        Assert.AreEqual("lineNumber", exception.ParamName);
    }

    [TestMethod]
    public void RecordConstructorNullRawLineThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new InvalidPersonRecordException(
                    "Invalid record.",
                    2,
                    null!));

        Assert.AreEqual("rawLine", exception.ParamName);
    }

    [TestMethod]
    public void RecordConstructorWhitespaceMessageThrowsArgumentException()
    {
        ArgumentException exception =
            Assert.ThrowsExactly<ArgumentException>(
                () => new InvalidPersonRecordException(
                    " ",
                    2,
                    "invalid"));

        Assert.AreEqual("message", exception.ParamName);
    }
}