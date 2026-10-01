using System;
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
// CORE 2.1: FileLogger derives from BaseLogger, it should take a path to a file to write log message to.
public class FileLogger(string filePath) : BaseLogger
{
    public override void Log(LogLevel level, string message)
    {
        string timestamp = DateTime.Now.ToString("G");
        
        // CORE 2.2: output should include:
        string line = 
            $"{timestamp} " + // the current date/time
            $"{ClassName} " + // the name of the class that created the logger
            $"{level} : " + // the log level
            $"{message}"; // the message
        
        // when its log method is called, it should append messages on their own line.
        File.AppendAllText(filePath, line + Environment.NewLine);
    }
}