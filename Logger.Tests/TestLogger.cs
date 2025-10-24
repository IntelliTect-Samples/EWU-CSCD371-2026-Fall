namespace Logger.Tests;

public class TestLogger : BaseLogger, ILogger
{
    public TestLogger(string logSource) : base(logSource) { }

    public List<(LogLevel LogLevel, string Message)> LoggedMessages { get; } = [];

    public static ILogger CreateLogger<TConfig, TLogger>(TConfig configuration)
        where TConfig : ILoggerConfiguration
        where TLogger : ILogger
    {
        if(configuration is TestLoggerConfiguration testLoggerConfiguration)
            return new TestLogger(testLoggerConfiguration.LogSource);
        else
            throw new ArgumentException("Invalid configuration type", nameof(configuration));
    }
        

    public override void Log(LogLevel logLevel, string message) => LoggedMessages.Add((logLevel, message));
}

public class TestLoggerConfiguration : BaseLoggerConfiguration, ILoggerConfiguration
{
    public TestLoggerConfiguration(string logSource) : base(logSource) { }

}
