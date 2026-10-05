using System;

namespace Logger;

public static class BaseLoggerExtensions
{
    // CORE 5:
    // each should: take a string for the message, as well as param array of arguments.
    // each is: shortcut for calling BaseLogger.Log, auto supplying appropriate LogLevel.
    // methods should throw an exception if the BaseLogger param is null.
    extension(BaseLogger? logger)
    {
        // CORE: extension method for Error
        public void Error(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);

            logger.Log(LogLevel.Error, string.Format(message, args));
        }

        // CORE: extension method for Warning
        public void Warning(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);
        
            logger.Log(LogLevel.Warning, string.Format(message, args));
        }

        // CORE: extension method for Information
        public void Information(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);
        
            logger.Log(LogLevel.Information, string.Format(message, args));
        }

        // CORE: extension method for debug
        public void Debug(string message, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(logger);
        
            logger.Log(LogLevel.Debug, string.Format(message, args));
        }
    }
}
