using System;

namespace Logger;

/// <summary>
/// Used to create <see cref="BaseLogger"/> instances of concrete type <see cref="FileLogger"/>.
/// Ensure <see cref="ConfigureFileLogger"/> is called prior to <see cref="CreateLogger"/>
/// </summary>
public class LogFactory
{
    private string? _fileLoggerPath;
    
    /// <summary>
    /// Creates a new <see cref="BaseLogger"/> instance using the configured FilePath.
    /// </summary>
    /// <param name="className">The name of the class creating the logger</param>
    /// <returns>
    /// A <see cref="BaseLogger"/> of concrete type <see cref="FileLogger"/>,
    /// or <c>null</c> if the factory was not configured prior to call.
    /// </returns>
    public BaseLogger? CreateLogger(string className)
    {
        ArgumentNullException.ThrowIfNull(className);
        
        return _fileLoggerPath is null ? null : new FileLogger(_fileLoggerPath) { ClassName = className };
    }
    
    /// <summary>
    /// Configures the factory with the file path,
    /// so that <see cref="FileLogger"/> instances created by this factory,
    /// write log entries to this specified location.
    /// </summary>
    /// <param name="filePath">The path where log entries should be written</param>
    public void ConfigureFileLogger(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);

        _fileLoggerPath = filePath;
    }
}
