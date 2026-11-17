object syncLock = new();
int counter = 0;
const int iterations = 1_000_000;

Task first = Task.Run(Increment);
Task second = Task.Run(Increment);

await Task.WhenAll(first, second);

Console.WriteLine($"{counter:N0} / {2 * iterations:N0}");

void Increment()
{
    for (int i = 0; i < iterations; i++)
    {
        lock (syncLock)
        {
            counter++;
        }
    }
}

#region Teaching notes
/*
Run with: dotnet run .\02-LockKeyword.cs

Compare this with RaceCondition. Both workers now acquire syncLock before
touching counter. Only one can execute this critical section at a time.
The lock object's identity matters: creating a different one per task would not help.

Keep syncLock private and stable. Every overlapping access that participates in
this invariant must use the same synchronization strategy. Reading counter after
awaiting both tasks is safe because the writers have finished.

Lock acquisition can block the calling thread. Leaving the block releases the
lock, even when an exception is thrown. An await is not allowed inside lock.
For a single counter, Interlocked.Increment is a smaller alternative; locks can
protect invariants spanning several operations or values.
*/
#endregion
