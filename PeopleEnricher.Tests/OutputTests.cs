namespace PeopleEnricher.Tests;

[TestClass]
public class OutputTests
{
    private const string Csv = "FirstName,LastName,EmailAddress,ZipCode\nAda,Lovelace,ada@example.com,99201";
    public required TestContext TestContext { get; set; }

    [TestMethod]
    public async Task EnrichToFileAsync_Success_WritesResultAndLeavesNoTemporaryFile()
    {
        string directory = CreateDirectory();
        try
        {
            string output = Path.Combine(directory, "people.csv");
            StubPeopleEndpointClient client = new(
                downloadPeopleCsv: () => Task.FromResult(Csv),
                zipCodeLookup: _ => Task.FromResult("\"SPOKANE\",\"WA\""));

            EnrichmentResult result = await new PeopleEnricher(client)
                .EnrichToFileAsync(output, TestContext.CancellationToken);

            Assert.AreEqual(result.EnrichedCsv, await File.ReadAllTextAsync(output, TestContext.CancellationToken));
            Assert.HasCount(1, Directory.GetFiles(directory));
            StringAssert.StartsWith(result.EnrichedCsv, "FirstName,LastName,EmailAddress,ZipCode,City,State");
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task EnrichToFileAsync_FailedOrCancelledRun_PreservesExistingFile(bool cancelled)
    {
        string directory = CreateDirectory();
        try
        {
            string output = Path.Combine(directory, "people.csv");
            await File.WriteAllTextAsync(output, "previous successful output", TestContext.CancellationToken);
            using CancellationTokenSource cancellation =
                CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
            StubPeopleEndpointClient client = new(downloadPeopleCsv: () => throw new HttpRequestException());
            PeopleEnricher enricher = new(client, maxAttempts: 1);
            if (cancelled)
            {
                cancellation.Cancel();
                await Assert.ThrowsAsync<OperationCanceledException>(
                    () => enricher.EnrichToFileAsync(output, cancellation.Token));
            }
            else
            {
                await Assert.ThrowsExactlyAsync<EnrichmentException>(
                    () => enricher.EnrichToFileAsync(output, cancellation.Token));
            }

            Assert.AreEqual("previous successful output",
                await File.ReadAllTextAsync(output, TestContext.CancellationToken));
            Assert.HasCount(1, Directory.GetFiles(directory));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [TestMethod]
    public async Task EnrichToFileAsync_PublicationFails_RemovesTemporaryFile()
    {
        string directory = CreateDirectory();
        try
        {
            string output = Path.Combine(directory, "already-a-directory");
            Directory.CreateDirectory(output);
            StubPeopleEndpointClient client = new(
                downloadPeopleCsv: () => Task.FromResult(Csv),
                zipCodeLookup: _ => Task.FromResult("\"SPOKANE\",\"WA\""));

            Exception? failure = null;
            try
            {
                await new PeopleEnricher(client).EnrichToFileAsync(output, TestContext.CancellationToken);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                failure = exception;
            }
            Assert.IsNotNull(failure);

            Assert.IsEmpty(Directory.GetFiles(directory));
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    private static string CreateDirectory() =>
        Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), $"PeopleEnricher-{Guid.NewGuid():N}")).FullName;
}
