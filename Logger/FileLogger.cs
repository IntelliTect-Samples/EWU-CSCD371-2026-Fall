using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Logger
{
    public sealed class FileLogger : BaseLogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        public override void Log(LogLevel logLevel, string message)
        {
            var line = $"{DateTime.Now:G} {ClassName} {logLevel}: {message}";
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }
}
