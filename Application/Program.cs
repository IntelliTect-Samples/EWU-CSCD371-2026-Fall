using PeopleEnricher;

using HttpClient httpClient = new();
PeopleEndpointClient peopleEndpointClient = new(httpClient);
PeopleEnricher.PeopleEnricher peopleEnricher = new(peopleEndpointClient);
using CancellationTokenSource cancellation = new();
ConsoleCancelEventHandler cancelHandler = (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};
Console.CancelKeyPress += cancelHandler;

try
{
    string outputPath = args.Length > 0 ? args[0] : "enriched-people.csv";
    EnrichmentResult result = await peopleEnricher.EnrichToFileAsync(outputPath, cancellation.Token);
    Console.WriteLine($"Success! Wrote {Path.GetFullPath(outputPath)}");
    Console.WriteLine($"Rejected Rows: {result.RejectedRows.Count}");

    foreach (var group in result.RejectedRows.GroupBy(row => row.Reason))
    {
        Console.WriteLine($"{group.Key}: {group.Count()}");
    }

    foreach (RejectedRow rejectedRow in result.RejectedRows)
    {
        Console.WriteLine();
        Console.WriteLine($"Line {rejectedRow.LineNumber}: {rejectedRow.RawData}");
        Console.WriteLine($"    - {rejectedRow.Reason}");
    }
}
catch (EnrichmentException exception)
{
    Console.Error.WriteLine($"Enrichment Failed: {exception.Message}");
    Environment.ExitCode = 1;
}
catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
{
    Console.Error.WriteLine("Enrichment cancelled.");
    Environment.ExitCode = 1;
}
finally
{
    Console.CancelKeyPress -= cancelHandler;
}
