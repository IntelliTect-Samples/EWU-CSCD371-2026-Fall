// Windows demos use dotnet run rather than a Unix shebang.
#:property NoWarn=$(NoWarn);CA2266

using System.Diagnostics;

// Referenced by each example with #:include (requires SDK 10.0.300 or newer).
internal static class ConsoleExtensions
{
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private static readonly object _sync = new();
    private static int _step;

    extension(Console)
    {
        public static void DemoTrace(string message)
        {
            lock (_sync)
            {
                Console.WriteLine(
                    $"{++_step:00} | {_stopwatch.ElapsedMilliseconds,5} ms | thread {Environment.CurrentManagedThreadId,2} | {message}");
            }
        }
    }
}
