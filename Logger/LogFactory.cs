using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;

namespace Logger;

public class LogFactory
{
    private readonly Dictionary<LogFormat, Type> loggers = new Dictionary<LogFormat, Type>(){ {
        LogFormat.File, typeof(FileLogger) }};
    private static string? FilePath { get; set; }
    public void ConfigureFileLogger(string loggerFilePath)
    {
        FilePath = loggerFilePath;
    }

    public BaseLogger? CreateLogger(string className, LogFormat format = LogFormat.Console)
    {
        var method = loggers[format].GetMethod("Create", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        BaseLogger? newLogger = (BaseLogger?)method.Invoke(null, [className, FilePath!]);

        return newLogger;
    }
}
