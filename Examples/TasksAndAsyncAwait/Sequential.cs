#:include ConsoleExtensions.cs

Console.DemoTrace("=== sequential: Awaiting each operation before starting the next. ===");
await Sequential.RunAsync();
Console.DemoTrace("Example finished.");

internal static class Sequential
{
    public static async Task RunAsync()
    {
        string first = await ReadAsync("First", 500);
        string second = await ReadAsync("Second", 300);
        Console.DemoTrace($"Results: {first}, {second}. Waits did not overlap (about 800 ms total).");
    }

    private static async Task<string> ReadAsync(string name, int milliseconds)
    {
        Console.DemoTrace($"{name}: starting simulated I/O ({milliseconds} ms).");
        await Task.Delay(milliseconds);
        Console.DemoTrace($"{name}: simulated I/O finished.");
        return name.ToUpperInvariant();
    }
}
