namespace ExceptionHandling.Shared;

/// <summary>
/// Simulates another program (say, Excel) holding a file open exclusively.
/// </summary>
public static class AnotherProgram
{
    public static IDisposable LockFile(string path) =>
        new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
}
