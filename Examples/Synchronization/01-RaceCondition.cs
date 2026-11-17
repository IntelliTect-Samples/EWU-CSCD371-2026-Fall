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
        counter++;
    }
}

#region Teaching notes
/*
Run with: dotnet run .\01-RaceCondition.cs

Task.Run schedules each worker on the thread pool. The workers share counter,
but each increment reads, changes, and writes it. Two workers can read the same
value and overwrite each other's updates. Awaiting Task.WhenAll waits for both
tasks to complete; it does not make increments atomic or guarantee parallelism.

The expected total is two million. The actual result depends on scheduling and
may occasionally be correct. Rerun it or change the iteration count. A correct
result on one run does not establish thread safety. Neither volatile nor a
delay would make this read-modify-write operation atomic.
*/
#endregion
