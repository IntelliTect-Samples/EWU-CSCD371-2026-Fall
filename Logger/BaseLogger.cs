namespace Logger;

public abstract class BaseLogger
{
    // req1: The BaseLogger class needs an auto property to store class name
    public string ClassName { get; set; } = string.Empty;
    public abstract void Log(LogLevel logLevel, string message);
}

