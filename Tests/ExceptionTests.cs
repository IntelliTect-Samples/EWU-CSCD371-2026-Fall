using Assignment;

namespace Tests;

public class ExceptionTests
{
    // ---- Wrapped exceptions keep the original as InnerException ----

    [Theory]
    [InlineData("4,Dan,Brown,dan@example.com,400 Elm St,Olympia,WA")]
    [InlineData("abc,Fay,Lee,fay@example.com,600 Fir St,Olympia,WA,98501")]
    [InlineData("0,Gus,King,gus@example.com,700 Oak St,Olympia,WA,98501")]
    [InlineData("9,,Park,ivy@example.com,900 Cove St,Olympia,WA,98501")]
    public void Import_InvalidRecord_PreservesFormatExceptionAsInnerException(string invalidRecord)
    {
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader(invalidRecord)));

        var exception = Assert.Throws<InvalidPersonRecordException>(() => importer.Import());

        var inner = Assert.IsType<FormatException>(exception.InnerException);
        Assert.Contains(inner.Message, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Import_SkipInvalid_ReportedExceptionPreservesInnerException()
    {
        Exception? reported = null;
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader("abc,Fay,Lee,fay@example.com,600 Fir St,Olympia,WA,98501")));

        importer.Import(exception => reported = exception);

        var recordException = Assert.IsType<InvalidPersonRecordException>(reported);
        Assert.IsType<FormatException>(recordException.InnerException);
    }

    // ---- Argument validation identifies the right parameter ----

    [Fact]
    public void Constructor_NullFilePath_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("filePath", () => new PeopleCSVImporter((string)null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_BlankFilePath_ThrowsArgumentException(string blankPath)
    {
        Assert.Throws<ArgumentException>("filePath", () => new PeopleCSVImporter(blankPath));
    }

    [Fact]
    public void Constructor_NullReader_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("reader", () => new PeopleCSVImporter((TextReader)null!));
    }

    [Fact]
    public void Import_NullCallback_ThrowsArgumentNullException()
    {
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader()));

        Assert.Throws<ArgumentNullException>("onInvalidRecord", () => importer.Import(null!));
    }

    [Fact]
    public void InvalidPersonRecordException_ZeroLineNumber_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            "lineNumber", () => new InvalidPersonRecordException("message", 0, "raw line"));
    }

    [Fact]
    public void InvalidPersonRecordException_NullRawLine_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            "rawLine", () => new InvalidPersonRecordException("message", 2, null!));
    }

    [Fact]
    public void Person_NullFirstName_ThrowsArgumentNullException()
    {
        var address = new Address("1 Main St", "Spokane", "WA", "99201");

        Assert.Throws<ArgumentNullException>(
            "firstName", () => new Person(null!, "Smith", "a@example.com", address));
    }

    [Fact]
    public void Address_NullCity_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("city", () => new Address("1 Main St", null!, "WA", "99201"));
    }

    // ---- The custom exception's standard constructors ----

    [Fact]
    public void InvalidPersonRecordException_StandardConstructors_SetMessageAndInnerException()
    {
        var inner = new FormatException("bad");

        var withDefault = new InvalidPersonRecordException();
        var withMessage = new InvalidPersonRecordException("message");
        var withInner = new InvalidPersonRecordException("message", inner);

        Assert.False(string.IsNullOrEmpty(withDefault.Message));
        Assert.Equal("message", withMessage.Message);
        Assert.Same(inner, withInner.InnerException);
    }

    [Fact]
    public void InvalidPersonRecordException_LineConstructor_SetsLineNumberRawLineAndInner()
    {
        var inner = new FormatException("bad");

        var exception = new InvalidPersonRecordException("message", 7, "raw line", inner);

        Assert.Equal(7, exception.LineNumber);
        Assert.Equal("raw line", exception.RawLine);
        Assert.Same(inner, exception.InnerException);
    }

    // ---- Reader and callback exceptions propagate as the same instance ----

    [Fact]
    public void Import_FailFastReaderThrowsIOException_PropagatesSameInstance()
    {
        var ioException = new IOException("Simulated disk failure.");
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice), ioException));

        var thrown = Assert.Throws<IOException>(() => importer.Import());

        Assert.Same(ioException, thrown);
    }

    [Fact]
    public void Import_SkipInvalidReaderThrowsIOException_PropagatesSameInstanceWithoutReporting()
    {
        var ioException = new IOException("Simulated disk failure.");
        var reportCount = 0;
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice), ioException));

        var thrown = Assert.Throws<IOException>(() => importer.Import(_ => reportCount++));

        Assert.Same(ioException, thrown);
        Assert.Equal(0, reportCount);
    }

    [Fact]
    public void Import_CallbackThrows_PropagatesSameInstance()
    {
        var callbackException = new InvalidOperationException("Callback failed.");
        using var importer = new PeopleCSVImporter(new FakeTextReader(
            TestCsv.WithHeader("0,Gus,King,gus@example.com,700 Oak St,Olympia,WA,98501")));

        var thrown = Assert.Throws<InvalidOperationException>(
            () => importer.Import(_ => throw callbackException));

        Assert.Same(callbackException, thrown);
    }
}
