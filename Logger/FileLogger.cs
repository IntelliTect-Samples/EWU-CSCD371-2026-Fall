using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class FileLogger : BaseLogger
    {
        public string FilePath { get; }

        public FileLogger(string filePath)
        {
            FilePath = filePath;
        }

        public override void Log(LogLevel logLevel, string message)
        {
            string timestamp = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
            string className = this.ClassName ?? this.GetType().Name;
            string formattedMessage = $"{timestamp} {className} {logLevel}: {message}";

            try
            {
                File.AppendAllText(FilePath, formattedMessage + Environment.NewLine);
            }
            catch (IOException ex)
            {
                // Optional: handle file I/O errors gracefully
                Console.Error.WriteLine($"Failed to write log to file: {ex.Message}");
            }
        }
    }
}
