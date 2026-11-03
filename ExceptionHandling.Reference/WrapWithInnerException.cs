using ExceptionHandling.Shared;

namespace ExceptionHandling.Reference;

public static class WrapWithInnerException
{
    public static void Run()
    {
        string[] settingsFile = ["retries=3", "timeout=thirty"];

        Console.WriteLine("BEFORE: new exception, original dropped");
        try
        {
            LoadSettingsLosingCause(settingsFile);
        }
        catch (InvalidSettingsException exception)
        {
            Output.ExceptionChain(exception);
        }

        Console.WriteLine();
        Console.WriteLine("AFTER: original preserved as InnerException");
        try
        {
            LoadSettings(settingsFile);
        }
        catch (InvalidSettingsException exception)
        {
            Output.ExceptionChain(exception);
        }
    }

    // ----- BEFORE -----
    private static Dictionary<string, int> LoadSettingsLosingCause(string[] lines)
    {
        Dictionary<string, int> settings = [];
        foreach (string line in lines)
        {
            string[] parts = line.Split('=');
            try
            {
                settings[parts[0]] = int.Parse(parts[1]);
            }
            catch (FormatException)
            {
                throw new InvalidSettingsException($"Setting '{parts[0]}' is invalid.");
            }
        }
        return settings;
    }

    // ----- AFTER -----
    private static Dictionary<string, int> LoadSettings(string[] lines)
    {
        Dictionary<string, int> settings = [];
        foreach (string line in lines)
        {
            string[] parts = line.Split('=');
            try
            {
                settings[parts[0]] = int.Parse(parts[1]);
            }
            catch (FormatException exception)
            {
                // Situation: changing the type clarifies the problem. The caller learns WHICH
                // setting is wrong (our domain), while the low-level cause rides along inside.
                throw new InvalidSettingsException(
                    $"Setting '{parts[0]}' must be a whole number but was '{parts[1]}'.", exception);
            }
        }
        return settings;
    }
}

public class InvalidSettingsException : Exception
{
    public InvalidSettingsException() { }

    public InvalidSettingsException(string message) : base(message) { }

    public InvalidSettingsException(string message, Exception innerException)
        : base(message, innerException) { }
}
