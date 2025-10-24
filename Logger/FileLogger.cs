namespace Logger;

public class FileLogger : BaseLogger, ILogger //Todo: Remove Ilogger, it is unnecessary
{
    private FileInfo File { get; }

    public string FilePath { get => File.FullName; }

    public FileLogger(string logSource, string filePath) : base(logSource) => File = new FileInfo(filePath);

    public FileLogger(FileLoggerConfiguration configuration) : this(configuration.LogSource, configuration.FilePath) {}

    static ILogger ILogger.CreateLogger(in ILoggerConfiguration logggerConfiguration) => 
        logggerConfiguration is FileLoggerConfiguration configuration
            ? CreateLogger(configuration)
            : throw new ArgumentException("Invalid configuration type", nameof(logggerConfiguration));
    //Todo: Clean this and choose either explicit or implicit. Only one of these should be implemented
    public static FileLogger CreateLogger(FileLoggerConfiguration configuration) => new(configuration);

    public override void Log(LogLevel logLevel, string message)
    {
        using StreamWriter writer = File.AppendText();
        writer.WriteLine($"{ DateTime.Now },{LogSource},{logLevel},{message}");
    }
}
