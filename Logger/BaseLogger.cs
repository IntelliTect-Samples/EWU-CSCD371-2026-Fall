namespace Logger;

public abstract class BaseLogger
{
    public required string LoggerClassName { get; set; }
    public abstract void Log(LogLevel logLevel, string message);
}

