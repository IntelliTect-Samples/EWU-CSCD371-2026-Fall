using System;
using System.Globalization;

namespace Logger;

public static class BaseLoggerMixins
{
    public static void Error(this BaseLogger? baseLogger, string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(baseLogger);
        baseLogger.Log(LogLevel.Error, string.Format(CultureInfo.InvariantCulture, message, args));
    }

    public static void Warning(this BaseLogger? baseLogger,string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(baseLogger);
        baseLogger.Log(LogLevel.Warning, string.Format(CultureInfo.InvariantCulture, message, args));
    }

    public static void Information(this BaseLogger? baseLogger, string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(baseLogger);
        baseLogger.Log(LogLevel.Information, string.Format(CultureInfo.InvariantCulture, message, args));
    }

    public static void Debug(this BaseLogger? baseLogger, string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(baseLogger);
        baseLogger.Log(LogLevel.Debug, string.Format(CultureInfo.InvariantCulture, message, args));
    } 
    
    
}
