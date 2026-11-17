Lock leftSyncLock = new();
Lock rightSyncLock = new();

Task first = Task.Run(() => TakeLocks("A", leftSyncLock, rightSyncLock));
Task second = Task.Run(() => TakeLocks("B", rightSyncLock, leftSyncLock));

await Task.WhenAll(first, second);

void TakeLocks(string name, Lock firstSyncLock, Lock secondSyncLock)
{
    lock (firstSyncLock)
    {
        Console.WriteLine($"{name}: first");

        lock (secondSyncLock)
        {
            Console.WriteLine($"{name}: second");
        }
    }
}

#region Teaching notes
/*
Run with: dotnet run .\11-Deadlock.cs
This example can deadlock depending on scheduling. Rerun if it completes.
Stop it with Ctrl+C when it hangs.

If A holds leftSyncLock while B holds rightSyncLock, each waits for the other's
lock. Neither can leave its first lock until it acquires the second. If one
worker acquires both locks before the other acquires its first, both can finish.
These synchronous waits block their thread-pool threads; awaiting the tasks
does not release the locks held inside them.

Nested locks are not automatically wrong, but inconsistent acquisition order
can deadlock. The ordinary repair is one global order, or one lock protecting
the entire invariant. A timeout can help detect a problem but does not establish
correctness. Reentering the same lock on one thread is not this two-thread cycle.
*/
#endregion
