string firstText = "inventory";
string secondText = "inventory";
Type firstType = typeof(List<int>);
Type secondType = typeof(List<int>);
int syncLock = 42;
object firstBox = syncLock;
object secondBox = syncLock;

Console.WriteLine($"string: {ReferenceEquals(firstText, secondText)}");
Console.WriteLine($"Type: {ReferenceEquals(firstType, secondType)}");
Console.WriteLine($"boxed int: {ReferenceEquals(firstBox, secondBox)}");

int counter = 0;
const int iterations = 100_000;

Task first = Task.Run(Increment);
Task second = Task.Run(Increment);

await Task.WhenAll(first, second);

Console.WriteLine($"{counter:N0} / {2 * iterations:N0}");

void Increment()
{
    for (int i = 0; i < iterations; i++)
    {
        lock ((object)syncLock)
        {
            counter++;
        }
    }
}

#region Teaching notes
/*
Run with: dotnet run .\05-UnsafeLockTargets.cs

Equal string literals can refer to the same interned object. A Type object is
also accessible to unrelated code. Using either as a lock lets unrelated code
contend with you or participate in a deadlock. A public instance, including
this when callers can reach it, has the same exposure problem.

A value type cannot be locked directly. Casting this int to object compiles,
but boxes it anew on each acquisition, so the workers do not share a gate.
The identity checks distinguish the shared objects from the separate boxes.
The counter can lose updates even though there is a lock in the loop.

A box stored once in an object variable would have stable identity, but a
dedicated private object or Lock is clearer and avoids the boxing trap.
Change the declaration to object syncLock = new() and lock syncLock without the
cast to repair the example.
*/
#endregion
