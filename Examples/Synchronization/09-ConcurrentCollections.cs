using System.Collections.Concurrent;

const int count = 60_000;
ConcurrentQueue<int> numbers = new();
ConcurrentDictionary<int, int> totals = new();

Task first = Task.Run(() => AddNumbers(0, count / 2));
Task second = Task.Run(() => AddNumbers(count / 2, count));

await Task.WhenAll(first, second);

Console.WriteLine($"{numbers.Count:N0} / {count:N0}");

while (numbers.TryDequeue(out int number))
{
    if (number < 5)
    {
        Console.WriteLine(number);
    }
}

foreach (KeyValuePair<int, int> total in totals.OrderBy(pair => pair.Key))
{
    Console.WriteLine($"{total.Key}: {total.Value:N0}");
}

void AddNumbers(int start, int end)
{
    for (int number = start; number < end; number++)
    {
        numbers.Enqueue(number);
        totals.AddOrUpdate(number % 3, 1, (key, current) => current + 1);
    }
}

#region Teaching notes
/*
Run with: dotnet run .\09-ConcurrentCollections.cs

ConcurrentQueue<T> supports concurrent producers and consumers. Enqueue order
depends on scheduling, not numeric order. TryDequeue combines testing for an
item and removing it; an IsEmpty check followed by a separate removal would race.
The loop drains the queue, printing only values below five to keep output small.

ConcurrentDictionary<TKey,TValue>.AddOrUpdate performs a thread-safe update.
Its update factory may execute more than once for an operation under contention,
so keep that function free of side effects. These buckets each end at 20,000.

Thread-safe operations do not make a sequence of operations transactional.
Enqueuing and updating the dictionary are not one atomic operation. Likewise,
ContainsKey followed by assignment is not an atomic add-if-absent; use TryAdd.
*/
#endregion
