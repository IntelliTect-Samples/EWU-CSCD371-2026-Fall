TaskCompletionSource<int> completion = new();

Task producer = Task.Run(async () =>
{
    await Task.Delay(100);
    completion.SetResult(42);
});

Task first = ReadAsync("A");
Task second = ReadAsync("B");

await Task.WhenAll(first, second, producer);

async Task ReadAsync(string name)
{
    int result = await completion.Task;
    Console.WriteLine($"{name}: {result}");
}

#region Teaching notes
/*
Run with: dotnet run .\14-TaskCompletionSource.cs

TaskCompletionSource<T> exposes a Task<T> whose completion is controlled by a
producer. Creating it does not start a thread or schedule work. Here a separate
Task.Run operation stands in for an external callback or event-based API.

Both consumers await the same task without blocking a thread while they wait.
They receive the same result; completion is not consumed by the first reader.
This resembles a one-shot asynchronous signal with a value, not a resettable
event, a queue, or a mutual-exclusion gate.

With the default constructor, registered continuations may execute inline
inside SetResult. Awaiting an already completed task can also continue
synchronously. Use SetException or SetCanceled for other completion outcomes;
only one terminal outcome is allowed. Use TrySetResult, TrySetException, or
TrySetCanceled when competing producers may legitimately race to complete it.
*/
#endregion
