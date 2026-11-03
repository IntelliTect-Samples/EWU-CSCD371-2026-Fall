namespace ExceptionHandling.Shared;

/// <summary>
/// A stand-in for a real logger (ILogger, Serilog, ...) that counts entries
/// so the class can see how many times a single failure was logged.
/// </summary>
public static class Log
{
    public static int Count { get; private set; }

    public static void Reset() => Count = 0;

    public static void Error(string source, Exception exception)
    {
        Count++;
        Output.WriteLine(ConsoleColor.Yellow,
            $"  [LOG #{Count}] {source}: {exception.GetType().Name}: {exception.Message}");
    }
}
