using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PeopleImporterTests
{
    private const string ValidLine1 =
        "1,John,Doe,john@example.com,123 Main St,Spokane,WA,99201";

    private const string ValidLine2 =
        "2,Jane,Smith,jane@example.com,456 Oak St,Seattle,WA,98101";

    private static string CreateCsv(params string[] records)
    {
        return PersonRecordParser.Header
            + Environment.NewLine
            + string.Join(Environment.NewLine, records);
    }

    [TestMethod]
    public void Import_ValidRecords_ReturnsPeople()
    {
        // Arrange
        using var reader = new StringReader(
            CreateCsv(ValidLine1, ValidLine2));

        using var importer = new PeopleImporter(reader);

        // Act
        var people = importer.Import(ImportMode.FailFast);

        // Assert
        Assert.HasCount(2, people);
        Assert.AreEqual(1, people[0].Id);
        Assert.AreEqual("John", people[0].FirstName);
        Assert.AreEqual("Spokane", people[0].Address.City);
        Assert.AreEqual("Jane", people[1].FirstName);
    }

    [TestMethod]
    public void Import_FailFast_ThrowsOnFirstInvalidRecord()
    {
        // Arrange
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                ValidLine1,
                "3,Bad,Record",
                ValidLine2)));

        // Act
        var exception =
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.AreEqual(3, exception.LineNumber);
        Assert.AreEqual("3,Bad,Record", exception.RawLine);
        Assert.IsInstanceOfType<FormatException>(
            exception.InnerException);
    }

    [TestMethod]
    public void Import_SkipInvalid_ReturnsOnlyValidRecords()
    {
        // Arrange
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                ValidLine1,
                "3,Bad,Record",
                "-1,Invalid,Id,bad@example.com,1 St,City,WA,99201",
                ValidLine2,
                "2,Duplicate,Id,dup@example.com,1 St,City,WA,99201",
                "4,,MissingName,test@example.com,1 St,City,WA,99201")));

        List<InvalidPersonRecordException> errors = new();

        // Act
        var people = importer.Import(
            ImportMode.SkipInvalid,
            error => errors.Add(error));

        // Assert
        Assert.HasCount(2, people);
        Assert.HasCount(4, errors);

        Assert.AreEqual("John", people[0].FirstName);
        Assert.AreEqual("Jane", people[1].FirstName);

        Assert.AreEqual(3, errors[0].LineNumber);
        Assert.AreEqual(4, errors[1].LineNumber);
        Assert.AreEqual(6, errors[2].LineNumber);
        Assert.AreEqual(7, errors[3].LineNumber);
    }

    [TestMethod]
    public void Import_InvalidId_PreservesInnerException()
    {
        // Arrange
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                "abc,John,Doe,john@example.com,1 St,City,WA,99201")));

        // Act
        var exception =
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));

        // Assert
        Assert.AreEqual(2, exception.LineNumber);
        Assert.IsInstanceOfType<FormatException>(
            exception.InnerException);
    }

    [TestMethod]
    public void Import_OverflowId_PreservesOverflowException()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                "99999999999,John,Doe,john@example.com,1 St,City,WA,99201")));

        var exception =
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));

        Assert.IsInstanceOfType<OverflowException>(
            exception.InnerException);
    }

    [TestMethod]
    public void Import_NullCallback_ThrowsArgumentNullException()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(ValidLine1)));

        var exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => importer.Import(ImportMode.SkipInvalid));

        Assert.AreEqual("onInvalidRecord", exception.ParamName);
    }

    [TestMethod]
    public void Import_InvalidMode_ThrowsArgumentOutOfRangeException()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(ValidLine1)));

        var exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => importer.Import((ImportMode)99));

        Assert.AreEqual("mode", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullReader_ThrowsArgumentNullException()
    {
        var exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new PeopleImporter(null!));

        Assert.AreEqual("reader", exception.ParamName);
    }

    [TestMethod]
    public void Import_CallbackException_PropagatesUnchanged()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv("Invalid,Record")));

        var expected = new InvalidOperationException(
            "Callback failed.");

        var actual =
            Assert.ThrowsExactly<InvalidOperationException>(
                () => importer.Import(
                    ImportMode.SkipInvalid,
                    error => throw expected));

        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void Import_ReaderIOException_PropagatesUnchanged()
    {
        var expected = new IOException("Reading failed.");

        using var importer = new PeopleImporter(
            new ThrowingReader(expected));

        var actual = Assert.ThrowsExactly<IOException>(
            () => importer.Import(ImportMode.FailFast));

        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var importer = new PeopleImporter(
            new StringReader(CreateCsv(ValidLine1)));

        importer.Dispose();
        importer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(
            () => importer.Import(ImportMode.FailFast));
    }

    [TestMethod]
    public void Dispose_ReaderClosedAfterSuccess()
    {
        var reader = new TrackingReader(CreateCsv(ValidLine1));

        using (var importer = new PeopleImporter(reader))
        {
            var people = importer.Import(ImportMode.FailFast);
            Assert.HasCount(1, people);
        }

        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Dispose_ReaderClosedAfterFailure()
    {
        var reader = new TrackingReader(CreateCsv("Bad,Record"));

        using (var importer = new PeopleImporter(reader))
        {
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));
        }

        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void ImportFromFile_ValidFile_ReturnsPeople()
    {
        string path = Path.Combine(
            Path.GetTempPath(),
            $"Assignment8-{Guid.NewGuid():N}.csv");

        try
        {
            File.WriteAllText(path, CreateCsv(ValidLine1));

            var people = PeopleImporter.ImportFromFile(
                path, ImportMode.FailFast);

            Assert.HasCount(1, people);
            Assert.AreEqual("John", people[0].FirstName);
        }
        finally
        {
            File.Delete(path);
        }
    }

    private sealed class ThrowingReader : TextReader
    {
        private readonly IOException _exception;

        public ThrowingReader(IOException exception)
        {
            _exception = exception;
        }

        public override string? ReadLine()
        {
            throw _exception;
        }
    }

    private sealed class TrackingReader : StringReader
    {
        public int DisposeCount { get; private set; }

        public TrackingReader(string text) : base(text)
        {
        }

        protected override void Dispose(bool disposing)
        {
            DisposeCount++;
            base.Dispose(disposing);
        }
    }

    [TestMethod]
    public void Import_CustomValidation_AcceptsValidPeople()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(ValidLine1, ValidLine2)));

        var people = importer.Import(
            ImportMode.FailFast,
            validator: person => person.Address.State == "WA");

        Assert.HasCount(2, people);
        Assert.AreEqual("John", people[0].FirstName);
        Assert.AreEqual("Jane", people[1].FirstName);
    }

    [TestMethod]
    public void Import_CustomValidation_SkipInvalidReportsFailures()
    {
        string invalidLine1 =
            "3,Sara,Jones,sara@example.com,10 Main St,Portland,OR,97201";

        string invalidLine2 =
            "4,Lee,Brown,lee@example.com,20 Main St,Boise,ID,83701";

        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                ValidLine1,
                invalidLine1,
                ValidLine2,
                invalidLine2)));

        List<InvalidPersonRecordException> errors = new();

        var people = importer.Import(
            ImportMode.SkipInvalid,
            error => errors.Add(error),
            person => person.Address.State == "WA");

        Assert.HasCount(2, people);
        Assert.HasCount(2, errors);

        Assert.AreEqual(3, errors[0].LineNumber);
        Assert.AreEqual(5, errors[1].LineNumber);

        Assert.AreEqual(invalidLine1, errors[0].RawLine);
        Assert.AreEqual(invalidLine2, errors[1].RawLine);

        Assert.IsInstanceOfType<FormatException>(
            errors[0].InnerException);
    }

    [TestMethod]
    public void Import_CustomValidation_FailFastThrows()
    {
        string invalidLine =
            "3,Sara,Jones,sara@example.com,10 Main St,Portland,OR,97201";

        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                ValidLine1,
                invalidLine,
                ValidLine2)));

        var exception =
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(
                    ImportMode.FailFast,
                    validator: person => person.Address.State == "WA"));

        Assert.AreEqual(3, exception.LineNumber);
        Assert.AreEqual(invalidLine, exception.RawLine);

        Assert.IsInstanceOfType<FormatException>(
            exception.InnerException);
    }

    [TestMethod]
    public void Import_ValidatorFormatException_PropagatesUnchanged()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(ValidLine1)));

        var expected = new FormatException(
            "Unexpected validator failure.");

        var actual = Assert.ThrowsExactly<FormatException>(
            () => importer.Import(
                ImportMode.FailFast,
                validator: person => throw expected));

        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void Import_ValidatorUnexpectedException_Propagates()
    {
        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(ValidLine1)));

        var expected = new InvalidOperationException(
            "Validation failed unexpectedly.");

        var actual =
            Assert.ThrowsExactly<InvalidOperationException>(
                () => importer.Import(
                    ImportMode.SkipInvalid,
                    error => { },
                    person => throw expected));

        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void Import_RejectedId_CanBeUsedByLaterValidRecord()
    {
        string rejectedLine =
            "2,Blocked,Person,bad@example.com,1 St,City,WA,99201";

        using var importer = new PeopleImporter(
            new StringReader(CreateCsv(
                rejectedLine,
                ValidLine2)));

        List<InvalidPersonRecordException> errors = new();

        var people = importer.Import(
            ImportMode.SkipInvalid,
            error => errors.Add(error),
            person => person.FirstName != "Blocked");

        Assert.HasCount(1, errors);
        Assert.HasCount(1, people);

        Assert.AreEqual(2, people[0].Id);
        Assert.AreEqual("Jane", people[0].FirstName);
    }
}