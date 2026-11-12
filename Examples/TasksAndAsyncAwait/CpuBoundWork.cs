#:include ConsoleExtensions.cs

Console.DemoTrace("=== task-run: Explicitly scheduling CPU-bound work on the thread pool. ===");
await CpuBoundWork.RunAsync();
Console.DemoTrace("Example finished.");

internal static class CpuBoundWork
{
    public static async Task RunAsync()
    {
        Console.DemoTrace("Caller: explicitly scheduling a synchronous calculation with Task.Run.");
        Task<long> calculation = Task.Run(() =>
        {
            Console.DemoTrace($"Worker: calculating. IsThreadPoolThread = {Thread.CurrentThread.IsThreadPoolThread}.");
            long sum = 0;
            for (int number = 1; number <= 5_000_000; number++)
            {
                sum += number;
            }
            Console.DemoTrace("Worker: calculation finished.");
            return sum;
        });

        Console.DemoTrace("Caller: Task.Run returned a task; now awaiting the result.");
        long result = await calculation;
        Console.DemoTrace($"Caller: sum = {result:N0}.");
    }
}
