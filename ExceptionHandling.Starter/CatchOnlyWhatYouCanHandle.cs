using ExceptionHandling.Shared;

namespace ExceptionHandling.Starter;

public static class CatchOnlyWhatYouCanHandle
{
    public static void Run()
    {
        string reportPath = Path.Combine(Path.GetTempPath(), "quarterly-report.txt");
        File.WriteAllText(reportPath, "Q3 revenue: $1,000,000");

        using (AnotherProgram.LockFile(reportPath))
        {
            string? report = ReadReport(reportPath, maxAttempts: 3);
            Console.WriteLine(report ?? "The report is open in another program. Close it and try again.");
        }

        Console.WriteLine();
        Console.WriteLine("Now the lock is released:");
        Console.WriteLine(ReadReport(reportPath, maxAttempts: 3));

        Console.WriteLine();
        Console.WriteLine("Now a report that doesn't exist:");
        Console.WriteLine(ReadReport(Path.Combine(Path.GetTempPath(), "no-such-report.txt"), maxAttempts: 3));
    }

    private static string? ReadReport(string path, int maxAttempts)
    {
        // TODO 1: Retry up to maxAttempts times, catching ONLY the exception we know how to handle ("file in use").
        //         Output.Note($"  Attempt {attempt} of {maxAttempts}: file is in use, waiting...");
        //         Thread.Sleep(250);
        // TODO 2: Run again. What happens to the report that doesn't exist? Fix it.
        using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
