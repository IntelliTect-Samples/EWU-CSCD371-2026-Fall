#:include ConsoleExtensions.cs

Console.DemoTrace("=== async-void: Runs asynchronously, but returns no awaitable task. ===");
TaskCompletionSource finished = new();

Console.DemoTrace("Caller: calling WorkAsync.");
WorkAsync();
Console.DemoTrace("Caller: WorkAsync returned void, not a Task; the caller can continue.");

// Uncomment to demonstrate CS4008: Cannot await 'void'.
// await WorkAsync();

Console.DemoTrace("Caller: awaiting a separate completion signal to keep this console app alive, not awaiting WorkAsync.");
await finished.Task;
Console.DemoTrace("Example finished.");

async void WorkAsync()
{
    Console.DemoTrace("WorkAsync: running synchronous code before await.");
    await Task.Delay(300);
    Console.DemoTrace("WorkAsync: resumed after await and finished.");
    finished.SetResult();
}
