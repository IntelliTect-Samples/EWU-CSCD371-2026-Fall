using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class PeopleEnrichmentRunnerTests
{
    [TestMethod]
    public async Task RunAsyncWritesEnrichedPeopleAndReturnsReport()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string outputPath = Path.Combine(directory, "people.csv");

            Person jane = CreatePerson(1);
            Person john = CreatePerson(2);

            RejectedPerson invalidRow = new(
                RejectionStage.PeopleCsv,
                4,
                "Invalid CSV row",
                "EmailAddress is invalid.");

            FakePeopleApiClient api = new(
                (_, _) => Task.FromResult(new PeopleCsvParseResult(
                    [jane, john],
                    [invalidRow])),
                (zipCode, _) =>
                {
                    if (zipCode == john.ZipCode)
                    {
                        return Task.FromException<ZipLocation>(
                            new HttpRequestException("ZIP lookup failed."));
                    }

                    return Task.FromResult(new ZipLocation("SPOKANE", "WA"));
                });

            PeopleEnrichmentService service = new(api);

            const string expectedCsv =
                "FirstName,LastName,EmailAddress,ZipCode,City,State\r\n" +
                "First1,Last1,person1@example.com,00001,SPOKANE,WA";

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter(expectedCsv));

            PeopleEnrichmentRunner runner = new(service, writer);

            EnrichmentRunResult result = await runner.RunAsync(
                "test-user",
                outputPath);

            string actualCsv = await File.ReadAllTextAsync(outputPath);

            Assert.AreEqual(expectedCsv, actualCsv);
            Assert.AreEqual(3, result.PeopleReceived);
            Assert.AreEqual(1, result.PeopleWritten);
            Assert.AreEqual(2, result.RejectionReport.TotalRejected);
            Assert.HasCount(2, result.RejectionReport.Details);
            Assert.AreEqual(
                Path.GetFullPath(outputPath),
                result.OutputPath);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task RunAsyncPeopleFeedFailurePreservesExistingOutput()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string outputPath = Path.Combine(directory, "people.csv");

            await File.WriteAllTextAsync(
                outputPath,
                "Original contents");

            FakePeopleApiClient api = new(
                (_, _) => Task.FromException<PeopleCsvParseResult>(
                    new HttpRequestException("People feed unavailable.")),
                (_, _) => throw new InvalidOperationException(
                    "ZIP lookup should not occur."));

            PeopleEnrichmentService service = new(api);

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter("New contents"));

            PeopleEnrichmentRunner runner = new(service, writer);

            await Assert.ThrowsExactlyAsync<HttpRequestException>(
                async () => await runner.RunAsync(
                    "test-user",
                    outputPath));

            string actual = await File.ReadAllTextAsync(outputPath);

            Assert.AreEqual("Original contents", actual);
            Assert.IsEmpty(Directory.GetFiles(directory, "*.tmp"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public async Task RunAsyncCancellationDoesNotCreateOutput()
    {
        string directory = CreateTemporaryDirectory();

        try
        {
            string outputPath = Path.Combine(directory, "people.csv");

            FakePeopleApiClient api = new(
                (_, _) => Task.FromResult(new PeopleCsvParseResult(
                    Array.Empty<Person>(),
                    Array.Empty<RejectedPerson>())),
                (_, _) => Task.FromResult(new ZipLocation("SPOKANE", "WA")));

            PeopleEnrichmentService service = new(api);

            AtomicCsvFileWriter writer = new(
                new StubCsvFormatter("New contents"));

            PeopleEnrichmentRunner runner = new(service, writer);

            using CancellationTokenSource cancellationSource = new();
            cancellationSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(
                async () => await runner.RunAsync(
                    "test-user",
                    outputPath,
                    cancellationSource.Token));

            Assert.IsFalse(File.Exists(outputPath));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static Person CreatePerson(int number)
    {
        return new Person(
            $"First{number}",
            $"Last{number}",
            $"person{number}@example.com",
            number.ToString("D5"));
    }

    private static string CreateTemporaryDirectory()
    {
        string directory = Path.Combine(
            Path.GetTempPath(),
            $"PeopleEnricherRunnerTests-{Guid.NewGuid():N}");

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

    private sealed class FakePeopleApiClient(
        Func<string, CancellationToken, Task<PeopleCsvParseResult>> getPeople,
        Func<string, CancellationToken, Task<ZipLocation>> getLocation)
        : IPeopleApiClient
    {
        public Task<PeopleCsvParseResult> GetPeopleAsync(
            string user,
            CancellationToken cancellationToken = default)
        {
            return getPeople(user, cancellationToken);
        }

        public Task<ZipLocation> GetZipLocationAsync(
            string zipCode,
            CancellationToken cancellationToken = default)
        {
            return getLocation(zipCode, cancellationToken);
        }
    }
}