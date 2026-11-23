namespace PeopleEnricher.Tests;

[TestClass]
public class PeopleEnricherTests
{
    private const string ValidHeader = "FirstName,LastName,EmailAddress,ZipCode\n";
    private const string EnrichedHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";
    
    private const string SmokeyDataRow = "Smokey,The Bear,smokey@forest.gov,20252";

    #region Enrichment Exception Cases

    [TestMethod]
    public async Task EnrichAsync_WhenCsvDownloadExceedsTimeout_ThrowsEnrichmentException()
    {
        TimeSpan testTimeout = TimeSpan.FromMilliseconds(50);
        const EnrichmentErrorType expectedErrorType = EnrichmentErrorType.CsvDownloadTimeout;
        
        // setup endpoint response for test
        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: async () =>
            {
                await Task.Delay(testTimeout * 2, TestContext.CancellationToken);

                return "csv";
            });
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient, testTimeout);
        
        EnrichmentException exception =
            await Assert.ThrowsExactlyAsync<EnrichmentException>(() =>
                peopleEnricher.EnrichAsync(TestContext.CancellationToken));
        
        Assert.AreEqual(expectedErrorType, exception.ErrorType);
    }

    [TestMethod]
    public async Task EnrichAsync_WhenCsvDownloadThrowsHttpRequestException_WrapsInEnrichmentException()
    {
        const EnrichmentErrorType expectedErrorType = EnrichmentErrorType.CsvDownloadFailed;
        
        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: () => 
                throw new HttpRequestException());
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient);
        
        EnrichmentException exception = 
            await Assert.ThrowsExactlyAsync<EnrichmentException>(() =>
                peopleEnricher.EnrichAsync(TestContext.CancellationToken));
        
        Assert.IsInstanceOfType<HttpRequestException>(exception.InnerException);
        Assert.AreEqual(expectedErrorType, exception.ErrorType);
    }

    [TestMethod]
    public async Task EnrichAsync_WhenCsvDownloadThrowsTimeoutException_WrapsInEnrichmentException()
    {
        const EnrichmentErrorType expectedErrorType = EnrichmentErrorType.CsvDownloadTimeout;
        
        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: () =>
                throw new TimeoutException());
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient);
        
        EnrichmentException exception =
            await Assert.ThrowsExactlyAsync<EnrichmentException>(() =>
                peopleEnricher.EnrichAsync(TestContext.CancellationToken));
        
        Assert.IsInstanceOfType<TimeoutException>(exception.InnerException);
        Assert.AreEqual(expectedErrorType, exception.ErrorType);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("Foo")]
    [DataRow("FirstName,LastName,EmailAddress,ZipCode,Extra")]
    public async Task EnrichAsync_WhenHeaderIsInvalid_ThrowsEnrichmentException(string wrongHeaderCsv)
    {
        const EnrichmentErrorType expectedErrorType = EnrichmentErrorType.InvalidHeader;

        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: () => Task.FromResult(wrongHeaderCsv));
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient);
        
        EnrichmentException exception =
            await Assert.ThrowsExactlyAsync<EnrichmentException>(() =>
                peopleEnricher.EnrichAsync(TestContext.CancellationToken));
        
        Assert.AreEqual(expectedErrorType, exception.ErrorType);
    }

    #endregion

    [TestMethod]
    public async Task EnrichAsync_WhenCsvContainsOnlyValidHeader_ReturnsEnrichedHeader()
    {
        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: () => Task.FromResult(ValidHeader));
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient);
        
        EnrichmentResult result = 
            await peopleEnricher.EnrichAsync(
                TestContext.CancellationToken);
        
        IReadOnlyList<string> enrichedCsvLines = CsvLines(result.EnrichedCsv);
        
        Assert.IsTrue(enrichedCsvLines.SequenceEqual([EnrichedHeader]));
        Assert.IsEmpty(result.RejectedRows);
    }

    [TestMethod]
    public async Task EnrichAsync_WhenDataRowIsValid_AppendsCityAndState()
    {
        const string csv = $"{ValidHeader}" +
                           $"{SmokeyDataRow}";

        const string expectedEnrichedRow = $"{SmokeyDataRow},Washington,DC";
        
        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: () => Task.FromResult(csv),
            zipCodeLookup: ZipCodeLookup);
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient);
        
        EnrichmentResult result = 
            await peopleEnricher.EnrichAsync(
                TestContext.CancellationToken);

        IReadOnlyList<string> enrichedCsvLines = CsvLines(result.EnrichedCsv);
        
        Assert.IsTrue(enrichedCsvLines.SequenceEqual([
            EnrichedHeader, 
            expectedEnrichedRow
        ]));
        
        Assert.IsEmpty(result.RejectedRows);
    }

    [TestMethod]
    public async Task EnrichAsync_WhenDataRowIsInvalid_RejectsDataRow()
    {
        const string invalidSmokeyDataRow =
            $"{SmokeyDataRow},Extra";
        
        const string csv = $"{ValidHeader}" +
                     $"{invalidSmokeyDataRow}";
        
        StubPeopleEndpointClient peopleEndpointClient = new(
            downloadPeopleCsv: () => Task.FromResult(csv),
            zipCodeLookup: ZipCodeLookup);
        
        PeopleEnricher peopleEnricher = new(peopleEndpointClient);
        
        EnrichmentResult result =
            await peopleEnricher.EnrichAsync(
                TestContext.CancellationToken);
        
        Assert.HasCount(1, result.RejectedRows);
        
        RejectedRow rejectedRow = result.RejectedRows.Single();
        
        Assert.AreEqual(2, rejectedRow.LineNumber);
        Assert.AreEqual(invalidSmokeyDataRow, rejectedRow.RawData);
        
        Assert.IsTrue(CsvLines(result.EnrichedCsv).SequenceEqual([EnrichedHeader]));
    }

    public required TestContext TestContext { get; set; }

    private static Task<string> ZipCodeLookup(string zipCode)
    {
        return Task.FromResult(zipCode switch
        {
            "20252" => "\"WASHINGTON\",\"DC\"",
            "99705" => "\"NORTH POLE\",\"AK\"",
            _ => throw new HttpRequestException()
        });
    }

    private static IReadOnlyList<string> CsvLines(string csv) => [.. GetLines(csv)];

    private static IEnumerable<string> GetLines(string csv)
    {
        using StringReader reader = new(csv);

        while (reader.ReadLine() is { } line)
            yield return line;
    }
}