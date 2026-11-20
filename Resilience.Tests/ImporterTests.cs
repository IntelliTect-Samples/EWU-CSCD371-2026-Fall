namespace Resilience.Tests;

[TestClass]
public class ImporterTests
{
    private const string Header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
    private const string GoodCsv =
           "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
           "2, Doe, John, john@test.com, 123 Main, Cheney, WA, 99004\n" +
           "3, James, Smith, james@test.com, 456 Main, Cheney, WA, 99004";

    private const string GoodRow1 = "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    private const string GoodRow2 = "2, Jim, Doe, john@test.com, 456 Main, Cheney, WA, 99004";
    private const string GoodRow3 = "3, John, Smith, john@test.com, 321 Main, Cheney, WA, 99004";
    private const string RowMissingFirstArgument = "John, Doe, john@test.com, 123 Main, Cheney, WA, 99204";
    private const string RowMissingLastArgument = "4, John, Doe, john@test.com, 123 Main, Cheney, WA";
    private const string RowWithInvalidStringID = "one, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    private const string RowWithZeroID = "0, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    private const string RowWithNegativeID = "-1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004";
    private const string BadCsv = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\nbad";

    [TestMethod]
    public void Constructor_NullFilename_Throws()
    {
        string s = null!;
        Importer importer;
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => importer = new(s));
        Assert.AreEqual("fileName", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullTextReader_Throws()
    {
        TextReader t = null!;
        Importer importer;
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(() => importer = new(t));
        Assert.AreEqual("textReader", exception.ParamName);
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
        StringReader sr = new StringReader(Header + "\n" + GoodCsv);

        using (Importer importer = new Importer(sr))
        {
            importer.Import(ImportMode.FailFast);
        }

        Assert.Throws<ObjectDisposedException>(() => sr.ReadLine());
    }

    [TestMethod]
    public void Import_Unsuccessful_DisposesOfTextReader()
    {
        var sr = new StringReader(BadCsv);

        using (Importer importer = new Importer(sr))
        {
            var ex = Assert.Throws<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));
        }

        Assert.Throws<ObjectDisposedException>(() => sr.ReadLine());
    }

    [TestMethod]
    public void Import_ReaderException_Propagates()
    {
        ThrowingTextReader reader = new();
        using Importer importer = new(reader);
        IOException exception = Assert.Throws<IOException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.AreSame(exception, reader.Exception);
        Assert.AreEqual("Simulated I/O failure.", exception.Message);
    }

    [TestMethod]
    public void Import_CallbackException_Propagates()
    {
        const string fileContents = Header + "\n" + RowWithInvalidStringID;
        const string exceptionMessage = "Exception";
        InvalidOperationException expected = new(exceptionMessage);

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        InvalidOperationException actual = Assert.Throws<InvalidOperationException>(
            () => importer.Import(ImportMode.SkipInvalid, null, onInvalidRecord: _ => throw expected));

        Assert.AreSame(expected, actual);
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
    [DataRow(GoodRow1, RowWithInvalidStringID, GoodRow2, 3)]
    [DataRow(RowMissingLastArgument, GoodRow2, GoodRow3, 2)]
    [DataRow(GoodRow1, RowMissingLastArgument, RowMissingFirstArgument, 3)]
    public void Import_BadLineFormat_Throws(string row1, string row2, string row3, int badLineNumber)
    {
        string fileContents = Header + "\n" + row1 + "\n" + row2 + "\n" + row3;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.IsNotNull(exception.InnerException);
        Assert.AreEqual(badLineNumber, exception.LineNumber);
        Assert.IsInstanceOfType<FormatException>(exception.InnerException);
    }

    [TestMethod]
    [DataRow(RowWithZeroID)]
    [DataRow(RowWithNegativeID)]
    public void Import_OutOfRange_Throws(string row1)
    {
        string fileContents = Header + "\n" + row1 + "\n";

        StringReader sr = new(fileContents);
        using Importer importer = new(sr);
        var ex = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.AreEqual(2, ex.LineNumber);
        Assert.IsInstanceOfType<ArgumentOutOfRangeException>(ex.InnerException);
    }

    [TestMethod]
    public void Import_DuplicateId_Throws()
    {
        string fileContents = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "2, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));
        Assert.IsNotNull(exception.InnerException);
        Assert.AreEqual(4, exception.LineNumber);
        Assert.IsInstanceOfType<ArgumentException>(exception.InnerException);
    }


    [TestMethod]
    [DataRow(GoodRow1)]
    [DataRow(RowMissingLastArgument)]
    public void Import_SkipInvalid_DelegateCalled(string badLine)
    {
        string fileContents = Header + "\n"
            + GoodCsv + "\n" +
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
    public void Import_SkipInvalid_ReportsEveryRejectedRecord()
    {
        string fileContents = Header + "\n" + GoodRow1
            + "\n" + GoodRow2 + "\n" + RowMissingFirstArgument
            + "\n" + RowMissingLastArgument + "\n" + GoodRow3;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        List<InvalidPersonRecordException> exceptions = [];

        var people = importer.Import(ImportMode.SkipInvalid,
            (InvalidPersonRecordException ex) =>
            {
                exceptions.Add(ex);
            });

        Assert.HasCount(2, exceptions);
        Assert.HasCount(3, people);

        Assert.AreEqual(4, exceptions[0].LineNumber);
        Assert.AreEqual(RowMissingFirstArgument, exceptions[0].RawLine);
        Assert.AreEqual(5, exceptions[1].LineNumber);
        Assert.AreEqual(RowMissingLastArgument, exceptions[1].RawLine);

        Assert.AreEqual(1, people[0].ID);
        Assert.AreEqual(2, people[1].ID);
        Assert.AreEqual(3, people[2].ID);
    }

    [TestMethod]
    public void Import_SkipInvalid_NullThrows()
    {
        string fileContents = Header + "\n" + GoodCsv;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        ArgumentException exception = Assert.Throws<ArgumentNullException>(
            () => importer.Import(ImportMode.SkipInvalid));

        Assert.AreEqual("onInvalidRecord", exception.ParamName);
    }

    [TestMethod]
    public void Import_TestValidationDelegate_UnhandledExceptionsPropagate()
    {
        string fileContents = Header + "\n" + GoodCsv;

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
        string fileContents = Header + "\n" + GoodCsv;

        using StringReader reader = new(fileContents);
        using Importer importer = new(reader);

        List<InvalidPersonRecordException> exceptions = [];

        IReadOnlyList<Person> people =
            importer.Import(ImportMode.SkipInvalid,
                person => (person.ID % 2 == 0, "bad row!"), exceptions.Add);

        Assert.HasCount(1, people);
        Assert.AreEqual(2, people[0].ID);

        Assert.HasCount(2, exceptions);
        Assert.AreEqual(2, exceptions[0].LineNumber);
        Assert.AreEqual(4, exceptions[1].LineNumber);

        Assert.AreEqual("bad row!", exceptions[0].InnerException?.Message);
        Assert.AreEqual("bad row!", exceptions[1].InnerException?.Message);
    }

    [TestMethod]
    public void Import_TestValidationDelegate_FailFast()
    {
        string fileContents = Header + "\n" + GoodCsv;

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var ex = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast, person =>
            {
                return (person.ID % 2 == 0, "bad row!");
            }, null));

        Assert.IsInstanceOfType<InvalidPersonRecordException>(ex);
        Assert.AreEqual("bad row!", ex.InnerException!.Message);
    }

    [TestMethod]
    public void Import_CalledTwice_Throws()
    {
        string csv =
            Header +
            "\n" +
            GoodRow1;

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
            Header +
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
            Header +
            "\n" +
            "1, ,Doe,john@test.com,123 Main,Cheney,WA,99004\n" +
            "1,John,Doe,john@test.com,123 Main,Cheney,WA,99004";

        using StringReader sr = new(csv);
        using Importer importer = new(sr);

        IReadOnlyList<Person> people = importer.Import(
            ImportMode.SkipInvalid, errors.Add);

        Assert.HasCount(1, people);

        Assert.AreEqual(1, people[0].ID);

        Assert.HasCount(1, errors);
    }
    [TestMethod]
    public void Import_FailFast_ReportsFirstBadLine()
    {
        string csv =
            Header +
            "\n" +
            GoodRow1 +
            "\n" +
            RowMissingLastArgument +
            "\n" +
            RowWithInvalidStringID;

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