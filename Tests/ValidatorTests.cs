using Assignment;

namespace Tests;

/// <summary>
/// Extra credit: the caller-supplied validation delegate, and the importer's <c>when</c> filter.
/// </summary>
public class ValidatorTests
{
    private const string NoAtSign = "4,Dan,Brown,dan.example.com,400 Elm St,Olympia,WA,98501";

    private static string? RequireAtSign(IPerson person) =>
        person.EmailAddress.Contains('@', StringComparison.Ordinal) ? null : "Email must contain '@'.";

    [Fact]
    public void Import_ValidatorAcceptsEveryone_ReturnsAllPeople()
    {
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice, TestCsv.Bob)))
        {
            Validator = RequireAtSign,
        };

        var people = importer.Import();

        Assert.Equal(2, people.Count);
    }

    [Fact]
    public void Import_FailFastValidatorRejects_ThrowsWithLineAndMessage()
    {
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice, NoAtSign)))
        {
            Validator = RequireAtSign,
        };

        var exception = Assert.Throws<InvalidPersonRecordException>(() => importer.Import());

        Assert.Equal(3, exception.LineNumber);
        Assert.Equal(NoAtSign, exception.RawLine);
        Assert.Contains("Email must contain '@'.", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Import_SkipInvalidValidatorRejects_ReportsAndContinues()
    {
        var reportedLines = new List<int>();
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice, NoAtSign, TestCsv.Bob)))
        {
            Validator = RequireAtSign,
        };

        var people = importer.Import(
            exception => reportedLines.Add(Assert.IsType<InvalidPersonRecordException>(exception).LineNumber));

        string[] expectedNames = ["Alice", "Bob"];
        int[] expectedLines = [3];
        Assert.Equal(expectedNames, people.Select(person => person.FirstName));
        Assert.Equal(expectedLines, reportedLines);
    }

    [Fact]
    public void Import_ValidatorThrowsUnexpectedException_PropagatesSameInstance()
    {
        var unexpected = new InvalidOperationException("Validator bug.");
        var reportCount = 0;
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice)))
        {
            Validator = _ => throw unexpected,
        };

        var thrown = Assert.Throws<InvalidOperationException>(() => importer.Import(_ => reportCount++));

        Assert.Same(unexpected, thrown);
        Assert.Equal(0, reportCount);
    }

    [Fact]
    public void Import_ValidatorThrowsFormatException_IsNotCaughtByParsingFilter()
    {
        // FormatException is a type the importer's `when` filter handles for parsing errors.
        // The filter only covers parsing, so the same type from the validator must not be caught.
        var fromValidator = new FormatException("Thrown by the validator, not by parsing.");
        var reportCount = 0;
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice)))
        {
            Validator = _ => throw fromValidator,
        };

        var thrown = Assert.Throws<FormatException>(() => importer.Import(_ => reportCount++));

        Assert.Same(fromValidator, thrown);
        Assert.Equal(0, reportCount);
    }

    [Fact]
    public void Import_NonMatchingExceptionType_IsNotCaughtByWhenFilter()
    {
        // The `when` filter matches only FormatException and ArgumentException.
        // An IOException in skip-invalid mode must not be reported or wrapped.
        var ioException = new IOException("Simulated disk failure.");
        var reports = new List<Exception>();
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice), ioException));

        var thrown = Assert.Throws<IOException>(() => importer.Import(reports.Add));

        Assert.Same(ioException, thrown);
        Assert.Empty(reports);
    }
}
