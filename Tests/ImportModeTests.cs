using Assignment;

namespace Tests;

public class ImportModeTests
{
    // One example of each kind of invalid record the importer rejects.
    private const string TooFewFields = "4,Dan,Brown,dan@example.com,400 Elm St,Olympia,WA";
    private const string TooManyFields = "5,Eve,Stone,eve@example.com,500 Ash St,Olympia,WA,98501,extra";
    private const string NonNumericId = "abc,Fay,Lee,fay@example.com,600 Fir St,Olympia,WA,98501";
    private const string ZeroId = "0,Gus,King,gus@example.com,700 Oak St,Olympia,WA,98501";
    private const string NegativeId = "-8,Hal,Ross,hal@example.com,800 Bay St,Olympia,WA,98501";
    private const string BlankFirstName = "9,,Park,ivy@example.com,900 Cove St,Olympia,WA,98501";
    private const string BlankLine = "";

    [Theory]
    [InlineData(TooFewFields)]
    [InlineData(TooManyFields)]
    [InlineData(NonNumericId)]
    [InlineData(ZeroId)]
    [InlineData(NegativeId)]
    [InlineData(BlankFirstName)]
    [InlineData(BlankLine)]
    public void Import_FailFastWithInvalidRecord_ThrowsWithLineNumberAndRawLine(string invalidRecord)
    {
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TestCsv.Alice, invalidRecord, TestCsv.Bob)));

        var exception = Assert.Throws<InvalidPersonRecordException>(() => importer.Import());

        Assert.Equal(3, exception.LineNumber);
        Assert.Equal(invalidRecord, exception.RawLine);
    }

    [Fact]
    public void Import_FailFastWithSeveralInvalidRecords_ReportsFirstBadLine()
    {
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader(
            TestCsv.Alice,  // line 2
            TestCsv.Bob,    // line 3
            ZeroId,         // line 4 <- first bad line
            NonNumericId,   // line 5
            TestCsv.Carol))); // line 6

        var exception = Assert.Throws<InvalidPersonRecordException>(() => importer.Import());

        Assert.Equal(4, exception.LineNumber);
        Assert.Equal(ZeroId, exception.RawLine);
    }

    [Fact]
    public void Import_SkipInvalidWithSeveralInvalidRecords_ReturnsOnlyValidPeople()
    {
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader(
            TestCsv.Alice, TooFewFields, NonNumericId, TestCsv.Bob, ZeroId, BlankFirstName, TestCsv.Carol)));

        var people = importer.Import(_ => { });

        string[] expected = ["Alice", "Bob", "Carol"];
        Assert.Equal(expected, people.Select(person => person.FirstName));
    }

    [Fact]
    public void Import_SkipInvalidWithSeveralInvalidRecords_ReportsEveryRejectedRecordInOrder()
    {
        var reports = new List<InvalidPersonRecordException>();
        using var importer = new PeopleCSVImporter(new FakeTextReader(TestCsv.WithHeader(
            TestCsv.Alice,  // line 2
            TooFewFields,   // line 3
            NonNumericId,   // line 4
            TestCsv.Bob,    // line 5
            ZeroId,         // line 6
            NegativeId,     // line 7
            BlankFirstName, // line 8
            BlankLine,      // line 9
            TestCsv.Carol))); // line 10

        // The closure captures the local list, so the test can see every report.
        importer.Import(exception => reports.Add(Assert.IsType<InvalidPersonRecordException>(exception)));

        int[] expectedLines = [3, 4, 6, 7, 8, 9];
        string[] expectedRawLines = [TooFewFields, NonNumericId, ZeroId, NegativeId, BlankFirstName, BlankLine];
        Assert.Equal(expectedLines, reports.Select(report => report.LineNumber));
        Assert.Equal(expectedRawLines, reports.Select(report => report.RawLine));
    }

    [Fact]
    public void Import_SkipInvalidWithOnlyInvalidRecords_ReturnsEmptyAndReportsAll()
    {
        var reportCount = 0;
        using var importer = new PeopleCSVImporter(
            new FakeTextReader(TestCsv.WithHeader(TooFewFields, ZeroId, BlankFirstName)));

        var people = importer.Import(_ => reportCount++);

        Assert.Empty(people);
        Assert.Equal(3, reportCount);
    }
}
