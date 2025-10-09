namespace Logger;

public abstract class BaseLogger
{
    public string ClassName { get; set; }

    protected BaseLogger()
    {
        ClassName = GetType().Name;
    }

    public abstract void Log(LogLevel logLevel, string message);
}

