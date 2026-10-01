namespace Logger;

/// <summary>
/// Used to create <see cref="BaseLogger"/> instances of concrete type <see cref="FileLogger"/>.
/// Ensure <see cref="ConfigureFileLogger"/> is called prior to <see cref="CreateLogger"/>
/// </summary>
public class LogFactory
{
    // CORE 3.2: store file path in a private member
    // this is implicitly null prior to ConfigureFileLogger call
    // TODO: Test prove this is null prior to configure
    private string _fileLoggerPath;
    
    /// <summary>
    /// Creates a new <see cref="BaseLogger"/> instance using the configured FilePath.
    /// </summary>
    /// <param name="className">The name of the class creating the logger</param>
    /// <returns>
    /// A <see cref="BaseLogger"/> of concrete type <see cref="FileLogger"/>,
    /// or <c>null</c> if the factory was not configured prior to call.
    /// </returns>
    public BaseLogger CreateLogger(string className)
    {
        // CORE 3.3: use file path when instantiating a new FileLogger in CreateLogger method
        return _fileLoggerPath is null ?
            
            // CORE 4: if file logger is not configured in the LogFactory, it's CreateLogger should return null.
            null : 
            
            // CORE 1.2: The class name auto-prop should be set in LogFactory using object initializer
            (BaseLogger)new FileLogger(_fileLoggerPath) { ClassName = className };
    }
    
    // CORE 3.1: the LogFactory should be updated with a new method ConfigureFileLogger,
    // should take a file path and store it in a private member: see _fileLoggerPath.
    /// <summary>
    /// Configures the factory with the file path,
    /// so that <see cref="FileLogger"/> instances created by this factory,
    /// write log entries to this specified location.
    /// </summary>
    /// <param name="filePath">The path where log entries should be written</param>
    public void ConfigureFileLogger(string filePath)
    {
        _fileLoggerPath = filePath;
    }
}
