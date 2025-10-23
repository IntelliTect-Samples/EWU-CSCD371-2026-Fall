namespace Logger;

// TODO: the comment below is not true. ILogger should be implemented here
// We do not implement ILogger here because you can
// only have abstract static methods on interfaces.
public abstract class BaseLogger // TODO : ILogger
{
    public string LogSource { get; }
    public BaseLogger(string logSource) => LogSource = string.IsNullOrWhiteSpace(logSource)
            ? throw new ArgumentException($"'{nameof(logSource)}' cannot be null or whitespace.", nameof(logSource))
            : logSource;

    public abstract void Log(LogLevel logLevel, string message);

    // TODO uncomment & remove static from method signature to match updated ILogger interface
    // You can only have abstract static methods on interfaces.
    // public abstract static ILogger CreateLogger(in ILoggerConfiguration configuration);
}
