using Assignment.Interfaces;

var baseUrl = "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io";
var user = args.Length > 0 ? args[0] : "yoko";
var outputPath = args.Length > 1 ? args[1] : "People-Enriched.csv";

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
};

using var service = new PeopleEnrichmentService(httpClient, disposeHttpClient: false);
var result = await service.RunAsync(user, outputPath).ConfigureAwait(false);

Console.WriteLine($"Wrote {result.EnrichedPeople} enriched people to {outputPath}");
foreach (var summary in result.FormatRejectionSummary())
{
    Console.WriteLine($"Rejected - {summary}");
}
