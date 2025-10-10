using System;

namespace Logger;

public class LogFactory
{
    private string? _ConfiguredFilePath; // ? for nullable, _ for private

    public BaseLogger? CreateLogger(string className)
    {
        if (_ConfiguredFilePath == null) // return null if not configured
            return null;

        return new FileLogger(_ConfiguredFilePath)
        {
            LoggerClassName = className
        };
    }

    public void ConfigureFileLogger(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException($"File path cannot be null or whitespace: '{path}'", nameof(path));

        _ConfiguredFilePath = path;
    }
}
