using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PeopleImport.Tests;

[TestClass]
public class PeopleImporterTests
{
    private const string Header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
    private const string Ada = "1,Ada,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123";
    private const string Grace = "2,Grace,Hopper,grace@example.com,2 Main St,Portland,OR,97201";

    [TestMethod]
    public void Import_ValidRows_MapsPeopleAndDisposesOwnedReaderAfterUsing()
    {
        TrackingReader reader = new(Header, Ada, Grace);
        IReadOnlyList<Person> people;
        using (PeopleImporter importer = new(reader))
        {
            people = importer.Import();
        }

        Assert.HasCount(2, people);
        Assert.AreEqual<int>(1, people[0].Id);
        Assert.AreEqual<string>("Ada", people[0].FirstName);
        Assert.AreEqual<string>("Lovelace", people[0].LastName);
        Assert.AreEqual<string>("ada@example.com", people[0].EmailAddress);
        Assert.AreEqual<string>("1 Main St", people[0].Address.StreetAddress);
        Assert.AreEqual<string>("Spokane", people[0].Address.City);
        Assert.AreEqual<string>("WA", people[0].Address.State);
        Assert.AreEqual<string>("00123", people[0].Address.Zip);
        Assert.AreEqual<string>("Grace", people[1].FirstName);
        Assert.AreEqual<int>(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Import_TemporaryFile_ReadsRecordsAndReleasesFile()
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllLines(path, [Header, Ada]);
            using (PeopleImporter importer = new(File.OpenText(path)))
            {
                Assert.AreEqual<string>("Ada", Assert.ContainsSingle(importer.Import()).FirstName);
            }

            using FileStream reopened = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            Assert.IsTrue(reopened.CanWrite);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    [DataRow("broken", typeof(FormatException))]
    [DataRow("wrong,Ada,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123", typeof(FormatException))]
    [DataRow("2147483648,Ada,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123", typeof(OverflowException))]
    [DataRow("0,Ada,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123", typeof(ArgumentOutOfRangeException))]
    [DataRow("1, ,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123", typeof(ArgumentException))]
    public void Import_InvalidRow_PreservesParsingExceptionAndRecordContext(string row, Type innerType)
    {
        using PeopleImporter importer = new(new StringReader($"{Header}\n{row}"));

        InvalidPersonRecordException exception = Assert.ThrowsExactly<InvalidPersonRecordException>(() => importer.Import());

        Assert.AreEqual<int>(2, exception.LineNumber);
        Assert.AreEqual<string>(row, exception.RawLine);
        Assert.IsNotNull(exception.InnerException);
        Assert.AreEqual<Type>(innerType, exception.InnerException.GetType());
        Assert.Contains("line 2", exception.Message);
    }

    [TestMethod]
    public void Import_FailFast_FilterDoesNotHandleFirstRecordErrorAndUsingDisposesReader()
    {
        TrackingReader reader = new(Header, Ada, "broken", "also broken", Grace);
        int reports = 0;
        InvalidPersonRecordException exception;
        using (PeopleImporter importer = new(reader))
        {
            exception = Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast, _ => reports++));
        }

        Assert.AreEqual<int>(3, exception.LineNumber);
        Assert.AreEqual<string>("broken", exception.RawLine);
        Assert.AreEqual<int>(3, reader.ReadCount);
        Assert.AreEqual<int>(0, reports);
        Assert.AreEqual<int>(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Import_SkipInvalid_ReportsEveryBadRecordAndKeepsValidPeople()
    {
        const string badId = "wrong,Ada,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123";
        const string missingName = "3, ,Lovelace,ada@example.com,1 Main St,Spokane,WA,00123";
        using PeopleImporter importer = new(new StringReader(string.Join('\n', Header, Ada, "broken", badId, missingName, Grace)));
        List<InvalidPersonRecordException> errors = [];

        IReadOnlyList<Person> people = importer.Import(ImportMode.SkipInvalid, error => errors.Add(error));

        Assert.IsTrue(people.Select(person => person.Id).SequenceEqual(new[] { 1, 2 }));
        Assert.IsTrue(errors.Select(error => error.LineNumber).SequenceEqual(new[] { 3, 4, 5 }));
        Assert.IsTrue(errors.Select(error => error.RawLine).SequenceEqual(new[] { "broken", badId, missingName }));
        Assert.IsTrue(errors.All(error => error.InnerException is not null));
    }

    [TestMethod]
    public void Import_ReaderFailureAfterValidRecord_PropagatesSameExceptionAndDisposesReader()
    {
        IOException failure = new("The reader failed.");
        TrackingReader reader = new(Header, Ada, Grace) { ReadFailure = failure };
        IOException actual;
        using (PeopleImporter importer = new(reader))
        {
            actual = Assert.ThrowsExactly<IOException>(() => importer.Import(ImportMode.SkipInvalid, _ => { }));
        }

        Assert.AreSame(failure, actual);
        Assert.AreEqual<int>(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Import_CallbackFailure_PropagatesSameExceptionAndDisposesReader()
    {
        InvalidPersonRecordException failure = new("The callback failed.");
        TrackingReader reader = new(Header, "broken", Grace);
        InvalidPersonRecordException actual;
        using (PeopleImporter importer = new(reader))
        {
            actual = Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.SkipInvalid, _ => throw failure));
        }

        Assert.AreSame(failure, actual);
        Assert.AreEqual<int>(2, reader.ReadCount);
        Assert.AreEqual<int>(1, reader.DisposeCount);
    }

    [TestMethod]
    public void Import_ValidationRule_FailFastRejectsFirstViolation()
    {
        using PeopleImporter importer = new(new StringReader($"{Header}\n{Ada}\n{Grace}"));

        InvalidPersonRecordException exception = Assert.ThrowsExactly<InvalidPersonRecordException>(
            () => importer.Import(validate: person => person.Id > 1));

        Assert.AreEqual<int>(2, exception.LineNumber);
        Assert.AreEqual<string>(Ada, exception.RawLine);
    }

    [TestMethod]
    public void Import_ValidationRule_SkipInvalidReportsViolationAndContinues()
    {
        using PeopleImporter importer = new(new StringReader($"{Header}\n{Ada}\n{Grace}"));
        List<InvalidPersonRecordException> errors = [];

        IReadOnlyList<Person> people = importer.Import(ImportMode.SkipInvalid,
            error => errors.Add(error), person => person.Id > 1);

        Assert.AreEqual<string>("Grace", Assert.ContainsSingle(people).FirstName);
        Assert.AreEqual<int>(2, Assert.ContainsSingle(errors).LineNumber);
    }

    [TestMethod]
    public void Import_UnexpectedValidationException_IsNotWrappedOrHandled()
    {
        FormatException failure = new("The validation delegate failed unexpectedly.");
        using PeopleImporter importer = new(new StringReader($"{Header}\n{Ada}"));
        int reports = 0;

        FormatException actual = Assert.ThrowsExactly<FormatException>(() => importer.Import(
            ImportMode.SkipInvalid, _ => reports++, _ => throw failure));

        Assert.AreSame(failure, actual);
        Assert.AreEqual<int>(0, reports);
    }

    [TestMethod]
    public void Constructor_WithNullReader_IdentifiesReaderArgument()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(() => new PeopleImporter(null!));

        Assert.AreEqual<string>("reader", exception.ParamName);
    }

    [TestMethod]
    public void Import_WithUnknownMode_IdentifiesModeArgument()
    {
        using PeopleImporter importer = new(new StringReader(Header));

        ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => importer.Import((ImportMode)(-1)));

        Assert.AreEqual<string>("mode", exception.ParamName);
    }

    [TestMethod]
    public void Import_SkipInvalidWithoutCallback_IdentifiesReportErrorArgument()
    {
        using PeopleImporter importer = new(new StringReader(Header));

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => importer.Import(ImportMode.SkipInvalid));

        Assert.AreEqual<string>("reportError", exception.ParamName);
    }

    [TestMethod]
    public void Dispose_CalledRepeatedly_DisposesReaderOnceAndPreventsImport()
    {
        TrackingReader reader = new(Header, Ada);
        using PeopleImporter importer = new(reader);

        importer.Dispose();
        importer.Dispose();

        Assert.AreEqual<int>(1, reader.DisposeCount);
        Assert.ThrowsExactly<ObjectDisposedException>(() => importer.Import());
    }

    [TestMethod]
    public void Import_DuplicateId_ThrowsInvalidPersonRecordException()
    {
        using PeopleImporter importer = new(new StringReader($"{Header}\n{Ada}\n{Ada}"));

        InvalidPersonRecordException exception =
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import());
        
        Assert.AreEqual<int>(3, exception.LineNumber);
        Assert.AreEqual<string>(Ada, exception.RawLine);
    }

    [TestMethod]
    public void Import_SkipInvalidWithDuplicateId_CapturesAndSkipsInvalid()
    {
        using PeopleImporter importer = new(new StringReader($"{Header}\n{Ada}\n{Ada}\n{Grace}"));
        
        List<InvalidPersonRecordException> errors = [];
        
        IReadOnlyList<Person> people = importer.Import(ImportMode.SkipInvalid, errors.Add);
        
        Assert.IsTrue(people.Select(person => person.Id).SequenceEqual([1, 2]));
        
        Assert.AreEqual<int>(1, errors.Count);
        Assert.AreEqual<int>(3, errors[0].LineNumber);
        Assert.AreEqual<string>(Ada, errors[0].RawLine);
    }

    private sealed class TrackingReader(params string[] rows) : StringReader(string.Join('\n', rows))
    {
        public int ReadCount { get; private set; }
        public int DisposeCount { get; private set; }
        public IOException? ReadFailure { get; init; }

        public override string? ReadLine()
        {
            ReadCount++;
            if (ReadCount == 3 && ReadFailure is not null)
            {
                throw ReadFailure;
            }

            return base.ReadLine();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCount++;
            }

            base.Dispose(disposing);
        }
    }
}
