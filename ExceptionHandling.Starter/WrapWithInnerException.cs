using ExceptionHandling.Shared;

namespace ExceptionHandling.Starter;

public static class WrapWithInnerException
{
    public static void Run()
    {
        string[] settingsFile = ["retries=3", "timeout=thirty"];

        try
        {
            LoadSettings(settingsFile);
        }
        catch (InvalidSettingsException exception)
        {
            Output.ExceptionChain(exception);
        }
    }

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
            catch (FormatException)
            {
                // TODO: The caller can't tell WHY. Improve the message and preserve the original exception.
                throw new InvalidSettingsException($"Setting '{parts[0]}' is invalid.");
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
