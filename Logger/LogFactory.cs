using System;

namespace Logger;

public class LogFactory
{
    string? _filePath;

    public BaseLogger? CreateLogger(string className)
    {
        return _filePath == null ? null : new FileLogger(_filePath)
        {
            ClassName = className,
        };
    }

    public void ConfigureFileLogger(string filePath)
    {
        _filePath = filePath;
    }
}
