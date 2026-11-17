using System.Diagnostics;

await RunAsync(holdLockDuringWork: true);
await RunAsync(holdLockDuringWork: false);

async Task RunAsync(bool holdLockDuringWork)
{
    Lock syncLock = new();
    int completed = 0;
    Stopwatch stopwatch = Stopwatch.StartNew();

    Task first = Task.Run(Work);
    Task second = Task.Run(Work);
    Task third = Task.Run(Work);
    Task fourth = Task.Run(Work);

    await Task.WhenAll(first, second, third, fourth);

    stopwatch.Stop();
    string scope = holdLockDuringWork ? "wide" : "narrow";
    Console.WriteLine($"{scope}: {completed}, {stopwatch.ElapsedMilliseconds} ms");

    void Work()
    {
        for (int i = 0; i < 6; i++)
        {
            if (holdLockDuringWork)
            {
                lock (syncLock)
                {
                    DoWork();
                    completed++;
                }
            }
            else
            {
                DoWork();
                lock (syncLock)
                {
                    completed++;
                }
            }
        }
    }
}

void DoWork()
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    while (stopwatch.ElapsedMilliseconds < 20)
    {
    }
}

#region Teaching notes
/*
Run with: dotnet run .\10-LockGranularity.cs

Both versions protect the same counter. DoWork uses a busy loop to stand in for
independent CPU work, not a recommended implementation of a real workload.
The wide critical section serializes that work; the narrow one allows it to
overlap. Both must still complete 24 updates.

Uncontended locks have a cost, but contention and unnecessarily serialized work
often matter much more. Keep I/O, delays, logging, and unknown callbacks outside
syncLock when correctness allows. Excessively frequent tiny acquisitions also
cost time; accumulating a local result and merging once can reduce contention.

Do not move work outside a lock if doing so breaks the protected invariant.
This is a qualitative demonstration, not a benchmark. Scheduling, thread-pool
availability, JIT compilation, and processor count affect elapsed time.
*/
#endregion
