using Assignment;

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

string user = args.Length > 0 ? args[0] : "your-name";
string outputPath = args.Length > 1 ? args[1] : "people-enriched.csv";

using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
var importer = new PeopleImporter(new PeopleApiClient(http));

try
{
    ImportResult result = await importer.ImportAsync(user, outputPath, cts.Token);

    Console.WriteLine($"Wrote {result.WrittenCount} people to {outputPath}");
    foreach (var (reason, count) in result.CountsByReason)
        Console.WriteLine($"  removed {count}: {reason}");

    foreach (Rejection r in result.Rejections)
        Console.WriteLine($"    row {r.Row}: {r.Reason}");

    return 0;
}
catch (OperationCanceledException) when (cts.IsCancellationRequested)
{
    Console.Error.WriteLine("Cancelled. No output written.");
    return 1;
}
