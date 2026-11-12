#:include ConsoleExtensions.cs

Console.DemoTrace("=== tasks: A Task represents completion; Task<T> also carries a result. ===");
await TaskBasics.RunAsync();
Console.DemoTrace("Example finished.");

internal static class TaskBasics
{
    public static async Task RunAsync()
    {
        TaskCompletionSource<int> completion = new();
        Task<int> answerTask = completion.Task;

        Console.DemoTrace($"A Task<int> is a promise of a result. IsCompleted = {answerTask.IsCompleted}.");
        Console.DemoTrace("The producer supplies 42; no worker thread was needed to create this task.");
        completion.SetResult(42);

        int answer = await answerTask;
        Console.DemoTrace($"Await unwraps the result: {answer}. Status = {answerTask.Status}.");

        int sameAnswer = await answerTask;
        Console.DemoTrace($"Awaiting the same task again gives {sameAnswer}; it does not rerun an operation.");
    }
}
