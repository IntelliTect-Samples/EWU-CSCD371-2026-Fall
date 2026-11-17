List<int> numbers = [1, 2, 3, 4, 5];

Task writer = Task.Run(async () =>
{
    await Task.Delay(30);
    numbers.Add(6);
});

try
{
    foreach (int number in numbers)
    {
        Console.WriteLine(number);
        await Task.Delay(20);
    }
}
catch (InvalidOperationException exception)
{
    Console.WriteLine(exception.GetType().Name);
}

await writer;

#region Teaching notes
/*
Run with: dotnet run .\06-ListEnumeration.cs

A List<T> enumerator checks the collection's version and normally throws when
the list is changed during enumeration. The delays make that interleaving likely,
not guaranteed. This catch exposes the expected demonstration failure; catching
and ignoring it is not a synchronization strategy.

Locking only Add is not enough if another thread enumerates without that lock.
Protect the whole enumeration with the same gate, or copy a snapshot while
holding the gate and enumerate the snapshot afterward. The copy itself must
be protected from concurrent writers; calling ToList alone is not a fix.

Prefer the snapshot approach when processing items would keep the gate occupied
for a long time. Concurrent collection enumeration has collection-specific
semantics; do not assume every collection provides a moment-in-time snapshot.
*/
#endregion
