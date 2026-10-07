namespace Logger;

public class FileLogger : BaseLogger
{
    public string FilePath { get; }

    public FileLogger(string filePath)
    {
        FilePath = filePath;
    }

    public override void Log(LogLevel logLevel, string message)
    {
        string line = $"{DateTime.Now} {ClassName} {logLevel}: {message}{Environment.NewLine}";
        File.AppendAllText(FilePath, line);
    }


}