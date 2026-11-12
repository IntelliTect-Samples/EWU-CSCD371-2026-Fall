#:include ConsoleExtensions.cs

Console.DemoTrace("=== exceptions: Observing faulted tasks and all Task.WhenAll failures. ===");
await Exceptions.RunAsync();
Console.DemoTrace("Example finished.");

internal static class Exceptions
{
    public static async Task RunAsync()
    {
        Task immediateFailure = FailImmediatelyAsync();
        Console.DemoTrace($"Caller: received a task, not a synchronous throw. Status = {immediateFailure.Status}.");
        try
        {
            await immediateFailure;
        }
        catch (InvalidOperationException exception)
        {
            Console.DemoTrace($"Caller: await rethrew the failure: {exception.Message}");
        }

        Task first = FailAfterDelayAsync("First");
        Task second = FailAfterDelayAsync("Second");
        Task allFailures = Task.WhenAll(first, second);

        try
        {
            await allFailures;
        }
        catch (InvalidOperationException exception)
        {
            Console.DemoTrace($"Await WhenAll rethrew one failure: {exception.Message}");
            if (allFailures.Exception is AggregateException failures)
            {
                foreach (Exception failure in failures.Flatten().InnerExceptions)
                {
                    Console.DemoTrace($"Collected failure: {failure.Message}");
                }
            }
        }

        Console.DemoTrace($"WhenAll waited for both tasks. Statuses: {first.Status}, {second.Status}.");
    }

    private static async Task FailImmediatelyAsync()
    {
        Console.DemoTrace("FailImmediatelyAsync: awaiting a completed task, then throwing.");
        await Task.CompletedTask;
        throw new InvalidOperationException("Immediate failure.");
    }

    private static async Task FailAfterDelayAsync(string name)
    {
        await Task.Delay(300);
        throw new InvalidOperationException($"{name} failure.");
    }
}
