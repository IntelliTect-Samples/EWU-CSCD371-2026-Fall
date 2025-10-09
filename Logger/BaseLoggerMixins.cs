using System;
using System.Globalization;

namespace Logger;

public static class BaseLoggerMixins
{
    public static void LogFormatHelper(this BaseLogger logger, LogLevel level, string message, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        string formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);
        logger.Log(level, formattedMessage);
    }

    public static void Error(this BaseLogger logger, string message, params object[] args)
    {
        logger.LogFormatHelper(LogLevel.Error, message, args);
    }
    public static void Warning(this BaseLogger logger, string message, params object[] args)
    {
        logger.LogFormatHelper(LogLevel.Warning, message, args);
    }
    public static void Information(this BaseLogger logger, string message, params object[] args)
    {
        logger.LogFormatHelper(LogLevel.Information, message, args);
    }
    public static void Debug(this BaseLogger logger, string message, params object[] args)
    {
        logger.LogFormatHelper(LogLevel.Debug, message, args);
    }
}
