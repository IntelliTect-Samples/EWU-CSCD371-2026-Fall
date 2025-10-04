using System;
using System.Runtime.CompilerServices;
using System.IO;

namespace Logger;

public class FileLogger : BaseLogger
{
    public string? Path { get; set; }
    public override void Log(LogLevel logLevel, string message)
    {
        File.AppendAllLines(Path, new[] { $"{DateTime.Now} {this.Name} {logLevel} {message}" });
    }

    public new static BaseLogger? Create(string className, string? filePath = null)
    {
        if (filePath == null)
        {
            return null;
        }

        FileLogger newLogger = new FileLogger
        {
            Name = className,
            Path = filePath
        };
        return newLogger;
    }
}