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
        bool lockTaken = false;
        try
        {
            Monitor.Enter(syncLock, ref lockTaken);
            counter++;
        }
        finally
        {
            if (lockTaken)
            {
                Monitor.Exit(syncLock);
            }
        }
    }
}

#region Teaching notes
/*
Run with: dotnet run .\03-Monitor.cs

This is the Monitor pattern generated for lock when its expression is typed
as object. The lock expression is evaluated once; here syncLock is already stable.
Monitor.Enter acquires the object's monitor and records successful acquisition.
The finally block releases it only if acquisition succeeded.

Prefer lock for ordinary mutual exclusion: it expresses this pattern directly
and prevents forgetting the finally block. Monitor also has TryEnter for bounded
acquisition attempts, plus Wait and Pulse for condition coordination.

A monitor is owned by a thread and is reentrant on that thread. The same thread
must release it. It is not a lock to hold across an asynchronous suspension.
*/
#endregion
