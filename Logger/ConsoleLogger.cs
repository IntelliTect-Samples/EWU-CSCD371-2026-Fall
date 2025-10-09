using System;

namespace Logger;

public class ConsoleLogger : BaseLogger
{
    public override void Log(LogLevel logLevel, string message)
    {
        Console.WriteLine($"{DateTime.Now} {this.SourceClassName} {logLevel} {message}");
    }

    public new static BaseLogger Create(string className, string? filePath = null)
    {
        ConsoleLogger newLogger = new ConsoleLogger
        {
            SourceClassName = className,
        };
        return newLogger;
    }
}