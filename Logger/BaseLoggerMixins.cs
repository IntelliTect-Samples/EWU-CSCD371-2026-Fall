using System;
using System.Globalization;

namespace Logger;

public static class BaseLoggerMixins
{
    private static void Invoke(BaseLogger? logger, LogLevel level, string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        logger.Log(level, string.Format(CultureInfo.CurrentCulture, message, args));
    }

    public static void Error(this BaseLogger? logger, string message, params object[] args)
    {
        Invoke(logger, LogLevel.Error, message, args);
    }

    public static void Warning(this BaseLogger? logger, string message, params object[] args)
    {
        Invoke(logger, LogLevel.Warning, message, args);
    }

    public static void Information(this BaseLogger? logger, string message, params object[] args)
    {
        Invoke(logger, LogLevel.Information, message, args);
    }

    public static void Debug(this BaseLogger? logger, string message, params object[] args)
    {
        Invoke(logger, LogLevel.Debug, message, args);
    }
}
