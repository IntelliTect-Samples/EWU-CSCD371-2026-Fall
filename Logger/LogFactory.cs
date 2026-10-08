namespace Logger;

public class LogFactory
{
    private string? _fileLoggerPathway;
    public void ConfigureFileLogger(string filePath)
    {
        _fileLoggerPathway = filePath;
    }
    
    public BaseLogger? CreateLogger(string className)
    {
        
        if (_fileLoggerPathway == null)
        {
            return null;
        }
        return new FileLogger(_fileLoggerPathway) { ClassName = className };
    }

}
