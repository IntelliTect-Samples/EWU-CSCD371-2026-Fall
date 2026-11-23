using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;
using ForceConnector.Core.Services;

CancellationTokenSource cts = new();

HttpClient httpClient = new()
{
    BaseAddress =
        new Uri(
            "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io")
};

IPeopleClient peopleClient =
    new PeopleClient(httpClient);

ILocationClient locationClient =
    new LocationClient(httpClient);

ICsvParser csvParser =
    new CsvParser();

PeopleEnricher enricher =
    new PeopleEnricher(
        peopleClient,
        locationClient,
        csvParser);

try
{
    EnrichmentResult result =
        await enricher.EnrichAsync(
            "Mateo",
            cts.Token);

    await CsvWriter.WriteAsync(
        "people.csv",
        result.People,
        cts.Token);

    Console.WriteLine(
        $"Enriched {result.People.Count} people.");

    Console.WriteLine(
        $"Rejected {result.Rejections.Count} people.");
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Run failed: {ex.Message}");
}