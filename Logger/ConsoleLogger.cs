using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class ConsoleLogger : BaseLogger
    {
        public override void Log(LogLevel logLevel, string message)
        {
            var timestamp = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
            var format = $"[{timestamp}] [{ClassName}] [{logLevel}]: {message}";
            Console.WriteLine(format);
        }
    }
}
