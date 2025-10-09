using System;

namespace Logger;

public class ConsoleLogger : BaseLogger
{
    public override void Log(LogLevel logLevel, string message)
    {
        Console.WriteLine($"{DateTime.Now} {this.Name} {logLevel} {message}");
    }

    public new static BaseLogger Create(string className, string? filePath = null)
    {
        ConsoleLogger newLogger = new ConsoleLogger
        {
            Name = className,
        };
        return newLogger;
    }
}