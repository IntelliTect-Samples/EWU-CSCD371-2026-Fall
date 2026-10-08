namespace Logger;

public class LogFactory
{
    private string? _filePath;
    private readonly object _lock = new();
    /// <summary>
    /// Creates a new instance of a logger for the specified class name. If the file logger has not been configured, it returns null.   
    /// </summary>
    /// <param name="className">
    /// <returns> a "FileLogger" object>
    public BaseLogger? CreateLogger(string className) // initialize classname at logger creation
    {
        string? filePath;
        lock (_lock)
        {
            filePath = _filePath;
        }

        if (string.IsNullOrEmpty(filePath))
        {
            return null;
        }

        return new FileLogger(filePath) { ClassName = className };
    }


    /// <summary>
    /// Configures the file logger with the specified file path. This method should be called before creating any loggers.
    /// </summary>
    /// <param name="filePath"></param>
    public void ConfigureFileLogger(string filePath)
    {
        lock (_lock)
        {
            _filePath = filePath;
        }
    }
}
