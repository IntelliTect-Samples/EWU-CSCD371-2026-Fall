namespace ExceptionHandling.Reference;

public static class DontBuryExceptions
{
    public static void Run()
    {
        string settingsPath = Path.Combine(Path.GetTempPath(), "no-such-folder", "settings.txt");

        Console.WriteLine("BEFORE: catch { } buries the failure");
        SaveSettingsBuried(settingsPath, "Theme=Dark");
        Console.WriteLine("  Settings saved!   <-- the user believes this...");
        Console.WriteLine($"  File exists? {File.Exists(settingsPath)}");

        Console.WriteLine();
        Console.WriteLine("OK: catching an exception we have decided is innocuous");
        DeleteFolder(Path.Combine(Path.GetTempPath(), "already-deleted-folder"));
        Console.WriteLine("  Cleanup complete.");

        Console.WriteLine();
        Console.WriteLine("AFTER: don't catch what we can't handle - the caller finds out");
        SaveSettings(settingsPath, "Theme=Dark");
        Console.WriteLine("  Settings saved!");
    }

    private static void SaveSettingsBuried(string path, string settings)
    {
        try
        {
            File.WriteAllText(path, settings);
        }
        catch
        {
            // "It works on my machine!"
        }
    }

    private static void SaveSettings(string path, string settings) =>
        File.WriteAllText(path, settings);

    private static void DeleteFolder(string path)
    {
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (DirectoryNotFoundException)
        {
            // Explicitly innocuous: the folder being gone is exactly the outcome we wanted.
        }
    }
}
