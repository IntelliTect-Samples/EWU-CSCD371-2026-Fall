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
            string timestamp = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
            string className = ClassName;
            Console.WriteLine($"{timestamp} {className} {logLevel}: {message}");
        }
    }
}
