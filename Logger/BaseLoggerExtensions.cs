using System;

namespace Logger;

using System;

using System.Globalization; // Fix for github class pipeline

public static class BaseLoggerExtensions
{
    
    public static void Error(this BaseLogger logger, string message, params object[] args)
    {
        
        logger.Log(LogLevel.Error, string.Format(CultureInfo.CurrentCulture, message, args));
    }
    public static void Warning(this BaseLogger logging, string notes, params object[] args)
    {
        
        logging.Log(LogLevel.Warning, string.Format(CultureInfo.CurrentCulture, notes, args));
    }
    public static void Information(this BaseLogger logger, string message, params object[] args)
    {
        
        logger.Log(LogLevel.Information, string.Format(CultureInfo.CurrentCulture, message, args));
    }
    public static void Debug(this BaseLogger logger, string message, params object[] args)
    {
        
        logger.Log(LogLevel.Debug, string.Format(CultureInfo.CurrentCulture, message, args));
    }
}
