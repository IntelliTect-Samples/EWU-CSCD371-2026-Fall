namespace ExceptionHandling.Starter;

public static class DontBuryExceptions
{
    public static void Run()
    {
        string settingsPath = Path.Combine(Path.GetTempPath(), "no-such-folder", "settings.txt");

        Console.WriteLine("BEFORE: catch { } buries the failure");
        SaveSettingsBuried(settingsPath, "Theme=Dark");
        Console.WriteLine("  Settings saved!   <-- the user believes this...");
        Console.WriteLine($"  File exists? {File.Exists(settingsPath)}");

        // TODO 1: Write DeleteFolder(path) using Directory.Delete(path, recursive: true),
        //         treating an already-missing folder as success. Call it on a folder that doesn't exist.

        // TODO 2: Write SaveSettings(path, settings) that does NOT catch, and call it here.
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
}
