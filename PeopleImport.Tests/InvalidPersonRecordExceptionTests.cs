using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PeopleImport.Tests;

[TestClass]
public class InvalidPersonRecordExceptionTests
{
    [TestMethod]
    public void Constructor_WithoutArguments_ProvidesDefaultMessage()
    {
        InvalidPersonRecordException exception = new();

        Assert.AreEqual<string>("The person record is invalid.", exception.Message);
    }

    [TestMethod]
    public void Constructor_WithMessage_PreservesMessage()
    {
        InvalidPersonRecordException exception = new("Invalid record.");

        Assert.AreEqual<string>("Invalid record.", exception.Message);
    }

    [TestMethod]
    public void Constructor_WithInnerException_PreservesOriginalException()
    {
        FormatException inner = new("Invalid ID.");

        InvalidPersonRecordException exception = new("Invalid record.", inner);

        Assert.AreEqual<string>("Invalid record.", exception.Message);
        Assert.AreSame(inner, exception.InnerException);
    }

    [TestMethod]
    public void Constructor_WithInvalidLineNumber_IdentifiesLineNumberArgument()
    {
        ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new InvalidPersonRecordException(0, "record", "Invalid record."));

        Assert.AreEqual<string>("lineNumber", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_WithNullRawLine_IdentifiesRawLineArgument()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new InvalidPersonRecordException(1, null!, "Invalid record."));

        Assert.AreEqual<string>("rawLine", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_WithBlankContextMessage_IdentifiesMessageArgument()
    {
        ArgumentException exception = Assert.ThrowsExactly<ArgumentException>(
            () => new InvalidPersonRecordException(1, "record", " "));

        Assert.AreEqual<string>("message", exception.ParamName);
    }
}
