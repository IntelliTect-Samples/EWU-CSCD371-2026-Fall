namespace Logger;

public abstract class BaseLogger
{
    public string ClassName { get; init; } = string.Empty;

    public abstract void Log(LogLevel logLevel, string message);
}

