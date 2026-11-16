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

        Assert.Throws<ObjectDisposedException>(() => importer.Import(ImportMode.FailFast));
    }

    [TestMethod]
    public void Import_ReaderException_Propagates()
    {
        using Importer importer = new(new ThrowingTextReader());
        IOException exception = Assert.Throws<IOException>(() => importer.Import(ImportMode.FailFast));

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

        var exception = Assert.Throws<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<FormatException>(exception.InnerException);
    }

    [TestMethod]
    public void Import_DuplicateId_Throws()
    {
        string fileContents = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n" +
            "1, John, Doe, john@test.com, 123 Main, Cheney, WA, 99004\n";

        StringReader sr = new(fileContents);

        using Importer importer = new(sr);

        var exception = Assert.Throws<InvalidPersonRecordException>(() => importer.Import(ImportMode.FailFast));
        Assert.IsNotNull(exception.InnerException);
        Assert.IsInstanceOfType<ArgumentException>(exception.InnerException);
    }
}