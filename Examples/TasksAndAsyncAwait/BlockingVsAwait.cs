#:include ConsoleExtensions.cs

Console.DemoTrace("=== blocking: Thread.Sleep blocks a thread; Task.Delay can suspend a method. ===");
await BlockingVsAwait.RunAsync();
Console.DemoTrace("Example finished.");

internal static class BlockingVsAwait
{
    public static async Task RunAsync()
    {
        Console.DemoTrace("Caller: calling an async method that uses Thread.Sleep.");
        Task blocking = SleepAsync();
        Console.DemoTrace($"Caller: SleepAsync finally returned. IsCompleted = {blocking.IsCompleted}.");
        await blocking;

        Console.DemoTrace("Caller: calling a method that awaits Task.Delay.");
        Task nonblocking = DelayAsync();
        Console.DemoTrace("Caller: can do other work after DelayAsync returns, before awaiting it.");
        await nonblocking;
        Console.DemoTrace("Caller: the nonblocking wait is complete.");
    }

    private static async Task SleepAsync()
    {
        Console.DemoTrace("SleepAsync: blocking the current thread for 300 ms.");
        Thread.Sleep(300);
        await Task.CompletedTask;
        Console.DemoTrace("SleepAsync: finished; async did not move the sleep to another thread.");
    }

    private static async Task DelayAsync()
    {
        Console.DemoTrace("DelayAsync: starting a 300 ms timer and awaiting it.");
        await Task.Delay(300);
        Console.DemoTrace("DelayAsync: timer completed; continuing the method.");
    }
}
