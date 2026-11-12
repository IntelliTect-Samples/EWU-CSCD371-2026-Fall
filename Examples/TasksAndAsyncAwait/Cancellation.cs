#:include ConsoleExtensions.cs

Console.DemoTrace("=== cancellation: Requesting cancellation, observing it, and cleaning up. ===");
await Cancellation.RunAsync();
Console.DemoTrace("Example finished.");

internal static class Cancellation
{
    public static async Task RunAsync()
    {
        using CancellationTokenSource cancellation = new();
        Task work = WaitAsync(cancellation.Token);
        Console.DemoTrace($"Caller: work is waiting. IsCompleted = {work.IsCompleted}.");
        Console.DemoTrace("Caller: requesting cancellation, not terminating a thread.");
        cancellation.Cancel();

        try
        {
            await work;
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            Console.DemoTrace($"Caller: observed cancellation. Status = {work.Status}.");
        }
    }

    private static async Task WaitAsync(CancellationToken cancellationToken)
    {
        Console.DemoTrace("WaitAsync: awaiting a delay that completes only through cancellation.");
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        finally
        {
            Console.DemoTrace("WaitAsync: finally runs during cancellation; clean up here.");
        }
    }
}
