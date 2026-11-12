#:include ConsoleExtensions.cs

Console.DemoTrace("=== completed-await: An already-completed await does not suspend. ===");
await CompletedAwait.RunAsync();
Console.DemoTrace("Example finished.");

internal static class CompletedAwait
{
    public static async Task RunAsync()
    {
        Console.DemoTrace("Caller: calling ReadCachedValueAsync.");
        Task<int> cachedValue = ReadCachedValueAsync();
        Console.DemoTrace($"Caller: method returned. IsCompleted = {cachedValue.IsCompleted}.");
        Console.DemoTrace($"Caller: result = {await cachedValue}.");
    }

    private static async Task<int> ReadCachedValueAsync()
    {
        Console.DemoTrace("ReadCachedValueAsync: before awaiting an already-completed task.");
        int value = await Task.FromResult(42);
        Console.DemoTrace("ReadCachedValueAsync: after await, still before returning to the caller.");
        return value;
    }
}
