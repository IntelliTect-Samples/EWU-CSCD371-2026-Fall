using PeopleEnricher.Models;
using PeopleEnricher.Services;

Console.WriteLine("Assignment 9 started!");

string user = args.Length > 0 ? args[0] : "Mavrick";
string outputPath = args.Length > 1
    ? args[1]
    : "CompletedPeople.csv";

using HttpClient httpClient = new()
{
    Timeout = TimeSpan.FromSeconds(10)
};

using CancellationTokenSource cancellation = new();

ConsoleCancelEventHandler cancelHandler = (_, e) =>
{
    e.Cancel = true;
    cancellation.Cancel();
};

Console.CancelKeyPress += cancelHandler;

try
{
    PeopleEnricherService service = new(httpClient);

    ProcessingResult result = await service.RunAsync(
        user, cancellation.Token);

    await PeopleEnricherService.SaveCsvAsync(
        result, outputPath, cancellation.Token);

    Console.WriteLine(
        $"Completed people: {result.CompletedPeople.Count}");

    Console.WriteLine(
        $"Rejected people: {result.RejectedPeople.Count}");

    Console.WriteLine(
        $"CSV saved to: {Path.GetFullPath(outputPath)}");

    foreach (var group in result.RejectedPeople
        .GroupBy(person => person.Reason)
        .OrderBy(group => group.Key))
    {
        Console.WriteLine(
            $"{group.Key}: {group.Count()}");
    }
}
catch (OperationCanceledException) when (
    cancellation.IsCancellationRequested)
{
    Console.WriteLine("The operation was canceled.");
}
finally
{
    Console.CancelKeyPress -= cancelHandler;
}