namespace Logger;
using System;
public class LogFactory
{
    private string? _logFilePath;
    public void ConfigureFileLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }
    public BaseLogger? CreateLogger(string className)
    {
        if(_logFilePath == null)
        {
            return null;
        }

        FileLogger fileLogger = new FileLogger(_logFilePath)
        {
            ClassName = className
        };
        return fileLogger;
    }
}
