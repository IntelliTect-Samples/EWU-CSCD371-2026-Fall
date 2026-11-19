namespace Resilience.Tests;

[TestClass]
public class ImporterTests
{
    const string _header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
    const string _goodRows =
           "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
           "2, Doe, John, john@test.com, 123 Main, Cheney, WA, 99004\n" +
           "3, James, Smith, james@test.com, 456 Main, Cheney, WA, 99004";

    const string _goodRow = "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    const string _rowMissingFirstArgument = "John, Doe, john@test.com, 123 Main, Cheney, WA, 99204";
    const string _rowMissingLastArgument = "4, John, Doe, john@test.com, 123 Main, Cheney, WA";
    const string _rowWithInvalidStringID = "one, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    const string _rowWithZeroID = "0, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    const string _rowWithNegativeID = "-1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    const string _badCsv = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\nbad";

    [TestMethod]
    public void Constructor_NullFilename_Throws()
    {
        string s = null!;
        Importer importer;
        Assert.Throws<ArgumentNullException>(() => importer = new(s));
    }

    [TestMethod]
    public void Constructor_NullTextReader_Throws()
    {
        TextReader t = null!;
        Importer importer;
        Assert.Throws<ArgumentNullException>(() => importer = new(t));
    }

    [TestMethod]
    public void Dispose_IsSafeToCallRepeatedly()
    {
        using Importer importer =
            new(new StringReader(
                "Id,FirstName,LastName,StreetAddress,City,State,Zip\n"));

        importer.Dispose();
        importer.Dispose();

        Assert.Throws<ObjectDisposedException>(
            () => importer.Import(ImportMode.FailFast));
    }

    [TestMethod]
    public void Import_Successful_DisposesOfTextReader()
    {
        StringReader sr = new StringReader(_header + "\n" + _goodRows);

        using (Importer importer = new Importer(sr))
        {
            importer.Import(ImportMode.FailFast);
        }

        Assert.Throws<ObjectDisposedException>(() => sr.ReadLine());
    }

    [TestMethod]
    public void Import_Unsuccessful_DisposesOfTextReader()
    {
        var sr = new StringReader(_badCsv);

        using (Importer importer = new Importer(sr))
        {
            var ex = Assert.Throws<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));
        }

        Assert.Throws<ObjectDisposedException>(() => sr.ReadLine());
    }

    [TestMethod]
    public void Import_ReaderException_Propagates()
    {
        using Importer importer = new(new ThrowingTextReader());
        IOException exception = Assert.Throws<IOException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.AreEqual("Simulated I/O failure.", exception.Message);
    }

    [TestMethod]
    public void Import_ReadsFromFile()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "1,John,Doe,john@test.com,123 Main,Cheney,WA,99004\n");

            using Importer importer = new(path);
            IReadOnlyList<Person> people = importer.Import(ImportMode.FailFast);
            Assert.HasCount(1, people);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    [DataRow(_rowWithInvalidStringID)]
    [DataRow(_rowMissingLastArgument)]
    [DataRow(_rowMissingFirstArgument)]
    public void Import_BadLineFormat_Throws(string row1)
    {
        string fileContents = _header + "\n" + row1 + "\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<FormatException>(exception.InnerException);
    }

    [TestMethod]
    [DataRow(_rowWithZeroID)]
    [DataRow(_rowWithNegativeID)]
    public void Import_OutOfRange_Throws(string row1)
    {
        string fileContents = _header + "\n" + row1 + "\n";

        StringReader sr = new(fileContents);
        using Importer importer = new(sr);
        var ex = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.IsInstanceOfType<ArgumentOutOfRangeException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_DuplicateId_Throws()
    {
        string fileContents = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<ArgumentException>(exception.InnerException);
    }


    [TestMethod]
    [DataRow(_goodRow)]
    [DataRow(_rowMissingLastArgument)]
    public void Import_SkipInvalid_DelegateCalled(string badLine)
    {
        string fileContents = _header + "\n"
            + _goodRows + "\n" +
            badLine + "\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        bool importDelegateInvoked = false;
        int lineNumberActual = 0;
        string rawLineActual = "";
        int lineNumberExpected = 5;

        var people = importer.Import(ImportMode.SkipInvalid,
            (InvalidPersonRecordException ex) =>
            {
                importDelegateInvoked = true;
                lineNumberActual = ex.LineNumber;
                rawLineActual = ex.RawLine;
            });

        Assert.IsTrue(importDelegateInvoked);
        Assert.AreEqual(lineNumberExpected, lineNumberActual);
        Assert.AreEqual(badLine, rawLineActual);
        Assert.HasCount(3, people);
    }

    [TestMethod]
    public void Import_SkipInvalid_NullThrows()
    {
        string fileContents = _header + "\n" + _goodRows;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        Assert.Throws<ArgumentNullException>(
            () => importer.Import(ImportMode.SkipInvalid));
    }

    [TestMethod]
    public void Import_TestValidationDelegate_UnhandledExceptionsPropagate()
    {
        string fileContents = _header + "\n" + _goodRows;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        Assert.Throws<IOException>(
            () => importer.Import(ImportMode.SkipInvalid, person =>
            {
                throw new IOException("failed");
            }, ex => { }));
    }

    [TestMethod]
    public void Import_TestValidationDelegate_SkipsInvalid()
    {
        string fileContents = _header + "\n" + _goodRows;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var list = importer.Import(ImportMode.SkipInvalid, person =>
            {
                return (person.ID % 2 == 0, "bad row!");
            }, ex => { });

        Assert.HasCount(1, list);
    }

    [TestMethod]
    public void Import_TestValidationDelegate_FailFast()
    {
        string fileContents = _header + "\n" + _goodRows;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var ex = Assert.Throws<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast, person =>
        {
            return (person.ID % 2 == 0, "bad row!");
        }, ex => { }));

        Assert.IsInstanceOfType<InvalidPersonRecordException>(ex);
        Assert.AreEqual("bad row!", ex.InnerException!.Message);
    }
    [TestMethod]
    public void Import_CalledTwice_Throws()
    {
        string csv =
            _header +
            "\n" +
            _goodRow;

        using StringReader sr = new(csv);
        using Importer importer = new(sr);

        importer.Import(ImportMode.FailFast);

        Assert.Throws<InvalidOperationException>(
            () => importer.Import(ImportMode.FailFast));
    }
    [TestMethod]
    public void Constructor_EmptyFilenameOrWhitespaceFilename_Throws()
    {
        ArgumentException ex =
            Assert.Throws<ArgumentException>(
                () => new Importer(""));
        ArgumentException ex2 =
            Assert.Throws<ArgumentException>(
                () => new Importer("   "));

        Assert.AreEqual(
            "fileName",
            ex.ParamName);
        Assert.AreEqual(
            "fileName",
            ex2.ParamName);

    }
    [TestMethod]
    public void Import_BlankFirstName_Throws()
    {
        string csv =
            _header +
            "\n" +
            "1, ,Doe,john@test.com,123 Main,Cheney,WA,99004";

        using StringReader sr = new(csv);
        using Importer importer = new(sr);

        InvalidPersonRecordException ex =
            Assert.Throws<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));

        Assert.AreEqual(
            2,
            ex.LineNumber);

        Assert.IsInstanceOfType<FormatException>(
            ex.InnerException);
    }
    [TestMethod]
    public void Import_InvalidRecordDoesNotReserveId()
    {
        List<InvalidPersonRecordException> errors = [];

        string csv =
            _header +
            "\n" +
            "1, ,Doe,john@test.com,123 Main,Cheney,WA,99004\n" +
            "1,John,Doe,john@test.com,123 Main,Cheney,WA,99004";

        using StringReader sr = new(csv);
        using Importer importer = new(sr);

        IReadOnlyList<Person> people =
    importer.Import(
        ImportMode.SkipInvalid,
        errors.Add);

        Assert.HasCount(1, people);

        Assert.AreEqual(1,people[0].ID);
         
        Assert.HasCount(1, errors);
    }
    [TestMethod]
    public void Import_FailFast_ReportsFirstBadLine()
    {
        string csv =
            _header +
            "\n" +
            _goodRow +
            "\n" +
            _rowMissingLastArgument +
            "\n" +
            _rowWithInvalidStringID;

        using StringReader sr = new(csv);
        using Importer importer = new(sr);

        InvalidPersonRecordException ex =
            Assert.Throws<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));

        Assert.AreEqual(
            3,
            ex.LineNumber);
    }
}