using System;
using System.Globalization;
namespace Logger;

public static class BaseLoggerExtensions
{
  public static void Error(this BaseLogger? logger, string? message, params object[] args)
  {
    ArgumentNullException.ThrowIfNull(logger);
    ArgumentNullException.ThrowIfNull(message);

    var formatted = (args != null && args.Length > 0) ? string.Format(CultureInfo.InvariantCulture, message, args) : message;
    logger.Log(LogLevel.Error, formatted);
  }

  public static void Warning(this BaseLogger? logger, string? message, params object[] args)
  {
    ArgumentNullException.ThrowIfNull(logger);
    ArgumentNullException.ThrowIfNull(message);

    var formatted = (args != null && args.Length > 0) ? string.Format(CultureInfo.InvariantCulture, message, args) : message;
    logger.Log(LogLevel.Warning, formatted);
  }

  public static void Information(this BaseLogger? logger, string? message, params object[] args)
  {
    ArgumentNullException.ThrowIfNull(logger);
    ArgumentNullException.ThrowIfNull(message);

    var formatted = (args != null && args.Length > 0) ? string.Format(CultureInfo.InvariantCulture, message, args) : message;
    logger.Log(LogLevel.Information, formatted);
  }

  public static void Debug(this BaseLogger? logger, string? message, params object[] args)
  {
    ArgumentNullException.ThrowIfNull(logger);
    ArgumentNullException.ThrowIfNull(message);

    var formatted = (args != null && args.Length > 0) ? string.Format(CultureInfo.InvariantCulture, message, args) : message;
    logger.Log(LogLevel.Debug, formatted);
  }
}
