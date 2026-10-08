using System;

namespace Logger;

public static class BaseLoggerExtensions
{
    
    public static void Error(this BaseLogger logger, string message, params object[] args)
    {
        
        logger.Log(LogLevel.Error, string.Format(message, args));
    }
    public static void Warning(this BaseLogger logging, string notes, params object[] args)
    {
        
        logging.Log(LogLevel.Warning, string.Format(notes, args));
    }
    public static void Information(this BaseLogger logger, string message, params object[] args)
    {
        
        logger.Log(LogLevel.Information, string.Format(message, args));
    }
    public static void Debug(this BaseLogger logger, string message, params object[] args)
    {
        
        logger.Log(LogLevel.Debug, string.Format(message, args));
    }
}
