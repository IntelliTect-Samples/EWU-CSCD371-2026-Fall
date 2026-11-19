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

    private sealed class TrackingReader(string text) : StringReader(text)
    {
        public int DisposeCount { get; private set; }

        protected override void Dispose(bool disposing)
        {
            DisposeCount++;
            base.Dispose(disposing);
        }
    }

    [TestMethod]
    public void Import_AllValid_ReturnsAllPeople()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv(ValidLine1, ValidLine2));

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
        using var importer = new PeopleImporter(Csv(ValidLine1, "bad line", "also bad"));

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
        using var importer = new PeopleImporter(Csv(OverflowIdLine));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.IsInstanceOfType<OverflowException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_FailFast_WrongFieldCount_PreservesFormatInnerException()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv(WrongFieldCountLine));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.IsInstanceOfType<FormatException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_SkipInvalid_ReturnsOnlyValidAndReportsEachRejected()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv(
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
    [DataRow(ImportMode.FailFast)]
    [DataRow(ImportMode.SkipInvalid)]
    public void Import_WrongHeader_ThrowsOnLineOneInEitherMode(ImportMode mode)
    {
        // Arrange
        const string wrongHeader = "Id,LastName,FirstName,Email,StreetAddress,City,State,Zip";
        using var importer = new PeopleImporter(new StringReader(wrongHeader + Environment.NewLine + ValidLine1));
        int callbackCount = 0;

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(
            () => importer.Import(mode, _ => callbackCount++));

        // Assert
        Assert.AreEqual(1, ex.LineNumber);
        Assert.AreEqual(wrongHeader, ex.RawLine);
        Assert.IsInstanceOfType<FormatException>(ex.InnerException);
        Assert.AreEqual(0, callbackCount);
    }

    [TestMethod]
    public void Import_HeaderMissing_FirstLineIsRecord_Throws()
    {
        // Arrange: a file with no header must not silently drop its first record.
        using var importer = new PeopleImporter(new StringReader(ValidLine1 + Environment.NewLine + ValidLine2));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.AreEqual(1, ex.LineNumber);
        Assert.AreEqual(ValidLine1, ex.RawLine);
    }

    [TestMethod]
    public void Import_EmptyInput_Throws()
    {
        // Arrange
        using var importer = new PeopleImporter(new StringReader(string.Empty));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.AreEqual(1, ex.LineNumber);
        Assert.IsInstanceOfType<FormatException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_HeaderOnly_ReturnsEmptyList()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv());

        // Act
        IReadOnlyList<Person> people = importer.Import(ImportMode.FailFast);

        // Assert
        Assert.IsEmpty(people);
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
        using var importer = new PeopleImporter(Csv(ValidLine1));

        // Act
        var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => importer.Import((ImportMode)42));

        // Assert
        Assert.AreEqual("mode", ex.ParamName);
    }

    [TestMethod]
    public void Import_SkipInvalidWithoutCallback_ThrowsArgumentNullException()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv(ValidLine1));

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
        using var importer = new PeopleImporter(new ThrowingReader(expected));

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
        using var importer = new PeopleImporter(Csv(WrongFieldCountLine));

        // Act
        var actual = Assert.ThrowsExactly<InvalidOperationException>(
            () => importer.Import(ImportMode.SkipInvalid, _ => throw expected));

        // Assert
        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void ImportFromFile_TempFile_ReturnsPeopleAndFileIsReleased()
    {
        // Arrange
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(path, [PersonRecordParser.Header, ValidLine1, ValidLine2]);

            // Act
            IReadOnlyList<Person> people = PeopleImporter.ImportFromFile(path, ImportMode.FailFast);

            // Assert
            Assert.HasCount(2, people);
            File.Delete(path); // would throw on Windows if the file were still open
        }
        finally
        {
            File.Delete(path);
        }
        Assert.IsFalse(File.Exists(path));
    }

    [TestMethod]
    public void ImportFromFile_InvalidRecord_ThrowsAndFileIsReleased()
    {
        // Arrange
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(path, [PersonRecordParser.Header, WrongFieldCountLine]);

            // Act
            var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => PeopleImporter.ImportFromFile(path, ImportMode.FailFast));

            // Assert
            Assert.AreEqual(2, ex.LineNumber);
        }
        finally
        {
            File.Delete(path);
        }
        Assert.IsFalse(File.Exists(path));
    }

    [TestMethod]
    public void ImportFromFile_BlankPath_ThrowsArgumentException()
    {
        // Act
        var ex = Assert.ThrowsExactly<ArgumentException>(() => PeopleImporter.ImportFromFile(" ", ImportMode.FailFast));

        // Assert
        Assert.AreEqual("path", ex.ParamName);
    }

    [TestMethod]
    public void Dispose_AfterSuccessfulImport_DisposesReader()
    {
        // Arrange
        var reader = new TrackingReader(PersonRecordParser.Header + Environment.NewLine + ValidLine1);
        var importer = new PeopleImporter(reader);
        importer.Import(ImportMode.FailFast);

        // Act
        importer.Dispose();

        // Assert
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Dispose_AfterFailedImport_DisposesReader()
    {
        // Arrange
        var reader = new TrackingReader(PersonRecordParser.Header + Environment.NewLine + WrongFieldCountLine);
        using (var importer = new PeopleImporter(reader))
        {
            Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));
        }

        // Assert
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Dispose_CalledRepeatedly_DisposesReaderOnlyOnce()
    {
        // Arrange
        var reader = new TrackingReader(PersonRecordParser.Header);
        var importer = new PeopleImporter(reader);

        // Act
        importer.Dispose();
        importer.Dispose();
        importer.Dispose();

        // Assert
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Import_AfterDispose_ThrowsObjectDisposedException()
    {
        // Arrange
        var importer = new PeopleImporter(Csv(ValidLine1));
        importer.Dispose();

        // Act & Assert
        Assert.ThrowsExactly<ObjectDisposedException>(() => importer.Import(ImportMode.FailFast));
    }

    [TestMethod]
    public void Import_ValidatorRejectsPerson_FailFast_ThrowsWithLineNumber()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv(ValidLine1, ValidLine2));

        // Act
        var ex = Assert.ThrowsExactly<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast, validator: p => p.Id != 2));

        // Assert
        Assert.AreEqual(3, ex.LineNumber);
        Assert.IsInstanceOfType<FormatException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_ValidatorRejectsPerson_SkipInvalid_ReportsAndContinues()
    {
        // Arrange
        using var importer = new PeopleImporter(Csv(ValidLine1, ValidLine2));
        List<InvalidPersonRecordException> rejected = [];

        // Act
        IReadOnlyList<Person> people = importer.Import(ImportMode.SkipInvalid, rejected.Add, p => p.Id != 1);

        // Assert
        Assert.HasCount(1, people);
        Assert.AreEqual(2, people[0].Id);
        Assert.HasCount(1, rejected);
        Assert.AreEqual(2, rejected[0].LineNumber);
    }

    [TestMethod]
    public void Import_ValidatorThrows_PropagatesSameInstance()
    {
        // Arrange
        var expected = new InvalidOperationException("Validator failed.");
        using var importer = new PeopleImporter(Csv(ValidLine1));

        // Act
        var actual = Assert.ThrowsExactly<InvalidOperationException>(
            () => importer.Import(ImportMode.FailFast, validator: _ => throw expected));

        // Assert
        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void Import_ValidatorThrowsFormatException_IsNotCaughtByRecordFilter()
    {
        // Arrange: a FormatException from the delegate is a bug in the delegate, not a bad record.
        var expected = new FormatException("Delegate bug.");
        using var importer = new PeopleImporter(Csv(ValidLine1));

        // Act
        var actual = Assert.ThrowsExactly<FormatException>(
            () => importer.Import(ImportMode.SkipInvalid, _ => { }, _ => throw expected));

        // Assert
        Assert.AreSame(expected, actual);
    }
}
