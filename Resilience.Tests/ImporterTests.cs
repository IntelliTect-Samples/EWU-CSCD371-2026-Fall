namespace Resilience.Tests;

[TestClass]
public class ImporterTests
{
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
        TrackingTextReader sr = new(new StringReader(
            "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004"));

        using (Importer importer = new Importer(sr))
        {
            importer.Import(ImportMode.FailFast);
        }

        Assert.IsTrue(sr.IsDisposed);
    }

    [TestMethod]
    public void Import_Unsuccessful_DisposesOfTextReader()
    {
        TrackingTextReader sr = new(new ThrowingTextReader());

        using (Importer importer = new Importer(sr))
        {
            var ex = Assert.Throws<IOException>(() => importer.Import(ImportMode.FailFast));
        }

        Assert.IsTrue(sr.IsDisposed);
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
    [DataRow("one, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004")]
    [DataRow("1, John, Doe, john@test.com, 123 Main, Cheney, WA")]
    [DataRow("John, Doe, 123 Main, Cheney, WA")]
    public void Import_BadLineFormat_Throws(string row1)
    {
        string fileContents = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            row1 + "\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var exception = Assert.Throws<InvalidPersonRecordException>(
            () => importer.Import(ImportMode.FailFast));
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<FormatException>(exception.InnerException);
    }

    [TestMethod]
    [DataRow("0, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004")]
    [DataRow("-1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004")]
    public void Import_OutOfRange_Throws(string row1)
    {
        string fileContents = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            row1 + "\n";

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
    [DataRow("1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004")]
    [DataRow("3, John, Doe, john@test.com, 123 Main, Cheney, WA")]
    public void Import_SkipInvalid_DelegateCalled(string badLine)
    {
        string fileContents =
            "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
            "2, James, Smith, james@test.com, 456 Main, Cheney, WA, 99004\n" +
            badLine + "\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        bool importDelegateInvoked = false;
        int lineNumberActual = 0;
        string rawLineActual = "";
        int lineNumberExpected = 4;

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
        Assert.HasCount(2, people);
    }

    [TestMethod]
    public void Import_SkipInvalid_NullThrows()
    {
        string fileContents =
            "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
            "2, James, Smith, james@test.com, 456 Main, Cheney, WA, 99004\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        Assert.Throws<ArgumentNullException>(
            () => importer.Import(ImportMode.SkipInvalid));
    }


}