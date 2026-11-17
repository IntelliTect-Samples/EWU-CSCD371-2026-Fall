Task task = new(() => Console.WriteLine(42));

Console.WriteLine(task.Status);

task.Start();
await task;

Console.WriteLine(task.Status);

#region Teaching notes
/*
Run with: dotnet run .\00-TaskConstructor.cs

Constructing this Task captures the action without scheduling it. Its initial
status is Created, and the action has not run.

Start schedules the action; it does not wait for completion. In this console
example the default scheduler runs the action on the thread pool. Awaiting the
task waits for completion before reading its final status, RanToCompletion.
Await alone would not start a task created with this constructor.

A Task can be started only once. Task.Run is the usual shorthand when work
should be scheduled immediately, as in the following examples. Use synchronous
actions with this constructor; an async lambda would become async void, so this
Task would not represent completion of the asynchronous operation.
*/
#endregion
