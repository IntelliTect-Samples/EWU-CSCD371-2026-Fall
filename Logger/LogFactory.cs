using System;

namespace Logger;

public class LogFactory
{
    private string? _filePath;
    public BaseLogger? CreateLogger(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            throw new ArgumentException("Class name cannot be null or empty.", nameof(className));
        }

        if (_filePath is null)
        {
            return null;
        }

        return new FileLogger(_filePath)
        {
            ClassName = className
        };
    }
    public void ConfigureFileLogger(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        _filePath = filePath;
    }
}
