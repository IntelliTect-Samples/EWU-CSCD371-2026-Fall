using System.IO;
using PeopleImporter;

namespace PeopleImporter.Tests;

[TestClass]
public sealed class PeopleImporterLifecycleTests
{
    [TestMethod]
    public void ConstructorNullReaderThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new PeopleImporter(null!));

        Assert.AreEqual("reader", exception.ParamName);
    }

    [TestMethod]
    public void DisposeCanBeCalledMultipleTimes()
    {
        TrackingStringReader trackingReader = new("test");

        PeopleImporter importer = new(trackingReader);

        importer.Dispose();
        importer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(
            () => trackingReader.ReadLine());

        Assert.AreEqual(1, trackingReader.DisposeCount);    
    }

    [TestMethod]
    public void ImportAfterDisposeThrowsObjectDisposedException()
    {
        PeopleImporter importer = new(
            new StringReader(string.Empty));

        importer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(
            () =>
            {
                importer.Import(ImportMode.FailFast);
            });
    }

    private sealed class TrackingStringReader : StringReader
    {
        public TrackingStringReader(string text)
            : base(text)
        {
        }

        public int DisposeCount { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCount++;
            }

            base.Dispose(disposing);
        }
    }

    private sealed class ThrowingTextReader : TextReader
    {
        private readonly IOException _exception;

        public ThrowingTextReader(IOException exception)
        {
            _exception = exception;
        }

        public int DisposeCount { get; private set; }

        public override string? ReadLine()
        {
            throw _exception;
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

    [TestMethod]
    public void ImportEmptyCsvWithHeaderReturnsEmptyListAndDisposesReader()
    {
        TrackingStringReader reader = new(
            "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip");

        PeopleImporter importer = new(reader);

        IReadOnlyList<Person> people =
            importer.Import(ImportMode.FailFast);

        Assert.IsEmpty(people);
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void ImportInvalidModeThrowsArgumentOutOfRangeExceptionAndDisposesReader()
    {
        TrackingStringReader reader = new(
            "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip");

        PeopleImporter importer = new(reader);

        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => importer.Import((ImportMode)999));

        Assert.AreEqual("mode", exception.ParamName);
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void ImportSkipInvalidWithoutCallbackThrowsArgumentNullExceptionAndDisposesReader()
    {
        TrackingStringReader reader = new(
            "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip");

        PeopleImporter importer = new(reader);

        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => importer.Import(ImportMode.SkipInvalid));

        Assert.AreEqual("onInvalidRecord", exception.ParamName);
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void ImportInvalidHeaderThrowsCustomExceptionAndDisposesReader()
    {
        TrackingStringReader reader = new("Wrong,Header");
        PeopleImporter importer = new(reader);

        InvalidPersonRecordException exception =
            Assert.ThrowsExactly<InvalidPersonRecordException>(
                () => importer.Import(ImportMode.FailFast));

        Assert.AreEqual(1, exception.LineNumber);
        Assert.AreEqual("Wrong,Header", exception.RawLine);
        Assert.AreEqual(1, reader.DisposeCount);
    }

    [TestMethod]
    public void ImportIOExceptionPropagatesSameExceptionAndDisposesReader()
    {
        IOException expectedException = new("Simulated I/O failure.");

        ThrowingTextReader reader = new(expectedException);
        PeopleImporter importer = new(reader);

        IOException actualException =
            Assert.ThrowsExactly<IOException>(
                () => importer.Import(ImportMode.FailFast));

        Assert.AreSame(expectedException, actualException);
        Assert.AreEqual(1, reader.DisposeCount);
    }
}