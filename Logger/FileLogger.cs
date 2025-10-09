using System;
using System.IO;

namespace Logger;

public class FileLogger : BaseLogger
{
    public string? FilePath { get; set; }
    public override void Log(LogLevel logLevel, string message)
    {
        File.AppendAllLines(FilePath!, new[] { $"{DateTime.Now} {this.SourceClassName} {logLevel}: {message}" });
    }

    public new static BaseLogger? Create(string className, string? filePath = null)
    {
        if (filePath == null)
        {
            return null;
        }

        FileLogger newLogger = new FileLogger
        {
            SourceClassName = className,
            FilePath = filePath
        };
        return newLogger;
    }
}