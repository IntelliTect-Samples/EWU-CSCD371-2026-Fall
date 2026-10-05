using System;

namespace Logger;

public static class BaseLoggerExtensions
{
    extension(BaseLogger? logger)
    {
        public void Error(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);

            logger.Log(LogLevel.Error, string.Format(message, args));
        }
        
        public void Warning(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);
        
            logger.Log(LogLevel.Warning, string.Format(message, args));
        }
        
        public void Information(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);
        
            logger.Log(LogLevel.Information, string.Format(message, args));
        }
        
        public void Debug(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);
        
            logger.Log(LogLevel.Debug, string.Format(message, args));
        }
    }
}
