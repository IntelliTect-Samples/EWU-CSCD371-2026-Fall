using PeopleEnricher;

PeopleEndpointClient peopleEndpointClient = new();

PeopleEnricher.PeopleEnricher peopleEnricher = 
    new(peopleEndpointClient);

try
{
    EnrichmentResult result =
        await peopleEnricher.EnrichAsync();
    
    Console.WriteLine("Success!");
    
    Console.WriteLine($"Rejected Rows: {result.RejectedRows.Count}");
}
catch (Exception exception) when (exception is EnrichmentException)
{
    Console.WriteLine(
        $"Enrichment Failed: {exception.Message}");
}