using System;

namespace Logger;

/// <summary>
/// BaseLogger implementation that writes log entries to the console.
/// appends a formatted string with a timestamp, the <see cref="BaseLogger.ClassName"/>,
/// the given log level, and the message.
/// Derived from <see cref="BaseLogger"/>
/// </summary>
public class ConsoleLogger : BaseLogger
{
    public override void Log(LogLevel level, string message)
    {
        string timestamp = DateTime.Now.ToString("G");

        // output should include:
        string line = $"{timestamp} {ClassName} {level} : {message}";
        // the current date/time
        // the name of the class that created the logger
        // the log level
        // the message

        Console.WriteLine(line);
    }
}