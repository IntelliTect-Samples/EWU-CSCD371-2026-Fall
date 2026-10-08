namespace Logger;


/*
 * BaseLogger is an abstract class that defines the basic logging functionality.
 * It provides a method to log messages with a specified log level.
 */
public abstract class BaseLogger
{
    public abstract void Log(LogLevel logLevel, string message);

    public string? ClassName { get; set; } //auto property to store the class name that created the logger. declared as nullable
}

