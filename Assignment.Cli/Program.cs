using Assignment9;
using Assignment9.Generated;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

// Usage: dotnet run --project Assignment.Cli -- [user] [outputPath]
string user = args.Length > 0 ? args[0] : "trever";
string outputPath = args.Length > 1 ? args[1] : "people-enriched.csv";

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

// One HttpClient for the whole run. The retry policy enforces a per-attempt timeout;
// HttpClient.Timeout is only a backstop. Because we pass our own HttpClient, Kiota does not add its
// own retry middleware, so RetryPolicy is the only retry logic.
using var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(1) };

// The Kiota request adapter for the generated client. It borrows httpClient (does not dispose it).
using var requestAdapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), httpClient: httpClient)
{
    BaseUrl = "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io",
};

var retryPolicy = new RetryPolicy(new RetryOptions
{
    MaxAttempts = 8,
    AttemptTimeout = TimeSpan.FromSeconds(10),
    BaseDelay = TimeSpan.FromMilliseconds(250),
    MaxDelay = TimeSpan.FromSeconds(5),
});

var enricher = new PeopleEnricher(new MayhemMarmotClient(new MayhemMarmotApiClient(requestAdapter)), retryPolicy, maxConcurrentLookups: 10);

try
{
    EnrichmentReport report = await enricher.RunAsync(user, outputPath, cancellation.Token);

    Console.WriteLine($"Downloaded {report.RowsDownloaded} rows for '{user}'.");
    Console.WriteLine($"Wrote {report.Enriched.Count} enriched people to {Path.GetFullPath(outputPath)}.");
    Console.WriteLine($"Removed {report.Rejections.Count} people.");

    foreach (RejectionCount count in report.RejectionCounts)
    {
        Console.WriteLine($"  {count.Count,3} x {count.Reason}");
    }

    foreach (Rejection rejection in report.Rejections)
    {
        Console.WriteLine($"  - {rejection.Subject}: {rejection.Reason}. {rejection.Detail}");
    }

    return 0;
}
catch (Exception exception) when (exception is TimeoutException || RetryPolicy.IsTransient(exception))
{
    // Only the people download can get here (lookup failures remove a single person instead).
    Console.Error.WriteLine($"Run failed: the people CSV could not be downloaded. No file was written. {exception.Message}");
    return 1;
}
catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
{
    Console.Error.WriteLine("Run cancelled. No file was written.");
    return 2;
}
