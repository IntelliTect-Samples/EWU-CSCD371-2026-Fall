using ManualResetEvent mre = new(false);

Task first = Task.Run(() => Wait("A"));
Task second = Task.Run(() => Wait("B"));

await Task.Delay(100);
mre.Set();

await Task.WhenAll(first, second);

Console.WriteLine(mre.WaitOne(0));
mre.Reset();
Console.WriteLine(mre.WaitOne(0));

void Wait(string name)
{
    Console.WriteLine($"{name}: waiting");
    mre.WaitOne();
    Console.WriteLine($"{name}: running");
}

#region Teaching notes
/*
Run with: dotnet run .\13-ManualResetEvent.cs

The event starts nonsignaled. WaitOne blocks the calling thread until Set opens
the gate. Set releases all waiting threads and keeps the gate open for later
waiters. The delay makes waiting visible but is not a synchronization guarantee.
Even a worker that starts after Set will pass through the still-open gate.

After both workers finish, a zero-timeout wait succeeds without consuming the
signal. Reset closes the gate, so the next zero-timeout wait returns false.
An event signals readiness; it does not provide mutual exclusion or count items.
AutoResetEvent instead releases one waiter per signal, without accumulating a
count of repeated signals.

Use ManualResetEvent for thread-blocking coordination, not asynchronous waiting.
Task.Run does not make WaitOne asynchronous; each waiter occupies a pool thread.
This is useful for demonstrating the event, not for scaling asynchronous waits.
ManualResetEventSlim is a lighter alternative for many in-process scenarios.
Dispose the event only once no workers can still be using it.
*/
#endregion
