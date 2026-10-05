namespace Logger;

public abstract class BaseLogger
{

    public required string className {get; set;}

    public abstract void Log(LogLevel logLevel, string message);
}

