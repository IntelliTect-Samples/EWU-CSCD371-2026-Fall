using System;
using System.Globalization;
using System.IO;

namespace Logger;

/// <summary>
/// BaseLogger implementation that writes log entries to a file.
/// FilePath must be provided at construction, and each call to <see cref="Log"/>
/// appends a formatted string with a timestamp, the <see cref="BaseLogger.ClassName"/>,
/// the given log level, and the message.
/// Derived from <see cref="BaseLogger"/>
/// </summary>
/// <param name="filePath">The path to the file to append logs to</param>
public class FileLogger(string filePath) : BaseLogger
{
    public override void Log(LogLevel logLevel, string message)
    {
        string timestamp = DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
        
        string line = $"{timestamp} {ClassName} {logLevel} : {message}";
        
        File.AppendAllText(filePath, line + Environment.NewLine);
    }
}