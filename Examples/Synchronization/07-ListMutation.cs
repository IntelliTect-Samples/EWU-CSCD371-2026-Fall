const int count = 100_000;
List<int> numbers = new(count);

Task first = Task.Run(() => AddNumbers(0, count / 2));
Task second = Task.Run(() => AddNumbers(count / 2, count));

await Task.WhenAll(first, second);

Console.WriteLine($"{numbers.Count:N0} / {count:N0}");
Console.WriteLine($"{numbers.Distinct().Count():N0} / {count:N0}");

void AddNumbers(int start, int end)
{
    for (int number = start; number < end; number++)
    {
        numbers.Add(number);
    }
}

#region Teaching notes
/*
Run with: dotnet run .\07-ListMutation.cs

List<T> does not support concurrent mutation. Add changes shared state, including
the size and an array slot, even when each worker supplies a different number.
Updates can be lost or overwritten. Concurrent growth can introduce additional
races and exceptions.

Capacity is allocated up front here to keep resizing out of the demonstration;
capacity does not make Add thread-safe. The output shows total entries and
distinct entries, each compared with the intended count. Results vary by run.

Each Task.Run worker adds a different half of the numbers. Awaiting Task.WhenAll
waits for both tasks, so these final reads are not concurrent with mutation.
Waiting afterward cannot repair earlier races.
*/
#endregion
