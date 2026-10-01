namespace Logger;

public abstract class BaseLogger
{
    public abstract void Log(LogLevel logLevel, string message);
    public required string ClassName 
    { 
        get; init;
    }
    
}

