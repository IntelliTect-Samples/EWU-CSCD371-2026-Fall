using System;

namespace Logger;

public static class BaseLoggerMixins
{
    public static void Error(this BaseLogger logger, string message, params object[] arguments)
    {
        if (logger == null) 
            throw new ArgumentNullException();

        logger.Log(LogLevel.Error, (arguments == null || arguments.Length == 0) ? message : string.Format(message,arguments));
    }
    public static void Warning(this BaseLogger logger, string message, params object[] arguments)
    {
        if (logger == null) throw new ArgumentNullException();

        logger.Log(LogLevel.Warning, (arguments == null || arguments.Length == 0) ? message : string.Format(message, arguments));
    }
    public static void Information(this BaseLogger logger, string message, params object[] arguments)
    {
        if (logger == null) throw new ArgumentNullException();

        logger.Log(LogLevel.Information, (arguments == null || arguments.Length == 0) ? message : string.Format(message, arguments));
    }
    public static void Debug(this BaseLogger logger, string message, params object[] arguments)
    {
        if (logger == null) throw new ArgumentNullException();

        logger.Log(LogLevel.Debug, (arguments == null || arguments.Length == 0) ? message : string.Format(message, arguments));
    }
}