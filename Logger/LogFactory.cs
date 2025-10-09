using System;

namespace Logger;

public class LogFactory
{
    private string? _filePath;

    public void ConfigureFileLogger(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be empty or whitespace", nameof(filePath));
        }
        _filePath = filePath;
    }

    public BaseLogger? CreateLogger(string className)
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            return null;
        }

        return new FileLogger(_filePath)
        {
            ClassName = className
        };
    }
}
