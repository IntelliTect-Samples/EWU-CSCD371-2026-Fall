using System;

namespace Logger;

public abstract class BaseLogger
{
    public string? SourceClassName { get; set; }
    public abstract void Log(LogLevel logLevel, string message);

    public static BaseLogger? Create(string className, string? filePath = null)
    {
        throw new NotImplementedException();
    }
}

