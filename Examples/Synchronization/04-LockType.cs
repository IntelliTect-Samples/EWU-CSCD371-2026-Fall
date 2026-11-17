Lock syncLock = new();
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
        lock (syncLock)
        {
            counter++;
        }
    }
}

#region Teaching notes
/*
Run with: dotnet run .\04-LockType.cs

System.Threading.Lock was added in .NET 9 with special compiler support in
C# 13. Compare this with LockKeyword: the declaration is the meaningful change.

When the expression is statically typed as Lock, the compiler uses EnterScope
and disposes its scope on exit, rather than calling Monitor.Enter and Exit.
An explicit equivalent is: using (syncLock.EnterScope()) { counter++; }

Keep the expression typed as Lock. Casting it to object selects the object's
monitor instead, a different synchronization mechanism; do not mix the two.
Lock is reentrant and thread-affine. Like Monitor, it blocks a contending thread
and cannot be held across await. Prefer a dedicated private Lock for new
synchronous locking code on supported runtimes.
*/
#endregion
