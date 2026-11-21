using Assignment;

using CancellationTokenSource cts = new();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

string user = args.Length > 0 ? args[0] : "your-name-here";
string outputPath = args.Length > 1 ? args[1] : "people-enriched.csv";

using HttpClient http = new()
{
    BaseAddress = new Uri("https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io"),
    Timeout = TimeSpan.FromSeconds(5)
};

PeopleEnricherService service = new(http);
RunSummary summary;
try
{
    summary = await service.RunAsync(user, outputPath, cts.Token);
}
catch (OperationCanceledException) when (cts.IsCancellationRequested)
{
    Console.Error.WriteLine("Cancelled. No output written.");
    return 130;
}

Console.WriteLine($"Wrote {summary.Written} people to {outputPath}");
foreach ((string reason, int count) in summary.RejectionCounts)
{
    Console.WriteLine($"  removed {count}: {reason}");
}

return 0;
