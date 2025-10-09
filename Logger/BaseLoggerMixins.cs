using System;
using System.Globalization;

namespace Logger;

public static class BaseLoggerMixins
{
    public static void Error(this BaseLogger logger, string message, params object[] arguments) =>
        LogInternal(logger, LogLevel.Error, message, arguments);

    public static void Warning(this BaseLogger logger, string message, params object[] arguments) =>
        LogInternal(logger, LogLevel.Warning, message, arguments);

    public static void Information(this BaseLogger logger, string message, params object[] arguments) =>
        LogInternal(logger, LogLevel.Information, message, arguments);

    public static void Debug(this BaseLogger logger, string message, params object[] arguments) =>
        LogInternal(logger, LogLevel.Debug, message, arguments);

    private static void LogInternal(BaseLogger logger, LogLevel level, string message, object[] arguments)
    {
        if (logger == null)
            throw new ArgumentNullException(nameof(logger));

        var formattedMessage = (arguments == null || arguments.Length == 0) ? message: string.Format(message, arguments);

        logger.Log(level, formattedMessage);
    }
}
