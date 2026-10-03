namespace Logger;
using System;
using System.IO;
public class FileLogger : BaseLogger
{

    private readonly string filePath;

    public FileLogger(string fp)
    {
        filePath = fp;
    
    }
    public override void Log(LogLevel logLevel, string message)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm tt");
        message = $"{date} {className} {logLevel}: {message}";
        File.AppendAllText(filePath, message + Environment.NewLine);
    }
    
}