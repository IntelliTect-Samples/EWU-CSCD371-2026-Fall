#:include ConsoleExtensions.cs

using System.Collections.Concurrent;

if (args.Length > 1 || (args.Length == 1 && args[0] is not ("result" or "wait" or "await")))
{
    Console.Error.WriteLine("Usage: dotnet run Deadlock.cs -- [result|wait|await]");
    return 2;
}

string mode = args.Length == 0 ? "result" : args[0];
Console.DemoTrace($"=== deadlock: mode = {mode} ===");
Console.DemoTrace("The result and wait modes intentionally hang. Press Ctrl+C to stop them.");

using SingleThreadSynchronizationContext context = new();
Task demonstration = Deadlock.RunAsync(mode, context);
context.RunOnCurrentThread();
await demonstration;
Console.DemoTrace("Example finished.");

return 0;

internal static class Deadlock
{
    public static async Task RunAsync(string mode, SingleThreadSynchronizationContext context)
    {
        try
        {
            Task<int> task = ReadAsync();
            Console.DemoTrace($"Caller: received the task. IsCompleted = {task.IsCompleted}.");

            int result;
            if (mode == "await")
            {
                Console.DemoTrace("Caller: awaiting the task; returning control so the context can process continuations.");
                result = await task;
            }
            else if (mode == "wait")
            {
                Console.DemoTrace("Caller: blocking with task.Wait(); this thread cannot process continuations.");
                task.Wait();
                result = task.Result;
            }
            else
            {
                Console.DemoTrace("Caller: blocking with task.Result; this thread cannot process continuations.");
                result = task.Result;
            }

            Console.DemoTrace($"Caller: result = {result}.");
        }
        finally
        {
            context.Complete();
        }
    }

    private static async Task<int> ReadAsync()
    {
        Console.DemoTrace("ReadAsync: before await; capturing the single-threaded context.");
        await Task.Delay(500);
        Console.DemoTrace("ReadAsync: after await; back on the context's thread.");
        return 42;
    }
}

internal sealed class SingleThreadSynchronizationContext : SynchronizationContext, IDisposable
{
    private readonly BlockingCollection<(SendOrPostCallback Callback, object? State)> _callbacks = new();
    private readonly int _threadId = Environment.CurrentManagedThreadId;

    public SingleThreadSynchronizationContext()
    {
        SynchronizationContext.SetSynchronizationContext(this);
    }

    public override void Post(SendOrPostCallback callback, object? state)
    {
        _callbacks.Add((callback, state));
        Console.DemoTrace($"Context: continuation queued for thread {_threadId}.");
    }

    public void RunOnCurrentThread()
    {
        foreach ((SendOrPostCallback callback, object? state) in _callbacks.GetConsumingEnumerable())
        {
            callback(state);
        }
    }

    public void Complete()
    {
        _callbacks.CompleteAdding();
    }

    public void Dispose()
    {
        _callbacks.Dispose();
    }
}
