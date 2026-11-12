#:include ConsoleExtensions.cs

Console.DemoTrace("=== when-all: Starting both operations before awaiting Task.WhenAll. ===");
await WhenAll.RunAsync();
Console.DemoTrace("Example finished.");

internal static class WhenAll
{
    public static async Task RunAsync()
    {
        Task<string> first = ReadAsync("First", 500);
        Task<string> second = ReadAsync("Second", 300);
        Console.DemoTrace("Caller: both operations have started; now awaiting Task.WhenAll.");

        string[] results = await Task.WhenAll(first, second);
        Console.DemoTrace($"Results in input order: {string.Join(", ", results)}. Waits overlapped (about 500 ms total).");
    }

    private static async Task<string> ReadAsync(string name, int milliseconds)
    {
        Console.DemoTrace($"{name}: starting simulated I/O ({milliseconds} ms).");
        await Task.Delay(milliseconds);
        Console.DemoTrace($"{name}: simulated I/O finished.");
        return name.ToUpperInvariant();
    }
}
