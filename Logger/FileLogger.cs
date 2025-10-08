using System;
using System.IO;

namespace Logger
{
    public class FileLogger(string filePath) : BaseLogger
    {
        private readonly string _filePath = filePath;

        public override void Log(LogLevel logLevel, string message)
        {
            using (StreamWriter writer = File.AppendText(_filePath))
            {
                writer.WriteLine($"{DateTime.Now} {ClassName} {logLevel} {message}");
            }
        }
    }
}
