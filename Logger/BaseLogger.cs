namespace Logger;

public abstract class BaseLogger
{
    /// <summary>
    /// Writes a log entry with the given <paramref name="logLevel"/> and <paramref name="message"/>.
    /// Implementations dictate the format and 'location' of the log.
    /// </summary>
    /// <param name="logLevel">The level of importance for this message</param>
    /// <param name="message">The message to log</param>
    public abstract void Log(LogLevel logLevel, string message);
    
    /// <summary>
    /// The name of the class that created this logger instance. Set once during initialization.
    /// </summary>
    // CORE 1.1: BaseLogger needs an auto property to store class name
    public string ClassName { get; init; }
}

