namespace Logger;


public class LogFactory
{
    private string? _filePath;

    public void ConfigureFileLogger(string filePath)
    {
        _filePath = filePath;
    }

    public IBaseLogger? CreateLogger(string className)
    {
        if (_filePath == null)
            return null;

        return new FileLogger(_filePath)
        {
            ClassName = className
        };
    }
}
