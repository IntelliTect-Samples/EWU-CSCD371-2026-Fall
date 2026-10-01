using System.IO;
using System;

namespace Logger;

public class FileLogger : BaseLogger
{
    private readonly string path
    ;

    public FileLogger(string filePath)
    {
        path
         = filePath;
    }

    public override void Log(LogLevel logLevel, string message)
    {
        string line = $"{DateTime.Now} {ClassName} {logLevel}: {message}{Environment.NewLine}";
        File.AppendAllText(path
        , line);
    }
}