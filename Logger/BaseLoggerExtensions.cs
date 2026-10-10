using System.Globalization;

namespace Logger;

/// <summary>
/// Extension methods that act as shortcuts for different log levels.
/// </summary>
public static class BaseLoggerExtensions
{
    public static void Error(this BaseLogger logger, string message, params object?[] args) =>
        LogMessage(logger, LogLevel.Error, message, args);

    public static void Warning(this BaseLogger logger, string message, params object?[] args) =>
        LogMessage(logger, LogLevel.Warning, message, args);
    public static void Information(this BaseLogger logger, string message, params object?[] args) =>
        LogMessage(logger, LogLevel.Information, message, args);

    public static void Debug(this BaseLogger logger, string message, params object?[] args) =>
        LogMessage(logger, LogLevel.Debug, message, args);

    private static void LogMessage(BaseLogger logger, LogLevel level, string message, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        logger.Log(level, string.Format(CultureInfo.CurrentCulture, message, args));
    }
}