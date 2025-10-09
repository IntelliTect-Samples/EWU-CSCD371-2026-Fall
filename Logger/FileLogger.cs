using System;
using System.IO;

namespace Logger
{
    public class FileLogger : BaseLogger
    {
        private readonly string _FilePath;

        public FileLogger(string filePath) 
        {
            _FilePath = filePath;
        }
        public override void Log(LogLevel logLevel, string message)
        {
            string logLine = $"{DateTime.Now} {ClassName} {logLevel}: {message}";
            File.AppendAllText(_FilePath, logLine + Environment.NewLine);
        }

    }
}
