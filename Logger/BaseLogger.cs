namespace Logger;

public abstract class BaseLogger
{
    public string ClassName { get; set; } = string.Empty; // allows empty string if className is null

    public abstract void Log(LogLevel logLevel, string message);
}

