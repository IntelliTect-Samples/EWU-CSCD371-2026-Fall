using System;
using System.Runtime.CompilerServices;

namespace Logger;

public class FileLogger : BaseLogger
{
    public override void Log(LogLevel logLevel, string message)
    {
        Console.WriteLine($"{DateTime.Now} {nameof(this.name)} {logLevel} {message}");
    }
}