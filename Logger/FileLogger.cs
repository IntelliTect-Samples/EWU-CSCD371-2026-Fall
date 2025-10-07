using System;
using System.Globalization;
using System.IO;

namespace Logger;

public class FileLogger : IBaseLogger
{
    public string ClassName { get; set; } = string.Empty;
    public string FilePath { get; }

    public FileLogger(string filePath)
    {
        FilePath = filePath;
    }

    public void Log(LogLevel logLevel, string message)
    {
        string timestamp = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        string logEntry = $"{timestamp} {ClassName} {logLevel}: {message}";
        
        File.AppendAllText(FilePath, logEntry + Environment.NewLine);
    }
}