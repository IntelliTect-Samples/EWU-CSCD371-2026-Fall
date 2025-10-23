namespace Logger.Tests;

public class TestLogger : BaseLogger, ILogger
{
    public TestLogger(string logSource) : base(logSource) { }

    public List<(LogLevel LogLevel, string Message)> LoggedMessages { get; } = [];

    // TODO: replace ILogger with TestLogger so that the method signature is different than ILogger.CreateLogger
    public static ILogger CreateLogger(in TestLoggerConfiguration configuration) =>
        new TestLogger(configuration.LogSource);

    // TODO: replace `static` with `public override` to match updated BaseLogger class, remove explicit implementation `ILogger.CreateLogger` => `CreateLogger`
    static ILogger ILogger.CreateLogger(in ILoggerConfiguration configuration) =>
        configuration is TestLoggerConfiguration testLoggerConfiguration
            ? CreateLogger(testLoggerConfiguration)
            : throw new ArgumentException("Invalid configuration type", nameof(configuration));

    public override void Log(LogLevel logLevel, string message) => LoggedMessages.Add((logLevel, message));
}

public class TestLoggerConfiguration : BaseLoggerConfiguration, ILoggerConfiguration
{
    public TestLoggerConfiguration(string logSource) : base(logSource) { }

}
