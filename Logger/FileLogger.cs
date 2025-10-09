using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;


namespace Logger
{
    public class FileLogger : BaseLogger
    {
        private readonly string _filePath;

        public FileLogger(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(path));
            }
            _filePath = path;
        }

        public override void Log(LogLevel logLevel, string message)
        {
            string timeStamp = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            string logText = $"{timeStamp} {ClassName} {logLevel}: {message}";
            File.AppendAllText(_filePath, logText + Environment.NewLine);
        }
    }
}
