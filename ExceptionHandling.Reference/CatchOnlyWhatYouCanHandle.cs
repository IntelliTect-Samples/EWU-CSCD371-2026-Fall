using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

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
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using FileStream stream = new(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                using StreamReader reader = new(stream);
                return reader.ReadToEnd();
            }
            // "File in use" has a known response: wait and retry.
            // FileNotFoundException IS an IOException, but waiting won't make the file appear,
            // so we don't catch it - the caller decides what a missing report means.
            catch (IOException exception)
                when (exception is not FileNotFoundException and not DirectoryNotFoundException)
            {
                Output.Note($"  Attempt {attempt} of {maxAttempts}: file is in use, waiting...");
                Thread.Sleep(250);
            }
        }

        return null;
    }
}
