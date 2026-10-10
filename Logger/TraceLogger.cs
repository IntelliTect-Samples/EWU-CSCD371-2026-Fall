using System.Diagnostics;
using System.Globalization;


namespace Logger;

/// <summary>
/// Writes logs using System.Trace instead of a file
/// </summary>
public class TraceLogger : BaseLogger, ILogger<TraceLogger>
{
    public static TraceLogger CreateLogger(string className)
    {
        return new TraceLogger() { ClassName = className };
    }

    public override void Log(LogLevel logLevel, string message)
    {
        Trace.WriteLine(string.Create(CultureInfo.CurrentCulture,
            $"{DateTime.Now} {base.ClassName} {logLevel}: {message}"));
    }
}
