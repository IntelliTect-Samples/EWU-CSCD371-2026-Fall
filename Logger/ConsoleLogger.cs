using System;
using System.Globalization;

namespace Logger;

/// <summary>
/// BaseLogger implementation that writes log entries to the console.
/// appends a formatted string with a timestamp, the <see cref="BaseLogger.ClassName"/>,
/// the given log level, and the message.
/// Derived from <see cref="BaseLogger"/>
/// </summary>
public class ConsoleLogger : BaseLogger, ILogger
{
    public override void Log(LogLevel logLevel, string message)
    {
        string timestamp = DateTime.Now.ToString("G", CultureInfo.InvariantCulture);
        
        string line = $"{timestamp} {ClassName} {logLevel} : {message}";

        Console.WriteLine(line);
    }
    
    public static ILogger Create(string className)
    {
        ArgumentNullException.ThrowIfNull(className);
        
        return new ConsoleLogger { ClassName = className };
    }
}