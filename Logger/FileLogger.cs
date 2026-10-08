using System;

namespace Logger;

/// <summary>
/// FileLogger is a concrete implementation of the BaseLogger class that logs messages to a specified file. It appends log messages to the file, including the current date/time, the name of the class that created the logger, the log level, and the message itself.
/// </summary>
public class FileLogger : BaseLogger
{
    private readonly string _filePath;
    private readonly object _writeLock = new();

    /// <summary>
    /// Initializes a new instance of the FileLogger class with the specified file path.
    /// </summary>
    /// <param name="filePath">The path to the file where log messages will be written.</param>
    public FileLogger(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
    }

    /// <summary>
    /// Logs a message to the specified file, including the current date/time, the name of the class that created the logger, the log level, and the message itself. Each log entry is appended on its own line in the file.
    /// </summary>  
    /// <param name="logLevel"> the type of message being logged.</param>
    /// <param name="message">  the string to write to the log </param>
    public override void Log(LogLevel logLevel, string message)
    {
        var logEntry = $"{DateTime.Now} {ClassName} {logLevel}: {message}";
        try
        {
            lock (_writeLock)
            {
                System.IO.File.AppendAllText(_filePath, logEntry + Environment.NewLine);
            }
        }
        catch (System.IO.IOException)
        {
            // Swallow IO exceptions to avoid crashing the host application in error scenarios (disk full, locked file, etc.).
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore permission issues similarly.
        }
    }
}