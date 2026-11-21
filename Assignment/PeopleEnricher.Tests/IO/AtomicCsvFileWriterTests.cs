using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class AtomicCsvFileWriterTests
{
    [TestMethod]
    public async Task WriteAsyncCreatesFormattedCsvFile()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string destination = Path.Combine(directory, "people.csv");
            string expected = "Header\r\nPerson";

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter(expected));

            await writer.WriteAsync(destination, []);

            string actual = await File.ReadAllTextAsync(destination);

            Assert.AreEqual(expected, actual);
            Assert.DoesNotStartWith("\uFEFF", actual);
            Assert.IsEmpty(Directory.GetFiles(directory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task WriteAsyncReplacesExistingFile()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string destination = Path.Combine(directory, "people.csv");

            await File.WriteAllTextAsync(destination, "Old contents");

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter("New contents"));

            await writer.WriteAsync(destination, []);

            string actual = await File.ReadAllTextAsync(destination);

            Assert.AreEqual("New contents", actual);
            Assert.IsEmpty(Directory.GetFiles(directory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task WriteAsyncFormatterFailurePreservesExistingFile()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string destination = Path.Combine(directory, "people.csv");

            await File.WriteAllTextAsync(destination, "Original contents");

            AtomicCsvFileWriter writer = new(
                new ThrowingCsvFormatter());

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await writer.WriteAsync(destination, []));

            string actual = await File.ReadAllTextAsync(destination);

            Assert.AreEqual("Original contents", actual);
            Assert.IsEmpty(Directory.GetFiles(directory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task WriteAsyncCancellationPreservesExistingFile()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string destination = Path.Combine(directory, "people.csv");

            await File.WriteAllTextAsync(destination, "Original contents");

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter("New contents"));

            using CancellationTokenSource cancellationSource = new();
            cancellationSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await writer.WriteAsync(
                    destination,
                    [],
                    cancellationSource.Token));

            string actual = await File.ReadAllTextAsync(destination);

            Assert.AreEqual("Original contents", actual);
            Assert.IsEmpty(Directory.GetFiles(directory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task WriteAsyncRejectsInvalidDestination()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string destination = Path.Combine(
                directory,
                "missing",
                "people.csv");

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter("New contents"));

            await Assert.ThrowsExactlyAsync<DirectoryNotFoundException>(
                async () => await writer.WriteAsync(destination, []));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string CreateTemporaryDirectory()
    {
        string directory = Path.Combine(
            Path.GetTempPath(),
            $"PeopleEnricherTests-{Guid.NewGuid():N}");

        Directory.CreateDirectory(directory);

        return directory;
    }

    private sealed class StubCsvFormatter(string csv)
        : IPeopleCsvFormatter
    {
        public string Format(IEnumerable<EnrichedPerson> people)
        {
            return csv;
        }
    }

    private sealed class ThrowingCsvFormatter : IPeopleCsvFormatter
    {
        public string Format(IEnumerable<EnrichedPerson> people)
        {
            throw new InvalidOperationException(
                "Simulated formatter failure.");
        }
    }
}