using System;
using System.Globalization;

namespace Logger;

public class ConsoleLogger : IBaseLogger
{
    public string ClassName { get; set; } = string.Empty;

    public void Log(LogLevel logLevel, string message)
    {
        string timestamp = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
        string logEntry = $"{timestamp} {ClassName} {logLevel}: {message}";
        
        Console.WriteLine(logEntry);
    }
}