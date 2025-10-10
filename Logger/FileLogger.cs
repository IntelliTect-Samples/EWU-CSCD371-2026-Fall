using System;
using System.IO;

namespace Logger;

public class FileLogger : BaseLogger
{ 
    private readonly string _filePath;

    public FileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public override void Log(LogLevel logLevel, string message)
    {
        string logMessage = $"{DateTime.Now}: {ClassName} [{logLevel}] {message}";
        File.AppendAllText(_filePath, logMessage + "\n");
    }
}