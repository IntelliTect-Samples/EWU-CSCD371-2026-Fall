using SemaphoreSlim syncLock = new(1, 1);
int balance = 0;

await Task.WhenAll(DepositAsync(100), DepositAsync(200));

Console.WriteLine(balance);

async Task DepositAsync(int amount)
{
    await syncLock.WaitAsync();
    try
    {
        int current = balance;
        await Task.Delay(100);
        balance = current + amount;
    }
    finally
    {
        syncLock.Release();
    }
}

#region Teaching notes
/*
Run with: dotnet run .\12-SemaphoreSlim.cs

A SemaphoreSlim with initial and maximum counts of one acts as an asynchronous
gate. WaitAsync waits without blocking a thread. Unlike Monitor and Lock, it has
no owning thread, so an awaited operation can resume elsewhere and release it.

The read, delay, and write form one protected operation. Removing syncLock loses
an update: both calls read the old balance before either delay completes.
Delay stands in for asynchronous work that must be coordinated with this state.

Acquire before entering try, and release exactly once in finally after successful
acquisition. If acquisition is canceled or fails, do not release an unacquired
permit. The maximum count helps detect some extra releases, but does not enforce
ownership. This semaphore is not reentrant; recursively acquiring it can deadlock.
Dispose it only after every user has finished.
*/
#endregion
