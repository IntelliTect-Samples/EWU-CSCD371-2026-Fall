using System.Globalization;
using PeopleEnricher;

internal static class Program
{
    private const int DefaultConcurrency = 8;

    public static async Task<int> Main(string[] args)
    {
        if (args.Length == 1 && (args[0] is "--help" or "-h"))
        {
            PrintUsage();
            return 0;
        }

        if (args.Length is < 2 or > 3)
        {
            PrintUsage();
            return 2;
        }

        string user = args[0];
        string outputPath = args[1];

        if (string.IsNullOrWhiteSpace(user))
        {
            Console.Error.WriteLine("A user name or ID is required.");
            return 2;
        }

        int maxConcurrency = DefaultConcurrency;

        if (args.Length == 3 &&
            (!int.TryParse(
                args[2],
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out maxConcurrency) ||
             maxConcurrency < 1))
        {
            Console.Error.WriteLine(
                "Concurrency must be a positive integer.");
            return 2;
        }

        string fullOutputPath;

        try
        {
            fullOutputPath = Path.GetFullPath(outputPath);

            string? outputDirectory =
                Path.GetDirectoryName(fullOutputPath);

            if (outputDirectory is null ||
                !Directory.Exists(outputDirectory))
            {
                Console.Error.WriteLine(
                    "The output directory does not exist.");

                return 2;
            }
        }
        catch (ArgumentException exception)
        {
            Console.Error.WriteLine(
                $"Invalid output path: {exception.Message}");

            return 2;
        }

        // The partner's formatter will be discovered when added
        // to the PeopleEnricher library.
        IPeopleCsvFormatter? formatter = CreateFormatter();

        if (formatter is null)
        {
            Console.Error.WriteLine(
                "PeopleCsvFormatter has not been implemented yet.");

            Console.Error.WriteLine(
                "Your partner needs to add the public " +
                "PeopleCsvFormatter class implementing " +
                "IPeopleCsvFormatter to the PeopleEnricher project.");

            return 2;
        }

        using CancellationTokenSource cancellationSource = new();

        ConsoleCancelEventHandler cancelHandler = (_, eventArgs) =>
        {
            eventArgs.Cancel = true;

            Console.WriteLine(
                "\nCancellation requested. Finishing cleanup...");

            cancellationSource.Cancel();
        };

        Console.CancelKeyPress += cancelHandler;

        try
        {
            Console.WriteLine("===== PEOPLE ENRICHER =====");
            Console.WriteLine($"User: {user}");
            Console.WriteLine($"Output: {fullOutputPath}");
            Console.WriteLine($"Maximum concurrent lookups: {maxConcurrency}");
            Console.WriteLine("Starting enrichment...\n");

            // One HttpClient is shared by every API request.
            // PeopleApiClient manages individual request timeouts.
            using HttpClient httpClient = new()
            {
                Timeout = Timeout.InfiniteTimeSpan
            };

            PeopleApiClient apiClient = new(httpClient);

            PeopleEnrichmentService enrichmentService = new(
                apiClient,
                maxConcurrency);

            AtomicCsvFileWriter fileWriter = new(formatter);

            PeopleEnrichmentRunner runner = new(
                enrichmentService,
                fileWriter);

            EnrichmentRunResult result = await runner.RunAsync(
                user,
                fullOutputPath,
                cancellationSource.Token);

            PrintResults(result);

            return 0;
        }
        catch (OperationCanceledException)
            when (cancellationSource.IsCancellationRequested)
        {
            Console.Error.WriteLine(
                "Enrichment was cancelled.");

            return 130;
        }
        catch (OperationCanceledException exception)
        {
            Console.Error.WriteLine(
                $"The request timed out: {exception.Message}");

            Console.Error.WriteLine(
                "The enrichment operation could not complete.");

            return 1;
        }
        catch (HttpRequestException exception)
        {
            Console.Error.WriteLine(
                $"The people API request failed: {exception.Message}");

            return 1;
        }
        catch (InvalidDataException exception)
        {
            Console.Error.WriteLine(
                $"The people API returned invalid data: {exception.Message}");

            return 1;
        }
        catch (IOException exception)
        {
            Console.Error.WriteLine(
                $"A file or network operation failed: {exception.Message}");

            return 1;
        }
        catch (UnauthorizedAccessException exception)
        {
            Console.Error.WriteLine(
                $"Access denied: {exception.Message}");

            return 1;
        }
        catch (TimeoutException exception)
        {
            Console.Error.WriteLine(
                $"The operation timed out: {exception.Message}");

            return 1;
        }
        finally
        {
            Console.CancelKeyPress -= cancelHandler;
        }
    }

    private static IPeopleCsvFormatter? CreateFormatter()
    {
        Type? formatterType =
            typeof(IPeopleCsvFormatter).Assembly.GetType(
                "PeopleEnricher.PeopleCsvFormatter");

        if (formatterType is null)
        {
            return null;
        }

        if (!typeof(IPeopleCsvFormatter).IsAssignableFrom(formatterType))
        {
            throw new InvalidOperationException(
                "PeopleCsvFormatter does not implement IPeopleCsvFormatter.");
        }

        return Activator.CreateInstance(formatterType)
            as IPeopleCsvFormatter
            ?? throw new InvalidOperationException(
                "Unable to create PeopleCsvFormatter.");
    }

    private static void PrintResults(EnrichmentRunResult result)
    {
        Console.WriteLine();
        Console.WriteLine("===== ENRICHMENT COMPLETE =====");

        Console.WriteLine(
            $"People received:       {result.PeopleReceived}");

        Console.WriteLine(
            $"Successfully enriched: {result.PeopleWritten}");

        Console.WriteLine(
            $"People rejected:       {result.RejectionReport.TotalRejected}");

        Console.WriteLine(
            $"Output file:            {result.OutputPath}");

        if (result.RejectionReport.TotalRejected == 0)
        {
            Console.WriteLine("\nNo records were rejected.");
            return;
        }

        Console.WriteLine("\n===== REJECTION SUMMARY =====");

        foreach (RejectionReasonSummary summary
                 in result.RejectionReport.ReasonSummaries)
        {
            Console.WriteLine(
                $"{summary.Stage}: {summary.Reason} ({summary.Count})");
        }

        Console.WriteLine("\n===== REJECTED RECORDS =====");

        foreach (RejectedPerson rejection
                 in result.RejectionReport.Details)
        {
            string location = rejection.LineNumber is int lineNumber
                ? $"Line {lineNumber}"
                : rejection.Subject;

            Console.WriteLine(
                $"[{rejection.Stage}] {location}: {rejection.Reason}");
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("People Enricher");
        Console.WriteLine();
        Console.WriteLine(
            "Usage: PeopleEnricher.Console <user> <output.csv> [maxConcurrency]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  user             Your name or ID for the API.");
        Console.WriteLine("  output.csv       Destination for the enriched CSV.");
        Console.WriteLine(
            "  maxConcurrency  Maximum simultaneous ZIP lookups (default: 8).");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine(
            "  dotnet run --project .\\PeopleEnricher.Console -- mparrish1 .\\people.csv 8");
    }
}