using System;

using System.Globalization;

namespace Logger;

public static class BaseLoggerExtensions
{
    public static void Error(this BaseLogger logger, string message, params object[] args)
    {
       ArgumentNullException.ThrowIfNull(logger);

        string formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);

        logger.Log(LogLevel.Error, formattedMessage);
    }
    public static void Warning(this BaseLogger logger, string message, params object[] args)
    {
         ArgumentNullException.ThrowIfNull(logger);

        string formattedMessage = string.Format(CultureInfo.InvariantCulture,message, args);

        logger.Log(LogLevel.Warning, formattedMessage);
    }

public static void Information(this BaseLogger logger, string message, params object[] args)
    {
         ArgumentNullException.ThrowIfNull(logger);

        string formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);

        logger.Log(LogLevel.Information, formattedMessage);
    }

    public static void Debug(this BaseLogger logger, string message, params object[] args)
    {
         ArgumentNullException.ThrowIfNull(logger);

        string formattedMessage = string.Format(CultureInfo.InvariantCulture, message, args);

        logger.Log(LogLevel.Debug, formattedMessage);
    }
}
