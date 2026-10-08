namespace Logger;

public abstract class BaseLogger
{
    public string ClassName { get; set; } = string.Empty;
    public abstract void Log(LogLevel logLevel, string message);
}

