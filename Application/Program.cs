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

    foreach (RejectedRow rejectedRow in result.RejectedRows)
    {
        Console.WriteLine();
        
        Console.WriteLine(
            $"Line {rejectedRow.LineNumber}: {rejectedRow.RawData}");
        
        Console.WriteLine(
            $"    - {rejectedRow.Reason}");
    }
}
catch (Exception exception) when (exception is EnrichmentException)
{
    Console.WriteLine(
        $"Enrichment Failed: {exception.Message}");
}