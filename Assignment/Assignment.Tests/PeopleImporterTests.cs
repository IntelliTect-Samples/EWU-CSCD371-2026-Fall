namespace Assignment.Tests;

[TestClass]
public class PeopleImporterTests
{
    private const string ValidLine1 = "1,John,Doe,jdoe@example.com,123 Main St,Anytown,WA,12345";
    private const string ValidLine2 = "2,Jane,Smith,jsmith@example.com,456 Oak St,Somewhere,CA,67890";
    private const string WrongFieldCountLine = "3,Too,Few";
    private const string OverflowIdLine = "99999999999,Big,Id,big@example.com,1 Elm St,Bigtown,TX,11111";
    private const string MissingFirstNameLine = "4,,Nofirst,nofirst@example.com,2 Elm St,Smalltown,OR,22222";

    private static StringReader Csv(params string[] records) =>
        new(string.Join(Environment.NewLine, [PersonRecordParser.Header, .. records]));

    private sealed class ThrowingReader(Exception toThrow) : TextReader
    {
        public override string? ReadLine() => throw toThrow;
    }

    [TestMethod]
    public void Import_AllValid_ReturnsAllPeople()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(ValidLine1, ValidLine2));

        // Act
        IReadOnlyList<Person> people = importer.Import(ImportMode.FailFast);

        // Assert
        Assert.HasCount(2, people);
        Assert.AreEqual("John", people[0].FirstName);
        Assert.AreEqual("Smith", people[1].LastName);
        Assert.AreEqual("Anytown", people[0].Address.City);
    }

    [TestMethod]
    public void Import_FailFast_ReportsFirstBadLineNumber()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(ValidLine1, "bad line", "also bad"));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.AreEqual(3, ex.LineNumber);
        Assert.AreEqual("bad line", ex.RawLine);
    }

    [TestMethod]
    public void Import_FailFast_OverflowId_PreservesOverflowInnerException()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(OverflowIdLine));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.IsInstanceOfType<OverflowException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_FailFast_WrongFieldCount_PreservesFormatInnerException()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(WrongFieldCountLine));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.IsInstanceOfType<FormatException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_SkipInvalid_ReturnsOnlyValidAndReportsEachRejected()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(
            ValidLine1,            // line 2
            WrongFieldCountLine,   // line 3
            OverflowIdLine,        // line 4
            ValidLine2,            // line 5
            MissingFirstNameLine)); // line 6
        List<InvalidPersonRecordException> rejected = [];

        // Act
        IReadOnlyList<Person> people = importer.Import(ImportMode.SkipInvalid, e => rejected.Add(e));

        // Assert
        int[] expectedIds = [1, 2];
        int[] expectedRejectedLines = [3, 4, 6];
        string[] expectedRejectedText = [WrongFieldCountLine, OverflowIdLine, MissingFirstNameLine];
        CollectionAssert.AreEqual(expectedIds, people.Select(p => p.Id).ToArray());
        CollectionAssert.AreEqual(expectedRejectedLines, rejected.Select(e => e.LineNumber).ToArray());
        CollectionAssert.AreEqual(expectedRejectedText, rejected.Select(e => e.RawLine).ToArray());
    }

    [TestMethod]
    public void Constructor_NullReader_ThrowsArgumentNullException()
    {
        // Act
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => new PeopleImporter(null!));

        // Assert
        Assert.AreEqual("reader", ex.ParamName);
    }

    [TestMethod]
    public void Import_UndefinedMode_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(ValidLine1));

        // Act
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => importer.Import((ImportMode)42));

        // Assert
        Assert.AreEqual("mode", ex.ParamName);
    }

    [TestMethod]
    public void Import_SkipInvalidWithoutCallback_ThrowsArgumentNullException()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(ValidLine1));

        // Act
        var ex = Assert.ThrowsExactly<ArgumentNullException>(() => importer.Import(ImportMode.SkipInvalid));

        // Assert
        Assert.AreEqual("onInvalidRecord", ex.ParamName);
    }

    [TestMethod]
    public void Import_ReaderThrowsIOException_PropagatesSameInstance()
    {
        // Arrange
        var expected = new IOException("Simulated disk failure.");
        var importer = new PeopleImporter(new ThrowingReader(expected));

        // Act
        var actual = Assert.ThrowsExactly<IOException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void Import_CallbackThrows_PropagatesSameInstance()
    {
        // Arrange
        var expected = new InvalidOperationException("Callback failed.");
        var importer = new PeopleImporter(Csv(WrongFieldCountLine));

        // Act
        var actual = Assert.ThrowsExactly<InvalidOperationException>(
            () => importer.Import(ImportMode.SkipInvalid, _ => throw expected));

        // Assert
        Assert.AreSame(expected, actual);
    }
}
