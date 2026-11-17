const int count = 100_000;
List<int> numbers = new(count);
Lock syncLock = new();

Task first = Task.Run(() => AddNumbers(0, count / 2));
Task second = Task.Run(() => AddNumbers(count / 2, count));

await Task.WhenAll(first, second);

List<int> snapshot;
lock (syncLock)
{
    snapshot = [.. numbers];
}

Console.WriteLine($"{snapshot.Count:N0} / {count:N0}");
Console.WriteLine($"{snapshot.Distinct().Count():N0} / {count:N0}");

void AddNumbers(int start, int end)
{
    for (int number = start; number < end; number++)
    {
        lock (syncLock)
        {
            numbers.Add(number);
        }
    }
}

#region Teaching notes
/*
Run with: dotnet run .\08-LockedList.cs

Compare with ListMutation. The same syncLock now protects every Add. Count and
distinct count both match the intended total, although insertion order varies.

Copying under syncLock provides a consistent snapshot; work on the copy does
not need to hold the lock. Here both tasks have already finished, so the copy
would be safe without a lock, but this is the pattern to keep if writers remain.
All writers must cooperate with syncLock for the snapshot pattern to work.

A regular collection plus an external lock can protect a multi-step invariant,
such as checking for a value and adding only when absent. Put the whole operation
inside one critical section, not separate locks around the check and the update.
*/
#endregion
